using DryIoc;
using elephant.core.tests.service;
using elephant.core.tests.util;

namespace elephant.core.tests.app.validation
{
    public class CircleDotBlockChainValidatorApp
    {
        public static void Run()
        {
            DIConfig.ConfigCircleDotHash();
            DIConfig.ConfigAllegroGraphTriplestore();
            DIConfig.ConfigValidator();
            DIConfig.DI.Unregister<IBenchmarkService>();
            DIConfig.DI.Register<IBenchmarkService, ValidateBlockChainBenchmarkService>(serviceKey: typeof(ValidateBlockChainBenchmarkService));
            var appRunner = new BenchmarkAppRunner();
            var benchmarkResults = appRunner.RunParticularBenchmarkWithinGivenContext(typeof(ValidateBlockChainBenchmarkService),
            10,
            10
            );
            ResultPrinter.PrintResults(typeof(CircleDotBlockChainValidatorApp).Name, benchmarkResults);
        }
    }
}
