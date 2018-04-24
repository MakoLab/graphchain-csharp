using System.Collections.Generic;
using elephant.core.model.rdf;
using elephant.core.model.rdf.impl;
using elephant.core.ramt.serialization;
using elephant.core.service.cryptography;
using elephant.core.util;
using elephant.core.vocabulary.bc;
using Microsoft.Extensions.Logging;

namespace elephant.core.service.hashing
{
    public class CircleDotHashingService : IHashingService
    {
        private ILogger<CircleDotHashingService> _logger;
        private IHashCalculator _hashCalculator;
        private NTriplesFormatHandler _nTriplesFormatHandler;

        public CircleDotHashingService(ILogger<CircleDotHashingService> logger, IHashCalculator hashCalculator)
        {
            _logger = logger;
            _hashCalculator = hashCalculator;

            _nTriplesFormatHandler = new NTriplesFormatHandler();
        }

        public string CalculateHash(HashSet<Triple> triples)
        {
            _logger.LogDebug("Calculating hash for {0} triples.", triples.Count);

            var finalDigest = new byte[_hashCalculator.HashSize / 8];

            foreach (Triple triple in triples)
            {
                string serializedTriple = _nTriplesFormatHandler.SerializeTriple(triple);
                byte[] hash = _hashCalculator.CalculateHashAsBytes(serializedTriple);
                
                _logger.LogTrace("T: {0} || H: {1}", serializedTriple, hash.ToHexString());
                finalDigest = ByteArrayUtils.AddHashes(finalDigest, hash);
            }

            return finalDigest.ToHexString().ToLower();
        }

        public HashSet<Triple> HandleTriplesBeforePersisting(HashSet<Triple> triples)
        {
            HashSet<Triple> resultTriples = new HashSet<Triple>(triples);
            foreach (Triple triple in triples)
            {
                ISubject subject = triple.Subject;
                if (subject.IsBlankNode())
                {
                    resultTriples.Add(
                        new Triple(
                            subject,
                            ElephantVocabulary.HasInternalLabel,
                            new Literal(subject.AsBlankNode().Identifier)));
                }
            }
            return resultTriples;
        }

        public HashSet<Triple> HandleTriplesBeforeHashing(HashSet<Triple> triples)
        {
            HashSet<Triple> result = new HashSet<Triple>();

            var actualIdsToInternalIds = new Dictionary<string, string>();

            foreach (Triple triple in triples)
            {
                if (triple.Predicate.Equals(ElephantVocabulary.HasInternalLabel))
                {
                    string actualId = triple.Subject.IsBlankNode() ? triple.Subject.AsBlankNode().Identifier : triple.Subject.AsIri().ValueAsString();
                    string internalId = triple.Object.AsLiteral().LexicalForm;
                    actualIdsToInternalIds.Add(actualId, internalId);
                }
            }

            _logger.LogDebug("actualIdsToInternalIds={0}", actualIdsToInternalIds);

            foreach (Triple triple in triples)
            {
                ISubject subject = triple.Subject;
                IObject object_ = triple.Object;
                if (triple.Predicate.Equals(ElephantVocabulary.HasInternalLabel))
                {
                    continue;
                }
                if (triple.Subject.IsIri() && triple.Subject.AsIri().ValueAsString().StartsWith("urn:"))
                {
                    subject = new BlankNode(actualIdsToInternalIds[subject.AsIri().ValueAsString()]);
                    result.Add(new Triple(subject, triple.Predicate, object_));
                    continue;
                }
                if (subject.IsBlankNode() || object_.IsBlankNode())
                {
                    if (subject.IsBlankNode())
                    {
                        subject = new BlankNode(actualIdsToInternalIds[subject.AsBlankNode().Identifier]);
                    }
                    if (object_.IsBlankNode())
                    {
                        object_ = new BlankNode(actualIdsToInternalIds[object_.AsBlankNode().Identifier]);
                    }

                    result.Add(new Triple(subject, triple.Predicate, object_));
                }
                else
                {
                    result.Add(triple);
                }
            }

            return result;
        }
    }
}
