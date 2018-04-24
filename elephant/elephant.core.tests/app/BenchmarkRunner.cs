using System;
using System.IO;
using elephant.core.tests.exception;
using elephant.core.tests.model;
using elephant.core.tests.service;

namespace elephant.core.tests.app
{
    public class BenchmarkRunner
    {
        //private ILogger<BenchmarkRunner> _logger;
        private IBenchmarkService _benchmarkService;

        public BenchmarkRunner(IBenchmarkService benchmarkService)
        {
            _benchmarkService = benchmarkService;
        }

        public BenchmarkResult Run(int i)
        {
            try
            {
                BenchmarkResult result = _benchmarkService.Run(i);
                WriteResult(result);
                return result;
            }
            catch (BenchmarkException ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        private void WriteResult(BenchmarkResult br)
        {
            var file = "/data/lei/" + _benchmarkService.Name + ".txt";
            File.AppendAllText(file, br.ToString());
        }
    }
}
