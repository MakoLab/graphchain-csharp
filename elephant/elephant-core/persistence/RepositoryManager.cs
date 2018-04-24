using System;
using System.Collections.Generic;
using System.Linq;
using elephant.core.exception;
using elephant.core.model.chain;
using elephant.core.model.rdf;
using elephant.core.model.rdf.impl;
using elephant.core.persistence.repository;
using elephant.core.ramt;
using elephant.core.service.cryptography;
using elephant.core.service.hashing;
using elephant.core.util;
using elephant.core.vocabulary;
using elephant.core.vocabulary.bc;
using elephant.core.vocabulary.owl;
using elephant.core.vocabulary.rdf;
using elephant.core.vocabulary.xml;
using Microsoft.Extensions.Logging;

namespace elephant.core.persistence
{
    public class RepositoryManager
    {
        private ILogger<RepositoryManager> _logger;

        public static string DEFAULT_GENESIS_BLOCK_DATA_GRAPH_IRI = GCO.MAKOLAB_BC;
        private static string DEFAULT_GENESIS_BLOCK_PREVIOUS_HASH = "0";
        private static string DEFAULT_GENESIS_BLOCK_TIME_STAMP = "1502269780";

        private string _chainGraphIri;
        private FileContentObtainer _fileContentObtainer;

        private IHashCalculator _hashCalculator;
        private IHashingService _hashingService;
        private ITriplestoreRepository _repository;

        private DotNetRdfMapper _dotNetRdfMapper;
        private DotNetRdfSerializationHandler _dotNetRdfSerializationHandler;

        public RepositoryManager(ILogger<RepositoryManager> logger, FileContentObtainer fileContentObtainer, IHashCalculator hashCalculator, IHashingService hashingService, ITriplestoreRepository repository, StoreConfiguration sConfig)
        {
            _logger = logger;
            _fileContentObtainer = fileContentObtainer;
            _hashCalculator = hashCalculator;
            _hashingService = hashingService;
            _repository = repository;
            _chainGraphIri = sConfig.ChainGraphIri;

            _dotNetRdfMapper = new DotNetRdfMapper();
            _dotNetRdfSerializationHandler = new DotNetRdfSerializationHandler();
            Init();
        }

        public void Init()
        {
            _logger.LogDebug("[CONFIG] Initializing the repository manager...");

            if (_repository.IsReadyToUse())
            {
                _logger.LogDebug("[CONFIG] The repository is already set to use.");
            }
            else
            {
                _logger.LogDebug("[CONFIG] Creating a genesis Block...");
                try
                {
                    CreateGenesisBlock();
                }
                catch (CreatingBlockException ex)
                {
                    throw new Exception("Exception was thrown while creating genesis block.", ex);
                }
            }

            _logger.LogInformation("[CONFIG] The repository manager is configured correctly.");
        }

        public LastBlockInfo GetLastBlockInfo()
        {
            LastBlockInfo lastBlockInfo = _repository.GetLastBlockInfo();
            if (lastBlockInfo == null)
            {
                throw new ReadingBlockException("Unable to create last block info.");
            }
            _logger.LogDebug("Last block info: {0}", lastBlockInfo);
            return lastBlockInfo;
        }

        public Block GetBlockByIri(string blockIri)
        {
            try
            {
                HashSet<Triple> blockHeaderTriples = _repository.GetBlockHeaderByIri(blockIri);
                if (!blockHeaderTriples.Any())
                {
                    string msg = String.Format("Block with IRI '{0}' was not found.", blockIri);
                    throw new BlockNotFoundException(msg);
                }
                BlockHeader blockHeader = CreateBlockHeaderFromTriples(blockHeaderTriples);
                HashSet<Triple> blockContentTriples = _repository.GetGraphByIri(blockHeader.DataGraphIri);
                BlockContent blockContent = new BlockContent(blockHeader.DataGraphIri, blockContentTriples);

                return new Block(blockIri, blockHeader, blockContent);
            }
            catch (ObtainingBlockHeaderException ex)
            {
                throw new ReadingBlockException("Exception was thrown while creating block header from triples.", ex);
            }
        }

        public string MakeSparqlQuery(string sparqlQuery)
        {
            return _repository.MakeSparqlQueryAndReturnGraphAsJsonLdString(sparqlQuery);
        }

        public void PersistBlock(Block blockToPersist, bool shouldHandleTriplesBeforePersisting)
        {
            HashSet<Triple> blockHeaderTriplesToPersist = new HashSet<Triple>();

            BlockHeader blockHeader = blockToPersist.BlockHeader;
            BlockContent blockContent = blockToPersist.BlockContent;

            Iri blockIri = new Iri(blockToPersist.BlockIri);
            blockHeaderTriplesToPersist.Add(new Triple(
                blockIri,
                RDF.type,
                GCO.Block
            ));
            if (blockToPersist.IsGenesisBlock)
            {
                blockHeaderTriplesToPersist.Add(new Triple(
                    blockIri,
                    RDF.type,
                    GCO.GenesisBlock
                ));
            }
            blockHeaderTriplesToPersist.Add(new Triple(
                blockIri,
                RDF.type,
                OWL.NamedIndividual
            ));
            blockHeaderTriplesToPersist.Add(new Triple(
                blockIri,
                GCO.hasDataGraphIri,
                new Literal(blockHeader.DataGraphIri, XMLSchema.AnyURI)
            ));
            blockHeaderTriplesToPersist.Add(new Triple(
                blockIri,
                GCO.hasDataHash,
                new Literal(blockHeader.DataHash)
            ));
            blockHeaderTriplesToPersist.Add(new Triple(
                blockIri,
                GCO.hasHash,
                new Literal(blockHeader.Hash)
            ));
            blockHeaderTriplesToPersist.Add(new Triple(
                blockIri,
                GCO.hasIndex,
                new Literal(blockHeader.Index, XMLSchema.Decimal)
            ));
            blockHeaderTriplesToPersist.Add(new Triple(
                blockIri,
                GCO.hasPreviousBlock,
                new Iri(blockHeader.PreviousBlock)
            ));
            blockHeaderTriplesToPersist.Add(new Triple(
                blockIri,
                GCO.hasPreviousHash,
                new Literal(blockHeader.PreviousHash)
            ));
            blockHeaderTriplesToPersist.Add(new Triple(
                blockIri,
                GCO.hasTimeStamp,
                new Literal(blockHeader.Timestamp, XMLSchema.Decimal)
            ));
            HashSet<Triple> blockContentTriplesToPersist;
            if (shouldHandleTriplesBeforePersisting)
            {
                blockContentTriplesToPersist = _hashingService.HandleTriplesBeforePersisting(blockContent.Triples);
            }
            else
            {
                blockContentTriplesToPersist = blockContent.Triples;
            }
            // TODO: The following invocations should be in the same transaction.
            _repository.PersistTriples(_chainGraphIri, blockHeaderTriplesToPersist);
            _repository.PersistTriples(blockContent.DataGraphIri, blockContentTriplesToPersist);
        }

        internal void ClearRepository()
        {
            _repository.ClearRepository();
        }

        public void ClearBlockchainAndPersistBlocks(List<Block> blocksToPersist)
        {
            // TODO: This should be run in the same transaction.
            _repository.ClearRepository();

            foreach (Block block in blocksToPersist)
            {
                PersistBlock(block, false);
            }
            _logger.LogDebug("The repository has been cleared and repopulated with obtained blocks. New number of blocks: {0}", blocksToPersist.Count);
        }

        private void CreateGenesisBlock()
        {
            string dataGraphIri = DEFAULT_GENESIS_BLOCK_DATA_GRAPH_IRI;
            string newIndex = "0";
            string newBlockIri = _chainGraphIri + "/" + newIndex;

            var deserializedModel = _dotNetRdfSerializationHandler.DeserializeGraph(dataGraphIri, GetRawContentOfGcoOntology(), RdfFormat.TURTLE);
            var triples = _dotNetRdfMapper.GraphToTriples(deserializedModel);

            string previousBlock = newBlockIri;
            string previousHash = DEFAULT_GENESIS_BLOCK_PREVIOUS_HASH;
            string timestamp = DEFAULT_GENESIS_BLOCK_TIME_STAMP;

            string dataHash = _hashingService.CalculateHash(triples);
            string stringToCalculateHash = (newIndex + newBlockIri + previousHash + timestamp + dataGraphIri + dataHash).Trim();
            string hash = _hashCalculator.CalculateHash(stringToCalculateHash).ToLower();

            BlockHeader blockHeader = new BlockHeader(
              dataGraphIri,
              dataHash,
              hash,
              newIndex,
              previousBlock,
              previousHash,
              timestamp);
            BlockContent blockContent = new BlockContent(dataGraphIri, triples);
            Block genesisBlock = new Block(newBlockIri, blockHeader, blockContent, true);

            PersistBlock(genesisBlock, true);
        }

        private BlockHeader CreateBlockHeaderFromTriples(HashSet<Triple> triples)
        {
            _logger.LogTrace("Attempt to create block header from triples: {0}", triples);
            string dataGraphIri = null;
            string dataHash = null;
            string hash = null;
            string index = null;
            string previousBlock = null;
            string previousHash = null;
            string timeStamp = null;

            foreach (Triple triple in triples)
            {
                string predicate = triple.Predicate.ValueAsString();
                string value = triple.Object.ValueAsString();

                switch (predicate)
                {
                    case GCO.HAS_DATA_GRAPH_IRI:
                        dataGraphIri = value;
                        break;
                    case GCO.HAS_DATA_HASH:
                        dataHash = value;
                        break;
                    case GCO.HAS_HASH:
                        hash = value;
                        break;
                    case GCO.HAS_INDEX:
                        index = value;
                        break;
                    case GCO.HAS_PREVIOUS_BLOCK:
                        previousBlock = value;
                        break;
                    case GCO.HAS_PREVIOUS_HASH:
                        previousHash = value;
                        break;
                    case GCO.HAS_TIME_STAMP:
                        timeStamp = value;
                        break;
                    case RDF.TYPE:
                        // Omitting RDF#type predicate.
                        break;
                    default:
                        _logger.LogWarning("Unknown predicate in the triple: '{0}', '{1}', '{2}'",
                            triple.Subject,
                            predicate,
                            value);
                        break;
                }
            }

            if (previousHash != null && hash != null && timeStamp != null)
            {
                return new BlockHeader(dataGraphIri, dataHash, hash, index, previousBlock, previousHash, timeStamp);
            }
            else
            {
                string blockSummary =
                    String.Format(
                        "[previousHash = '{0}', hash = '{1}', timestamp = '{2}']",
                        previousHash, hash, timeStamp);
                throw new ObtainingBlockHeaderException("Incorrect info concerning Block:" + blockSummary);
            }
        }
        private string GetRawContentOfGcoOntology()
        {
            _logger.LogDebug("Getting GCO ontology content.");
            return _fileContentObtainer.GetFileContent("\\data\\ontologies\\ontobc.ttl");
        }
    }

}
