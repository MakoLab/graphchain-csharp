using System;
using System.Collections;
using System.Collections.Generic;
using elephant.core.model;
using elephant.core.model.rdf;
using elephant.core.model.rdf.impl;
using elephant.core.ramt.serialization;
using elephant.core.service.cryptography;
using elephant.core.vocabulary;

namespace elephant.core.service.normalization
{
    class RDFDatasetNormalizationService : INormalizationService
    {
        private IHashCalculator _hashCalculator;
        private NTriplesFormatHandler _nTriplesFormatHandler;

        public RDFDatasetNormalizationService(IHashCalculator hashCalculator, NTriplesFormatHandler nTriplesFormatHandler)
        {
            _hashCalculator = hashCalculator;
            _nTriplesFormatHandler = nTriplesFormatHandler;
        }

        public NormalizedRdf NormalizeRdf(HashSet<Triple> triples)
        {
            var nState = new NormalizationState();
            var normalizedTriples = new HashSet<Triple>();
            List<string> hashList;

            foreach (var t in triples) //4.4.2 2)
            {
                if (t.Subject.IsBlankNode())
                {
                    AddEntryToDictionaryList(nState.BNodesToQuads, t.Subject.AsBlankNode().ValueAsString(), t);
                }
                if (t.Object.IsBlankNode())
                {
                    AddEntryToDictionaryList(nState.BNodesToQuads, t.Object.AsBlankNode().ValueAsString(), t);
                }
            }
            var nonNormalizedNodes = new List<string>(nState.BNodesToQuads.Keys); //4.4.2 3)
            var simple = true; //4.4.2 4)
            while (simple)
            {
                simple = false; //4.4.2 5.1)
                nState.HashToBNodes.Clear(); //4.4.2 5.2)
                foreach (var bN in nonNormalizedNodes) //4.4.2 5.3)
                {
                    var hash = HashFirstDegree(bN, nState.BNodesToQuads); //4.4.2 5.3.1) --> 4.6.2
                    AddEntryToDictionaryList(nState.HashToBNodes, hash, bN); //4.4.2 5.3.2)
                }
                hashList = new List<string>(nState.HashToBNodes.Keys);
                hashList.Sort();
                foreach (var h in hashList) //4.4.2 5.4)
                {
                    if (nState.HashToBNodes[h].Count <= 1) //4.4.2 5.4.1)
                    {
                        nState.CanonicalIssuer.IssueIdentifier(nState.HashToBNodes[h][0]); //4.4.2 5.4.2)
                        nonNormalizedNodes.Remove(nState.HashToBNodes[h][0]); //4.4.2 5.3)
                        nState.HashToBNodes.Remove(h); //4.4.2 5.4.4)
                        simple = true; //4.4.2 5.4.5)
                    }
                }
            }
            hashList = new List<string>(nState.HashToBNodes.Keys);
            hashList.Sort();
            foreach (var h in hashList)
            {
                var hashPathList = new List<(string, IdentifierIssuer)>();
                foreach (var identifier in nState.HashToBNodes[h])
                {
                    if (nState.CanonicalIssuer.IsIssued(identifier)) //4.4.2 6.2.1)
                    {
                        continue;
                    }
                    var tIssuer = new IdentifierIssuer("_:b"); //4.4.2 6.2.2)
                    tIssuer.IssueIdentifier(identifier); //4.4.2 6.2.3)
                    hashPathList.Add(HashNDegree(identifier, nState, tIssuer));
                }
                hashPathList.Sort(delegate((string, IdentifierIssuer) x, (string, IdentifierIssuer) y) { return String.Compare(x.Item1, y.Item1); });
                foreach (var result in hashPathList) //4.4.2 6.3)
                {
                    foreach (var existingIdentifier in result.Item2.IssuedIdentifiers.Keys) //4.4.2 6.3.1)
                    {
                        nState.CanonicalIssuer.IssueIdentifier(existingIdentifier);
                    }
                }
            }
            foreach (var t in triples)
            {
                normalizedTriples.Add(ReplaceBlankNodes(t, nState.CanonicalIssuer));
            }

            return new NormalizedRdf(new RdfDataset(normalizedTriples), null, RdfFormat.N_TRIPLES);
        }
        
        private string HashFirstDegree(string refNode, Dictionary<string, List<Triple>> bNodesToQuads)
        {
            var nquads = new List<string>(); //4.6.2 1)
            foreach (var t in bNodesToQuads[refNode]) //4.6.2 2)
            {
                nquads.Add(SerializeTriple(t, refNode)); //4.6.2 3)
            }
            nquads.Sort(); //4.6.2 4)
            return _hashCalculator.CalculateHash(String.Concat(nquads)); //4.6.2 5)
        }

        private (string, IdentifierIssuer) HashNDegree(string identifier, NormalizationState nState, IdentifierIssuer issuer)
        {
            var hashToRelatedBNodes = new Dictionary<string, List<string>>(); //4.8.2 1)
            var quads = nState.BNodesToQuads[identifier]; //4.8.2 2)
            foreach (var q in quads) //4.8.2 3)
            {
                if (q.Subject.IsBlankNode() && q.Subject.ValueAsString() != identifier)
                {
                    var hash = HashRelatedBlankNode(q.Subject.ValueAsString(), nState, q, issuer, "s");
                    AddEntryToDictionaryList(hashToRelatedBNodes, hash, q.Subject.ValueAsString());
                }
                if (q.Object.IsBlankNode() && q.Object.ValueAsString() != identifier)
                {
                    var hash = HashRelatedBlankNode(q.Object.ValueAsString(), nState, q, issuer, "o");
                    AddEntryToDictionaryList(hashToRelatedBNodes, hash, q.Object.ValueAsString());
                }
            }
            var dataToHash = "";
            var hashList = new List<string>(hashToRelatedBNodes.Keys);
            hashList.Sort();
            foreach (var hash in hashList)
            {
                dataToHash += hash;
                var chosenPath = "";
                IdentifierIssuer chosenIssuer = null;
                var bNodesCopy = new List<string>(hashToRelatedBNodes[hash]);
                var permutations = Permutate(bNodesCopy, bNodesCopy.Count);
                foreach (var p in permutations) //4.8.2 5.4)
                {
                    var issuerCopy = issuer.GetCopy();
                    var path = "";
                    var recursionList = new List<string>();
                    foreach (var elem in p) //4.8.2 5.4.4)
                    {
                        var related = elem as string;
                        if (nState.CanonicalIssuer.IsIssued(related))
                        {
                            path += nState.CanonicalIssuer.GetIssuedIdentifier(related);
                        }
                        else
                        {
                            if (!issuerCopy.IsIssued(related))
                            {
                                recursionList.Add(related);
                            }
                            path += issuerCopy.IssueIdentifier(related);
                        }
                        if (!String.IsNullOrEmpty(chosenPath) && path.Length >= chosenPath.Length && String.Compare(path, chosenPath, true) > 0)
                        {
                            break; //4.8.2 5.4.4.3)
                        }
                    }
                    if (!String.IsNullOrEmpty(chosenPath) && path.Length >= chosenPath.Length && String.Compare(path, chosenPath, true) > 0)
                    {
                        continue; //4.8.2 5.4.4.3)
                    }
                    foreach (var r in recursionList)
                    {
                        var result = HashNDegree(r, nState, issuerCopy);
                        path += issuerCopy.IssueIdentifier(r);
                        path = path + "<" + result.Item1 + ">";
                        issuerCopy = result.Item2;
                        if (!String.IsNullOrEmpty(chosenPath) && path.Length >= chosenPath.Length && String.Compare(path, chosenPath, true) > 0)
                        {
                            break; //4.8.2 5.4.5.5)
                        }
                    }
                    if (!String.IsNullOrEmpty(chosenPath) && path.Length >= chosenPath.Length && String.Compare(path, chosenPath, true) > 0)
                    {
                        continue; //4.8.2 5.4.5.5)
                    }
                    if (String.IsNullOrEmpty(chosenPath) || String.Compare(path, chosenPath, true) < 0)
                    {
                        chosenPath = path;
                        chosenIssuer = issuerCopy;
                    }
                }
                dataToHash += chosenPath;
                if (chosenIssuer != null)
                {
                    issuer = chosenIssuer;
                }
                
            }
            return (_hashCalculator.CalculateHash(dataToHash), issuer);
        }

        private string HashRelatedBlankNode(string related, NormalizationState nState, Triple quad, IdentifierIssuer issuer, string position)
        {
            var identifier = nState.CanonicalIssuer.GetIssuedIdentifier(related) ?? issuer.GetIssuedIdentifier(related) ?? HashFirstDegree(related, nState.BNodesToQuads); //4.7.2 1)
            var input = position + "<" + quad.Predicate.ValueAsString() + ">" + identifier; //4.7.2 2,3,4)
            return _hashCalculator.CalculateHash(input); //4.7.2 5)
        }

        private string SerializeTriple(Triple t, string refNode)
        {
            ISubject subject;
            if (t.Subject.IsBlankNode())
            {
                if (t.Subject.AsBlankNode().ValueAsString().Equals(refNode))
                {
                    subject = new BlankNode("a");
                }
                else
                {
                    subject = new BlankNode("z");
                }
            }
            else
            {
                subject = new Iri(t.Subject.AsIri().ValueAsString());
            }
            IObject object_;
            if (t.Object.IsBlankNode())
            {
                if (t.Object.AsBlankNode().ValueAsString().Equals(refNode))
                {
                    object_ = new BlankNode("a");
                }
                else
                {
                    object_ = new BlankNode("z");
                }
            }
            else
            {
                object_ = new Iri(t.Object.AsIri().ValueAsString());
            }
            IPredicate predicate = new Iri(t.Predicate.AsIri().ValueAsString());
            return _nTriplesFormatHandler.SerializeTriple(new Triple(subject, predicate, object_));
        }

        private Triple ReplaceBlankNodes(Triple t, IdentifierIssuer canonicalIssuer)
        {
            ISubject subject = t.Subject;
            if (subject.IsBlankNode())
            {
                subject = new BlankNode(canonicalIssuer.GetIssuedIdentifier(subject.AsBlankNode().ValueAsString()));
            }
            IObject @object = t.Object;
            if (@object.IsBlankNode())
            {
                @object = new BlankNode(canonicalIssuer.GetIssuedIdentifier(@object.AsBlankNode().ValueAsString()));
            }
            return new Triple(subject, t.Predicate, @object);
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

        public static void RotateRight(IList sequence, int count)
        {
            object tmp = sequence[count - 1];
            sequence.RemoveAt(count - 1);
            sequence.Insert(0, tmp);
        }

        public static IEnumerable<IList> Permutate(IList sequence, int count)
        {
            if (count == 1) yield return sequence;
            else
            {
                for (int i = 0; i < count; i++)
                {
                    foreach (var perm in Permutate(sequence, count - 1))
                        yield return perm;
                    RotateRight(sequence, count);
                }
            }
        }
    }
}
