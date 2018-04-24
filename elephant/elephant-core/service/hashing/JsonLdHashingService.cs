using System.Collections.Generic;
using elephant.core.exception;
using elephant.core.model;
using elephant.core.model.rdf;
using elephant.core.service.cryptography;
using elephant.core.service.normalization;
using Microsoft.Extensions.Logging;

namespace elephant.core.service.hashing
{
    public class JsonLdHashingService : IHashingService
    {
        private ILogger<JsonLdHashingService> _logger;

        private IHashCalculator _hashCalculator;

        private INormalizationService _jsonLdNormalizationService;

        public JsonLdHashingService(ILogger<JsonLdHashingService> logger, IHashCalculator hashCalculator, INormalizationService jsonLdNormalizationService)
        {
            _logger = logger;
            _hashCalculator = hashCalculator;
            _jsonLdNormalizationService = jsonLdNormalizationService;
        }

        public string CalculateHash(HashSet<Triple> triples)
        {
            _logger.LogDebug("Calculating hash for {0} triples.", triples.Count);
            try
            {
                NormalizedRdf normalizedRdf = _jsonLdNormalizationService.NormalizeRdf(triples);
                return _hashCalculator.CalculateHash(normalizedRdf.SerializedNormalizedObject);
            }
            catch (NormalizingRdfException ex)
            {
                throw new CalculatingHashException("Exception was thrown while calculating hash for triple.", ex);
            }
        }

        public HashSet<Triple> HandleTriplesBeforeHashing(HashSet<Triple> triples)
        {
            return triples;
        }

        public HashSet<Triple> HandleTriplesBeforePersisting(HashSet<Triple> triples)
        {
            // Nothing to do here.
            return triples;
        }
    }
}
