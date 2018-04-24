using System;
using System.Collections.Generic;
using DryIoc;
using elephant.core.tests.exception;
using elephant.core.tests.model;
using elephant.core.tests.service;

namespace elephant.core.tests.app
{
    public class BenchmarkAppRunner
    {
        private const int NUMBER_OF_RUNS = 1;

        public List<BenchmarkResult> RunParticularBenchmarkWithinGivenContext(Type benchmarkServiceClass)
        {
            return RunParticularBenchmarkWithinGivenContext(
                benchmarkServiceClass,
                10,
                10
            );
        }

        public List<BenchmarkResult> RunParticularBenchmarkWithinGivenContext(Type benchmarkServiceClass, int from, int to)
        {
            List<BenchmarkResult> results = new List<BenchmarkResult>();
            for (int i = from; i <= to; i *= 10)
            {
                long size = i;
                long timeSum = 0;

                for (int j = 0; j < NUMBER_OF_RUNS; j++)
                {
                    try
                    {
                        IBenchmarkService benchmarkService = DIConfig.DI.Resolve<IBenchmarkService>(serviceKey: benchmarkServiceClass);
                        BenchmarkRunner benchmarkRunner = new BenchmarkRunner(benchmarkService);
                        BenchmarkResult benchmarkResult = benchmarkRunner.Run(i);
                        timeSum += benchmarkResult.TimeInMillis;
                    }
                    catch (ContainerException ex)
                    {
                        throw new BenchmarkAppRunnerException("Problem with instatiation.", ex);
                    }
                }

                results.Add(new BenchmarkResult(size, timeSum / NUMBER_OF_RUNS));
            }

            return results;
        }
    }
}
