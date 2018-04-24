using System;
using DryIoc;
using elephant.core.tests.app;
using Microsoft.Extensions.Logging;
using Serilog;

namespace elephant.core.tests
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var sLogger = new LoggerConfiguration().WriteTo.File("log.txt").MinimumLevel.Debug().CreateLogger();
            var loggerfactory = DIConfig.DI.Resolve<ILoggerFactory>().AddConsole(LogLevel.Information).AddSerilog(sLogger);

            DIConfig.ConfigCircleDotHash();
            DIConfig.ConfigAllegroGraphTriplestore();

            var app = DIConfig.DI.Resolve<OntologyUploadApp>();
            app.Run();
            Console.WriteLine("OntologyUploaded");

            //Console.WriteLine("Start benchmark");
            //CircleDotHashing.Run();
            //JsonLdHashingApp.Run();
            //CircleDotHashingBlockCreationApp.Run();
            //JsonLdHashingBlockCreationApp.Run();
            //CircleDotBlockChainValidatorApp.Run();
        }
    }
}
