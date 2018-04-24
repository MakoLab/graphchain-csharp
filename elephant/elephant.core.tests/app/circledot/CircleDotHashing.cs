using DryIoc;
using elephant.core.tests.service;
using elephant.core.tests.service.circledot;
using elephant.core.tests.util;

namespace elephant.core.tests.app.circledot
{
    public class CircleDotHashing
    {
        public static void Run()
        {
            DIConfig.DI.Unregister<IBenchmarkService>();
            DIConfig.DI.Register<IBenchmarkService, CircleDotHashingBenchmarkService>(serviceKey: typeof(CircleDotHashingBenchmarkService));
            var appRunner = new BenchmarkAppRunner();
            var benchmarkResults = appRunner.RunParticularBenchmarkWithinGivenContext(typeof(CircleDotHashingBenchmarkService));
            ResultPrinter.PrintResults(typeof(CircleDotHashing).Name, benchmarkResults);
        }
    }
}
