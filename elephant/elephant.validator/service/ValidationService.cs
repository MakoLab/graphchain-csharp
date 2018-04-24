using System.Collections.Generic;
using elephant.core.exception;
using elephant.core.model.chain;
using elephant.core.model.rdf;
using elephant.core.persistence;
using elephant.core.service.hashing;
using elephant.validator.model;
using Microsoft.Extensions.Logging;

namespace elephant.validator.service
{
    public class ValidationService
    {
        private ILogger<ValidationService> _logger;
        private IHashingService _hashingService;
        private RepositoryManager _repositoryManager;
        private BlockHashValidator _blockHashValidator;

        public ValidationService(ILogger<ValidationService> logger, IHashingService hashingService, RepositoryManager repositoryManager, BlockHashValidator blockHashValidator)
        {
            _logger = logger;
            _hashingService = hashingService;
            _repositoryManager = repositoryManager;
            _blockHashValidator = blockHashValidator;
        }

        public GraphChainValidationResult ValidateGraphChain()
        {
            bool isValid = false;
            var blocks = new List<BlockValidationResult>();

            try
            {
                LastBlockInfo lastBlockInfo = _repositoryManager.GetLastBlockInfo();
                _logger.LogDebug("Last block IRI: '{0}'", lastBlockInfo.BlockIri);

                bool getPreviousBlock = true;
                string blockIriToObtain = lastBlockInfo.BlockIri;

                while (getPreviousBlock)
                {
                    Block currentBlock = _repositoryManager.GetBlockByIri(blockIriToObtain);
                    HashSet<Triple> triples = _hashingService.HandleTriplesBeforeHashing(currentBlock.BlockContent.Triples);
                    string calculatedDataHash = _hashingService.CalculateHash(triples);

                    BlockValidationResult blockValidationResult = _blockHashValidator.ValidateBlock(currentBlock, calculatedDataHash);
                    if (blockValidationResult.IsValid)
                    {
                        _logger.LogInformation("Block '{0}' is valid.", currentBlock.BlockIri);
                    }
                    else
                    {
                        _logger.LogWarning("Block '{0}' is not valid. Details: {1}", currentBlock.BlockIri, blockValidationResult);
                        getPreviousBlock = false;
                    }
                    blocks.Add(blockValidationResult);

                    blockIriToObtain = currentBlock.BlockHeader.PreviousBlock;

                    if (IsGenesisBlock(currentBlock))
                    {
                        // for the first block IRI and its previous block IRI are the same,
                        // so we do not go further
                        _logger.LogDebug("Block '{0}' has '{1}' as the previous block; they are equal.", blockIriToObtain, currentBlock.BlockIri);
                        getPreviousBlock = false;
                        isValid = true;
                    }
                }
            }
            catch (ReadingBlockException ex)
            {
                _logger.LogError("Exception was thrown while normalizing rdf graph.", ex);
            }
            catch (CalculatingHashException ex)
            {
                _logger.LogError("Exception was thrown while normalizing rdf graph.", ex);
            }
            return new GraphChainValidationResult(isValid, blocks);
        }

        private bool IsGenesisBlock(Block block)
        {
            return block.BlockIri.Equals(block.BlockHeader.PreviousBlock);
        }
    }
}
