using DryIoc;
using elephant.core.tests.service;
using elephant.core.tests.service.jsonld;
using elephant.core.tests.util;

namespace elephant.core.tests.app.jsonld
{
    public class JsonLdHashingBlockCreationApp
    {
        public static void Run()
        {
            DIConfig.ConfigJsonLd();
            DIConfig.DI.Unregister<IBenchmarkService>();
            DIConfig.DI.Register<IBenchmarkService, JsonLdHashingBlockCreationBenchmarkService>(serviceKey: typeof(JsonLdHashingBlockCreationBenchmarkService));
            var appRunner = new BenchmarkAppRunner();
            var benchmarkResults = appRunner.RunParticularBenchmarkWithinGivenContext(typeof(JsonLdHashingBlockCreationBenchmarkService));
            ResultPrinter.PrintResults(typeof(JsonLdHashingApp).Name, benchmarkResults);
        }
    }
}
