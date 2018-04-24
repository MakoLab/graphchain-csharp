using DryIoc;
using elephant.core.persistence;
using elephant.core.persistence.repository;
using elephant.core.persistence.repository.impl;
using elephant.core.service;
using elephant.core.service.cryptography;
using elephant.core.service.hashing;
using elephant.core.service.normalization;
using elephant.core.tests.app;
using elephant.core.tests.service.auxiliary;
using elephant.core.tests.util;
using elephant.core.util;
using elephant.validator.service;
using Microsoft.Extensions.Logging;

namespace elephant.core.tests
{
    public class DIConfig
    {
        public static Container DI = new Container();

        static DIConfig()
        {
            DI.Register<ILoggerFactory, LoggerFactory>(reuse: Reuse.Singleton, made: Made.Of(() => new LoggerFactory()));
            DI.Register(typeof(ILogger<>), typeof(Logger<>), reuse: Reuse.Singleton);
            DI.Register(made: Made.Of(() => ConfigurationFactory.CreateConfigurationRoot()));
            DI.Register<ReadFileService>(reuse: Reuse.Singleton);
            DI.Register<BlockService>();
            DI.Register<IHashCalculator, Sha256HashCalculator>(reuse: Reuse.Singleton);
            DI.Register<RepositoryManager>();
            DI.Register<QueryTemplatesService>(reuse: Reuse.Singleton);
            DI.Register<FileContentObtainer>(reuse: Reuse.Singleton);
            DI.Register<StoreConfiguration>();
            DI.Register<OntologyUploadApp>();
        }

        public static void ConfigCircleDotHash()
        {
            DI.Unregister<IHashingService>();
            DI.Register<IHashingService, CircleDotHashingService>(reuse: Reuse.Singleton);
        }

        public static void ConfigJsonLd()
        {
            DI.Unregister<IHashingService>();
            DI.Register<IHashingService, JsonLdHashingService>(reuse: Reuse.Singleton);
            DI.Unregister<INormalizationService>();
            DI.Register<INormalizationService, JsonLdNormalizationService>(reuse: Reuse.Singleton);
        }

        public static void ConfigDotNetRdfTriplestore()
        {
            DI.Unregister<ITriplestoreRepository>();
            DI.Register<ITriplestoreRepository, DotNetRdfRepository>();
        }

        public static void ConfigAllegroGraphTriplestore()
        {
            DI.Register<AllegroGraphConnectionData>();
            DI.Unregister<ITriplestoreRepository>();
            DI.Register<ITriplestoreRepository, AllegroGraphTripleStoreRepository>();
        }

        public static void ConfigStardogTriplestore()
        {
            DI.Register<StardogConnectionData>();
            DI.Unregister<ITriplestoreRepository>();
            DI.Register<ITriplestoreRepository, StardogTripleStoreRepository>();
        }

        internal static void ConfigValidator()
        {
            DI.Register<ValidationService>();
            DI.Register<BlockHashValidator>();
        }
    }
}
