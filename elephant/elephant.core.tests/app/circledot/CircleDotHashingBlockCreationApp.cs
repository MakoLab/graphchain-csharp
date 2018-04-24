using DryIoc;
using elephant.core.tests.service;
using elephant.core.tests.service.circledot;
using elephant.core.tests.util;

namespace elephant.core.tests.app.circledot
{
    public class CircleDotHashingBlockCreationApp
    {
        public static void Run()
        {
            DIConfig.ConfigCircleDotHash();
            DIConfig.ConfigAllegroGraphTriplestore();
            DIConfig.DI.Unregister<IBenchmarkService>();
            DIConfig.DI.Register<IBenchmarkService, CircleDotHashingBlockCreationBenchmarkService>(serviceKey: typeof(CircleDotHashingBlockCreationBenchmarkService));
            var appRunner = new BenchmarkAppRunner();
            var benchmarkResults = appRunner.RunParticularBenchmarkWithinGivenContext(typeof(CircleDotHashingBlockCreationBenchmarkService));
            ResultPrinter.PrintResults(typeof(CircleDotHashingBlockCreationApp).Name, benchmarkResults);
        }
    }
}

