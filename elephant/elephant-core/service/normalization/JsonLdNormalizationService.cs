using System.Collections.Generic;
using System.IO;
using elephant.core.exception;
using elephant.core.model;
using elephant.core.model.rdf;
using elephant.core.ramt;
using elephant.core.vocabulary;
using JsonLD.Core;
using JsonLD.Util;
using Microsoft.Extensions.Logging;

namespace elephant.core.service.normalization
{
    public class JsonLdNormalizationService : INormalizationService
    {
        private ILogger<JsonLdNormalizationService> _logger;

        private DotNetRdfMapper _rdf4jMapper;

        public JsonLdNormalizationService(ILogger<JsonLdNormalizationService> logger)
        {
            _logger = logger;
            _rdf4jMapper = new DotNetRdfMapper();
        }

        public NormalizedRdf NormalizeRdf(HashSet<Triple> triples)
        {
            var model = _rdf4jMapper.TriplesToGraph(triples);
            string modelAsJsonLd = _rdf4jMapper.GraphToSerialization(model, new VDS.RDF.Writing.JsonLdWriter());
            //fix for JsonLdWriter bug
            modelAsJsonLd = modelAsJsonLd.Replace("@language\": \"http:", "@type\": \"http:");
            try
            {
                var jsonObject = JSONUtils.FromString(modelAsJsonLd);
                var normalizedJsonObject = JsonLdProcessor.Normalize(jsonObject, PrepareJsonLdOptions());

                return new NormalizedRdf(new RdfDataset(triples), normalizedJsonObject.ToString(), RdfFormat.JSON_LD);
            }
            catch (IOException ex)
            {
                throw new NormalizingRdfException("Exception was thrown while normalizing JSON object.", ex);
            }
            catch (JsonLdError ex)
            {
                throw new NormalizingRdfException("Exception was thrown while normalizing JSON object.", ex);
            }
        }

        private JsonLdOptions PrepareJsonLdOptions()
        {
            JsonLdOptions options = new JsonLdOptions
            {
                format = "application/nquads"
            };
            return options;
        }
    }
}
