using System.Diagnostics;
using elephant.core.tests.model;
using elephant.validator.model;
using elephant.validator.service;
using Microsoft.Extensions.Logging;

namespace elephant.core.tests.service
{
    public class ValidateBlockChainBenchmarkService : IBenchmarkService
    {
        private ILogger<ValidateBlockChainBenchmarkService> _logger;
        private ValidationService _validationService;

        public string Name { get { return GetType().Name; } }

        public ValidateBlockChainBenchmarkService(ILogger<ValidateBlockChainBenchmarkService> logger, ValidationService validationService)
        {
            _logger = logger;
            _validationService = validationService;
        }

        public BenchmarkResult Run(int size)
        {
            _logger.LogDebug("Starting Elephant Benchmark App...");
            _logger.LogDebug("Config info:");
            _logger.LogDebug("\tsize={0}", size);

            _logger.LogDebug("Testing block chain verification for {0} blocks...", size);
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            GraphChainValidationResult graphChainValidationResult = _validationService.ValidateGraphChain();

            stopWatch.Stop();

            return new BenchmarkResult(size, stopWatch.ElapsedMilliseconds);
        }
    }
}
