using System.Collections.Generic;
using elephant.core.model.rdf;
using elephant.core.model.rdf.impl;
using elephant.core.ramt.serialization;
using elephant.core.service.cryptography;
using elephant.core.util;
using Microsoft.Extensions.Logging;

namespace elephant.core.service.hashing
{
    public class InterwovenHashingService : IHashingService
    {
        ILogger<InterwovenHashingService> _logger;
        private NTriplesFormatHandler _nTriplesFormatHandler;
        private IHashCalculator _hashCalculator;
        private Dictionary<string, List<Triple>> _triplesWithBlankNodeAsSubject;
        private Dictionary<string, List<Triple>> _triplesWithBlankNodeAsObject;

        public InterwovenHashingService(ILogger<InterwovenHashingService> logger, IHashCalculator hashCalculator)
        {
            _logger = logger;
            _hashCalculator = hashCalculator;
            _nTriplesFormatHandler = new NTriplesFormatHandler();
        }

        public string CalculateHash(HashSet<Triple> triples)
        {
            var finalDigest = new byte[_hashCalculator.HashSize / 8];
            _triplesWithBlankNodeAsSubject = new Dictionary<string, List<Triple>>();
            _triplesWithBlankNodeAsObject = new Dictionary<string, List<Triple>>();
            PopulateBlankNodeLists(triples);
            foreach (var t in triples)
            {
                var subject = t.Subject;
                var object_ = t.Object;
                if (subject.IsBlankNode() || t.Object.IsBlankNode())
                {
                    if (t.Subject.IsBlankNode())
                    {
                        subject = NormalizeBlankNode(_triplesWithBlankNodeAsObject, t.Subject.AsBlankNode(), "Magic_S");
                    }
                    if (t.Object.IsBlankNode())
                    {
                        object_ = NormalizeBlankNode(_triplesWithBlankNodeAsSubject, t.Object.AsBlankNode(), "Magic_O");
                    }
                    var nTriple = _nTriplesFormatHandler.SerializeTriple(new Triple(subject, t.Predicate, object_));
                    finalDigest = ByteArrayUtils.AddHashes(finalDigest, _hashCalculator.CalculateHashAsBytes(nTriple));
                }
                else
                {
                    var nTriple = _nTriplesFormatHandler.SerializeTriple(t);
                    finalDigest = ByteArrayUtils.AddHashes(finalDigest, _hashCalculator.CalculateHashAsBytes(nTriple));
                }
            }
            return finalDigest.ToHexString().ToLower();
        }

        public HashSet<Triple> HandleTriplesBeforeHashing(HashSet<Triple> triples)
        {
            return triples;
        }

        public HashSet<Triple> HandleTriplesBeforePersisting(HashSet<Triple> triples)
        {
            return triples;
        }

        private void PopulateBlankNodeLists(HashSet<Triple> triples)
        {
            foreach (var t in triples)
            {
                if (t.Subject.IsBlankNode())
                {
                    AddEntryToDictionaryList(_triplesWithBlankNodeAsSubject, t.Subject.ValueAsString(), t);
                }
                if (t.Object.IsBlankNode())
                {
                    AddEntryToDictionaryList(_triplesWithBlankNodeAsObject, t.Object.ValueAsString(), t);
                }
            }
        }

        private byte[] SimpleHash(Triple t)
        {
            ISubject subject;
            if (t.Subject.IsBlankNode())
            {
                subject = new BlankNode("Magic_S");
            }
            else
            {
                subject = new Iri(t.Subject.AsIri().ValueAsString());
            }
            IObject object_;
            if (t.Object.IsBlankNode())
            {
                object_ = new BlankNode("Magic_O");
            }
            else
            {
                object_ = new Iri(t.Object.AsIri().ValueAsString());
            }
            IPredicate predicate = new Iri(t.Predicate.AsIri().ValueAsString());
            var nTriple = _nTriplesFormatHandler.SerializeTriple(new Triple(subject, predicate, object_));
            return _hashCalculator.CalculateHashAsBytes(nTriple);
        }

        private BlankNode NormalizeBlankNode(Dictionary<string, List<Triple>> tripleList, BlankNode bNode, string emptyListName)
        {
            if (tripleList.ContainsKey(bNode.ValueAsString()))
            {
                var hashSum = new byte[_hashCalculator.HashSize / 8];
                foreach (var ot in tripleList[bNode.ValueAsString()])
                {
                    hashSum = ByteArrayUtils.AddHashes(hashSum, SimpleHash(ot));
                }
                return new BlankNode(hashSum.ToHexString());
            }
            else
            {
                return new BlankNode(emptyListName);
            }
        }

        private static void AddEntryToDictionaryList<T>(Dictionary<string, List<T>> dictionary, string key, T t)
        {
            if (dictionary.ContainsKey(key))
            {
                dictionary[key].Add(t);
            }
            else
            {
                dictionary.Add(key, new List<T>() { t });
            }
        }
    }
}
