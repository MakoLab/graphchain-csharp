using System;
using System.Collections.Generic;
using elephant.core.exception;
using elephant.core.model.rdf;
using elephant.core.ramt;
using elephant.core.service.hashing;
using elephant.core.tests.exception;
using elephant.core.tests.model;
using elephant.core.vocabulary;

namespace elephant.core.tests.service
{
    public abstract class AbstractHashingBenchmarkService : IBenchmarkService
    {
        protected IHashingService _hashingService;
        private DotNetRdfMapper rdf4jMapper;
        private DotNetRdfSerializationHandler rdf4jSerializationHandler;

        public AbstractHashingBenchmarkService()
        {
            rdf4jMapper = new DotNetRdfMapper();
            rdf4jSerializationHandler = new DotNetRdfSerializationHandler();
        }

        public string Name
        {
            get { return GetType().Name; }
        }

        public virtual BenchmarkResult Run(int size)
        {
            throw new NotImplementedException();
        }

        public void RunBenchmark(Dictionary<string, string> filesContent)
        {
            foreach (var fileContentEntry in filesContent)
            {
                try
                {
                    VDS.RDF.IGraph deserializedModel = rdf4jSerializationHandler.DeserializeGraph(
                        fileContentEntry.Key,
                        fileContentEntry.Value,
                        RdfFormat.TURTLE);
                    HashSet<Triple> triples = rdf4jMapper.GraphToTriples(deserializedModel);

                    _hashingService.CalculateHash(triples);
                }
                catch (RdfSerializationException ex)
                {
                    throw new BenchmarkException("Exception: ", ex);
                }
                catch (CalculatingHashException ex)
                {
                    throw new BenchmarkException("Exception: ", ex);
                }
            }
        }
    }
}
