using System.Diagnostics;
using elephant.core.service;
using elephant.core.tests.model;
using elephant.core.tests.service.auxiliary;
using elephant.core.tests.util;
using Microsoft.Extensions.Logging;

namespace elephant.core.tests.service.circledot
{
    public class CircleDotHashingBlockCreationBenchmarkService : AbstractHashingAndBlockCreationBenchmarkService, IBenchmarkService
    {
        //private ILogger<CircleDotHashingBlockCreationBenchmarkService> _logger;
        private ReadFileService _readFileService;
        private bool _clearRepositoryAfterTest;
        private string _rdfFilesPath;

        public CircleDotHashingBlockCreationBenchmarkService(ILogger<CircleDotHashingBlockCreationBenchmarkService> logger, ReadFileService readFileService, BlockService blockService)
        {
            _logger = logger;
            _readFileService = readFileService;
            _repositoryUrl = Constants.RepositoryUrl;
            _clearRepositoryAfterTest = Constants.ClearRepositoryAfterTest;
            _rdfFilesPath = Constants.RdfFilesPath;
            _blockService = blockService;
        }

        public string Name { get { return GetType().Name; } }

        public BenchmarkResult Run(int size)
        {
            _logger.LogDebug("Running CircleDot Hashing Block Creation Benchmark Service...");
            _logger.LogDebug("Config info:");
            _logger.LogDebug("\trepository.repositoryUrl={0}", _repositoryUrl);
            _logger.LogDebug("\tbenchmark.clearRepositoryAfterTest={0}", _clearRepositoryAfterTest);
            _logger.LogDebug("\tbenchmark.rdfFilesPath={0}", _rdfFilesPath);
            _logger.LogDebug("\tsize={0}", size);

            var filesContent = _readFileService.ReadFilesContent(_rdfFilesPath, size);

            Stopwatch stopWatch = new Stopwatch();

            _logger.LogDebug("Testing {0} graphs...", filesContent.Count);
            stopWatch.Start();

            RunBenchmark(filesContent);

            stopWatch.Stop();

            if (_clearRepositoryAfterTest)
            {
                _blockService.ClearRepository();
            }

            return new BenchmarkResult(filesContent.Count, stopWatch.ElapsedMilliseconds);
        }
    }
}
