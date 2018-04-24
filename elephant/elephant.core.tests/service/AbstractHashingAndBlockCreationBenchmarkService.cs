using System.Collections.Generic;
using elephant.core.exception;
using elephant.core.model.chain;
using elephant.core.service;
using elephant.core.tests.exception;
using elephant.core.vocabulary;
using Microsoft.Extensions.Logging;

namespace elephant.core.tests.service
{
    public abstract class AbstractHashingAndBlockCreationBenchmarkService
    {
        protected ILogger<AbstractHashingAndBlockCreationBenchmarkService> _logger;
        protected BlockService _blockService;
        protected string _repositoryUrl;

        public void RunBenchmark(Dictionary<string, string> filesContent)
        {
            foreach (var graphIriToFileContent in filesContent)
            {
                try
                {
                    Block block = _blockService.CreateBlock(
                        graphIriToFileContent.Key,
                        graphIriToFileContent.Value,
                        RdfFormat.TURTLE);
                }
                catch (CreatingBlockException ex)
                {
                    _logger.LogWarning(ex, "Exception was thrown while creating block.");
                    throw new BenchmarkException("Exception was thrown while creating block.", ex);
                }
            }
        }
    }
}
