using elephant.core.exception;
using elephant.core.model.chain;
using elephant.core.service;
using elephant.core.tests.exception;
using elephant.core.tests.service.auxiliary;
using elephant.core.vocabulary;
using Microsoft.Extensions.Logging;

namespace elephant.core.tests.service.populator
{
    public class RepositoryPopulatorService
    {
        private ILogger<RepositoryPopulatorService> _logger;
        private ReadFileService _readFileService;
        private BlockService _blockService;
        private string _repositoryUrl;
        private string _rdfFilesPath;

        public RepositoryPopulatorService(ILogger<RepositoryPopulatorService> logger, ReadFileService readFileService, BlockService blockService, string repositoryUrl, string rdfFilesPath)
        {
            _logger = logger;
            _readFileService = readFileService;
            _blockService = blockService;
            _repositoryUrl = repositoryUrl;
            _rdfFilesPath = rdfFilesPath;
        }

        public void Run(int size)
        {
            _logger.LogDebug("Running {0}...", GetType().Name);
            _logger.LogDebug("Config info:");
            _logger.LogDebug("\trepository.repositoryUrl={0}", _repositoryUrl);
            _logger.LogDebug("\tbenchmark.rdfFilesPath={0}", _rdfFilesPath);
            _logger.LogDebug("\tsize={0}", size);

            var filesContent = _readFileService.ReadFilesContent(_rdfFilesPath, size);

            foreach (var fileContent in filesContent)
            {
                try
                {
                    Block block = _blockService.CreateBlock(
                        fileContent.Key,
                        fileContent.Value,
                        RdfFormat.TURTLE);
                }
                catch (CreatingBlockException ex)
                {
                    throw new BenchmarkException("Exception was thrown while creating block.", ex);
                }
            }

            _logger.LogDebug("Finishing running {}...", GetType().Name);
        }
    }
}
