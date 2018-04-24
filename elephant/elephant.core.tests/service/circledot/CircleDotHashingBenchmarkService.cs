﻿using System.Collections.Generic;
using System.Diagnostics;
using elephant.core.service.hashing;
using elephant.core.tests.model;
using elephant.core.tests.service.auxiliary;
using elephant.core.tests.util;
using Microsoft.Extensions.Logging;

namespace elephant.core.tests.service.circledot
{
    public class CircleDotHashingBenchmarkService : AbstractHashingBenchmarkService
    {
        private ILogger<CircleDotHashingBenchmarkService> _logger;
        private ReadFileService _readFileService;
        private string _rdfFilesPath;

        public CircleDotHashingBenchmarkService(ILogger<CircleDotHashingBenchmarkService> logger, ReadFileService readFileService, IHashingService hashingService)
        {
            _logger = logger;
            _readFileService = readFileService;
            _rdfFilesPath = Constants.RdfFilesPath;
            _hashingService = hashingService;
        }

        public override BenchmarkResult Run(int size)
        {
            _logger.LogDebug("Starting Elephant Benchmark App...");
            _logger.LogDebug("Config info:");
            _logger.LogDebug("\tsize={0}", size);
            Dictionary<string, string> filesContent = _readFileService.ReadFilesContent(_rdfFilesPath, size);
            _logger.LogDebug("Testing hashing speed for {0} graphs...", size);
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            RunBenchmark(filesContent);
            stopWatch.Stop();
            return new BenchmarkResult(size, stopWatch.ElapsedMilliseconds);
        }
    }
}
