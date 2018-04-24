using DryIoc;
using elephant.core.tests.service;
using elephant.core.tests.service.jsonld;
using elephant.core.tests.util;

namespace elephant.core.tests.app.jsonld
{
    public class JsonLdHashingApp
    {
        public static void Run()
        {
            DIConfig.DI.Unregister<IBenchmarkService>();
            DIConfig.DI.Register<IBenchmarkService, JsonLdHashingBenchmarkService>(serviceKey: typeof(JsonLdHashingBenchmarkService));
            var appRunner = new BenchmarkAppRunner();
            var benchmarkResults = appRunner.RunParticularBenchmarkWithinGivenContext(typeof(JsonLdHashingBenchmarkService));
            ResultPrinter.PrintResults(typeof(JsonLdHashingApp).Name, benchmarkResults);
        }
    }
}
