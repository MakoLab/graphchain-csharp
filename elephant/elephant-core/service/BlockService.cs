using System;
using System.Collections.Generic;
using elephant.core.exception;
using elephant.core.model.chain;
using elephant.core.model.rdf;
using elephant.core.persistence;
using elephant.core.ramt;
using elephant.core.service.cryptography;
using elephant.core.service.hashing;
using elephant.core.util;
using elephant.core.vocabulary;
using Microsoft.Extensions.Logging;

namespace elephant.core.service
{
    public class BlockService
    {
        private ILogger<BlockService> _logger;
        private string _chainGraphIri;
        private IHashCalculator _hashCalculator;
        private IHashingService _hashingService;
        private RepositoryManager _repositoryManager;
        private DotNetRdfSerializationHandler _rdf4jSerializationHandler;
        private DotNetRdfMapper _rdf4jMapper;

        private long _lastIndex;

        public BlockService(ILogger<BlockService> logger, IHashCalculator hashCalculator, IHashingService hashingService, RepositoryManager repositoryManager, StoreConfiguration sConfig)
        {
            _logger = logger;
            _chainGraphIri = sConfig.ChainGraphIri;
            _hashCalculator = hashCalculator;
            _hashingService = hashingService;
            _repositoryManager = repositoryManager;

            _rdf4jSerializationHandler = new DotNetRdfSerializationHandler();
            _rdf4jMapper = new DotNetRdfMapper();
        }

        public void Init()
        {
            _logger.LogDebug("[CONFIG] Initializing the block service...");
            try
            {
                LastBlockInfo lastBlockInfo = _repositoryManager.GetLastBlockInfo();
                _lastIndex = Int64.Parse(lastBlockInfo.BlockIndex);
            }
            catch (ReadingBlockException ex)
            {
                _logger.LogError("Exception occurred while tried to determine last index.", ex);
                throw new Exception("Unable to determine the last index value. Aborting initialization of BlockService.");
            }
            _logger.LogInformation("The block service initialized successfully.");
        }

        public void AddBlock(BlockHeader blockHeader, BlockContent blockContent)
        {
            var blockIri = GenerateNewBlockIri(blockHeader.GetIndexAsInt());
            Block blockToStore = new Block(blockIri, blockHeader, blockContent);
            _repositoryManager.PersistBlock(blockToStore, false);
            _lastIndex++;
        }

        public Block CreateBlock(string dataGraphIri, string rawRdf, RdfFormat rdfFormat)
        {
            _logger.LogDebug("Creating block with graphIri '{0}' and rdfFormat '{1}'...", dataGraphIri, rdfFormat.GetJenaName());

            try
            {
                HashSet<Triple> triples = GetTriplesFromSerializedModel(rawRdf, rdfFormat);

                long newIndex = GenerateNewIndex();
                string newBlockIri = GenerateNewBlockIri(newIndex);

                LastBlockInfo lastBlockInfo = _repositoryManager.GetLastBlockInfo();
                string previousBlock = lastBlockInfo.BlockIri;
                string previousHash = lastBlockInfo.BlockHash;
                string timestamp = TimestampCreator.CreateTimestampString();
                string dataHash = _hashingService.CalculateHash(triples);
                string stringToCalculateHash = (newIndex + previousBlock + previousHash + timestamp + dataGraphIri + dataHash).Trim();
                string hash = _hashCalculator.CalculateHash(stringToCalculateHash).ToLower();

                BlockHeader blockHeader = new BlockHeader(
                    dataGraphIri,
                    dataHash,
                    hash,
                    newIndex.ToString(),
                    previousBlock,
                    previousHash,
                    timestamp);
                BlockContent blockContent = new BlockContent(dataGraphIri, triples);

                Block blockToStore = new Block(newBlockIri, blockHeader, blockContent);

                _repositoryManager.PersistBlock(blockToStore, true);

                // MAYBE: Maybe this should be obtained from Triple Store in order to avoid some kind of inconsistency.
                _lastIndex++;

                return GetBlock(newIndex.ToString());
            }
            catch (ReadingBlockException ex)
            {
                string msg = "Exception was thrown while getting information about the last block.";
                throw new CreatingBlockException(msg, ex);
            }
            catch (RdfSerializationException ex)
            {
                string msg = String.Format("Exception was thrown while deserializing RDF model from '{0}' format.", rdfFormat);
                throw new CreatingBlockException(msg, ex);
            }
            catch (CalculatingHashException ex)
            {
                throw new CreatingBlockException("Exception was thrown while calculating hash.", ex);
            }
        }

        public void ClearRepository()
        {
            _repositoryManager.ClearRepository();
        }

        public List<BlockHeader> GetAllBlocks()
        {
            List<BlockHeader> blockHeaders = new List<BlockHeader>();

            bool getPreviousBlock = true;

            LastBlockInfo lastBlockInfo = _repositoryManager.GetLastBlockInfo();
            String blockIriToObtain = lastBlockInfo.BlockIri;

            while (getPreviousBlock)
            {
                Block block = GetBlockByIri(blockIriToObtain);

                blockHeaders.Add(block.BlockHeader);

                blockIriToObtain = block.BlockHeader.PreviousBlock;

                if (blockIriToObtain.Equals(block.BlockIri))
                {
                    _logger.LogDebug("Block '{0}' is the first block.", blockIriToObtain);
                    getPreviousBlock = false;
                }
            }

            return blockHeaders;
        }

        public List<BlockContent> GetAllBlockContents()
        {
            var result = new List<BlockContent>();
            bool getPreviousBlock = true;
            var lastBlockInfo = _repositoryManager.GetLastBlockInfo();
            var blockIriToObtain = lastBlockInfo.BlockIri;
            while (getPreviousBlock)
            {
                Block block = GetBlockByIri(blockIriToObtain);
                result.Add(block.BlockContent);
                blockIriToObtain = block.BlockHeader.PreviousBlock;
                if (blockIriToObtain.Equals(block.BlockIri))
                {
                    getPreviousBlock = false;
                }
            }
            return result;
        }

        public Block GetLatestBlock()
        {
            var lastBlockInfo = _repositoryManager.GetLastBlockInfo();
            var blockIriToObtain = lastBlockInfo.BlockIri;
            return GetBlockByIri(blockIriToObtain);
        }

        public Block GetBlock(String blockIndex)
        {
            String blockIri = _chainGraphIri + "/" + blockIndex;
            return GetBlockByIri(blockIri);
        }

        public Block GetBlockByIri(String blockIri)
        {
            return _repositoryManager.GetBlockByIri(blockIri);
        }

        public void ReplaceBlockchain(List<BlockHeader> blockHeaders, List<BlockContent> blockContents)
        {
            _logger.LogDebug("Attempting to replace the whole blockchain.");
            var blocksToPersist = new List<Block>();
            foreach (BlockHeader blockHeader in blockHeaders)
            {
                var blockIri = GenerateNewBlockIri(blockHeader.GetIndexAsInt());
                BlockContent blockContent = FindBlockContentByBlockHeader(blockContents, blockHeader);
                blocksToPersist.Add(new Block(blockIri, blockHeader, blockContent));
            }
            _logger.LogDebug("Ready to persist the following blocks: {}.", blocksToPersist);
            _repositoryManager.ClearBlockchainAndPersistBlocks(blocksToPersist);
        }

        private HashSet<Triple> GetTriplesFromSerializedModel(String serializedModel, RdfFormat rdfFormat)
        {
            VDS.RDF.IGraph deserializeModel = _rdf4jSerializationHandler.DeserializeGraph(serializedModel, rdfFormat);
            return _rdf4jMapper.GraphToTriples(deserializeModel);
        }

        private long GenerateNewIndex()
        {
            long newIndex = _lastIndex + 1;
            _logger.LogDebug("Generated new index: {0}", newIndex);
            return newIndex;
        }

        private string GenerateNewBlockIri(long newIndex)
        {
            return _chainGraphIri + "/" + newIndex;
        }

        private BlockContent FindBlockContentByBlockHeader(List<BlockContent> blockContents, BlockHeader blockHeader)
        {
            BlockContent blockContent = null;
            foreach (var currentBlockContent in blockContents)
            {
                if (currentBlockContent.DataGraphIri.Equals(blockHeader.DataGraphIri))
                {
                    blockContent = currentBlockContent;
                }
            }
            if (blockContent == null)
            {
                String msg = String.Format("Unable to find matching block content for the data graph IRI '{0}'.", blockHeader.DataGraphIri);
                throw new CreatingBlockException(msg);
            }
            return blockContent;
        }
    }
}
