using DryIoc;
using elephant.core.persistence;
using elephant.core.persistence.repository;
using elephant.core.persistence.repository.impl;
using elephant.core.service;
using elephant.core.service.base64;
using elephant.core.service.cryptography;
using elephant.core.service.hashing;
using elephant.core.util;
using elephant.validator.service;
using elephant.web.Services.p2p;
using elephant.web.WebSockets;
using Microsoft.Extensions.Logging;

namespace elephant.web
{
    public class DIConfig
    {
        public DIConfig(IRegistrator c)
        {
            c.Register<ILoggerFactory, LoggerFactory>(reuse: Reuse.Singleton, made: Made.Of(() => new LoggerFactory()));
            c.Register(typeof(ILogger<>), typeof(Logger<>), reuse: Reuse.Singleton);
            c.Register(made: Made.Of(() => ConfigurationFactory.CreateConfigurationRoot()));

            //c.Register<ReadFileService>(reuse: Reuse.Singleton);
            c.Register<BlockService>(reuse: Reuse.Singleton);
            c.Register<BlockContentService>(reuse: Reuse.Singleton);
            c.Register<Base64Handler>(reuse: Reuse.Singleton);
            c.Register<IHashCalculator, Sha256HashCalculator>(reuse: Reuse.Singleton);
            c.Register<RepositoryManager>(reuse: Reuse.Singleton);
            c.Register<QueryTemplatesService>(reuse: Reuse.Singleton);
            c.Register<FileContentObtainer>(reuse: Reuse.Singleton);
            c.Register<StoreConfiguration>(reuse: Reuse.Singleton);

            c.Register<WebSocketsManager>(reuse: Reuse.Singleton);
            c.Register<WebSocketsHandler>(reuse: Reuse.Singleton);
            c.Register<WebSocketsMiddleware>(reuse: Reuse.Singleton);
            c.Register<PeerToPeerService>(reuse: Reuse.Singleton);
            c.Register<PeerToPeerMessageHandler>(reuse: Reuse.Singleton);
            c.Register<BlockContentService>(reuse: Reuse.Singleton);

            c.Register<IHashingService, CircleDotHashingService>(reuse: Reuse.Singleton);
            c.Register<ITriplestoreRepository, DotNetRdfRepository>(reuse: Reuse.Singleton);

            c.Register<ValidationService>();
            c.Register<BlockHashValidator>();
        }

        //public static void ConfigCircleDotHash()
        //{
        //    DI.Unregister<IHashingService>();
        //    DI.Register<IHashingService, CircleDotHashingService>(reuse: Reuse.Singleton);
        //}

        //public static void ConfigJsonLd()
        //{
        //    DI.Unregister<IHashingService>();
        //    DI.Register<IHashingService, JsonLdHashingService>(reuse: Reuse.Singleton);
        //    DI.Unregister<INormalizationService>();
        //    DI.Register<INormalizationService, JsonLdNormalizationService>(reuse: Reuse.Singleton);
        //}

        //public static void ConfigDotNetRdfTriplestore()
        //{
        //    DI.Unregister<ITriplestoreRepository>();
        //    DI.Register<ITriplestoreRepository, DotNetRdfRepository>();
        //}

        //public static void ConfigAllegroGraphTriplestore()
        //{
        //    DI.Register<AllegroGraphConnectionData>();
        //    DI.Unregister<ITriplestoreRepository>();
        //    DI.Register<ITriplestoreRepository, AllegroGraphTripleStoreRepository>();
        //}

        //public static void ConfigStardogTriplestore()
        //{
        //    DI.Register<StardogConnectionData>();
        //    DI.Unregister<ITriplestoreRepository>();
        //    DI.Register<ITriplestoreRepository, StardogTripleStoreRepository>();
        //}

        //internal static void ConfigValidator()
        //{
        //    DI.Register<ValidationService>();
        //    DI.Register<BlockHashValidator>();
        //}
    }
}
