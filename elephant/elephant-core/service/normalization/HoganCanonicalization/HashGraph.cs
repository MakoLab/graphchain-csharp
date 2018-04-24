using System.Collections.Generic;
using System.Text;
using elephant.core.model.rdf;
using elephant.core.model.rdf.impl;
using elephant.core.service.cryptography;

namespace elephant.core.service.normalization.HoganCanonicalization
{
    public class HashGraph
    {
        // the hash function used for the graph
        private static IHashCalculator _hf;

        // a map from original IRIs and literals to hashed versions
        private Dictionary<Node, string> _staticHashes;

        // a map from original blank nodes to dynamic hashes
        private Dictionary<Node, string> _dynamicHashes;

        // the hashed triples
        private List<Triple> _data;

        // the initial blank hash
        private string _blankHash;

        /**
         * Create a blank HashGraph
         * @param hf The hashing function to be used
         */
        public HashGraph(IHashCalculator hf) : this(hf, new List<Triple>(), new Dictionary<Node, string>(), hf.CalculateHash(""), new Dictionary<Node, string>())
        { }

        // used mainly for the branch copy
        private HashGraph(IHashCalculator hf, List<Triple> data, Dictionary<Node, string> staticHashes, string blankHash, Dictionary<Node, string> dynamicHashes)
        {
            _hf = hf;
            _staticHashes = staticHashes;
            _blankHash = blankHash;
            _data = data;
            _dynamicHashes = dynamicHashes;
        }

        /**
         * The initial hash used for blank nodes ... the hash of 
         * the empty string
         * @return
         */
        public string GetBlankHash()
        {
            return _blankHash;
        }

        /**
         * Adds a triple to the graph.
         * Be aware that it doesn't check for duplicates.
         * That's your job.
         * 
         * @param triple
         */
        public void AddTriple(Triple stmt)
        {
            var subject = stmt.Subject;
            var predicate = stmt.Predicate;
            var @object = stmt.Object;
            GetOrCreatestring(subject as Node);
            GetOrCreatestring(predicate as Node);
            GetOrCreatestring(@object as Node);
            _data.Add(new Triple(subject, predicate, @object));
        }

        //	/**
        //	 * Clear objects cached for efficiency when the
        //	 * object state changes.
        //	 * 
        //	 * Messy since state can be changed outside 
        //	 * but helps with runtimes.
        //	 */
        //	public void clearCachedObjects(){
        //		graphHash = null;
        ////		hashCode = 0;
        //		groundGraphHash = null;
        //	}
        //	
        /**
         * Produces a cheap copy of the graph, where immutable objects
         * (graph structure, static hashes, hash function) are copied by pointer, 
         * whereas dynamic objects (dynamic hashes) are shallow-copied
         * @return
         */
        public HashGraph Branch()
        {
            Dictionary<Node, string> dh = new Dictionary<Node, string>(_dynamicHashes);
            HashGraph hg = new HashGraph(_hf, _data, _staticHashes, _blankHash, dh);
            return hg;
        }

        /**
         * Get the HashNode containing the current hash for the node.
         * 
         * @param n
         * @return
         */
        public string GetHash(Node n)
        {
            string val;
            if (n.IsBlankNode())
            {
                _dynamicHashes.TryGetValue(n, out val);
                return val;
            }
            _staticHashes.TryGetValue(n, out val);
            return val;
        }

        /**
         * Get the HashNode containing the current hash for the blank node.
         * 
         * @param n
         * @return
         */
        public string GetHash(BlankNode n)
        {
            _dynamicHashes.TryGetValue(n, out string val);
            return val;
        }

        private string GetOrCreatestring(Node n)
        {
            string hc = GetHash(n);
            if (hc == null)
            {
                if (n.IsBlankNode())
                {
                    hc = _blankHash;
                    _dynamicHashes.Add(n, hc);
                }
                else
                {
                    hc = _hf.CalculateHash(n.ValueAsString());
                    _staticHashes.Add(n, hc);
                }
            }
            return hc;
        }

        public IHashCalculator GetIHashCalculator()
        {
            return _hf;
        }

        public Dictionary<Node, string> GetBlankNodeHashes()
        {
            return _dynamicHashes;
        }

        public string GetGraphHash()
        {
            string b = _blankHash;
            foreach (var t in _data)
            {
                var tup = new string[]
                {
                    GetHash(t.Subject as Node),
                    GetHash(t.Predicate as Node),
                    GetHash(t.Object as Node)
                };
                string o = _hf.CombineOrdered(tup);

                var tup2 = new string[] { o, b };
                b = _hf.CombineUnordered(tup2);
            }
            return b;
        }

        /**
         * Hash all blank nodes with the mux
         * @param mux
         * @return
         */
        public static void MuxHash(HashGraph hg, string mux)
        {
            HashSet<Node> bnodes = new HashSet<Node>(hg.GetBlankNodeHashes().Keys);

            foreach (var b in bnodes)
            {
                string hc = hg.GetHash(b);
                var tup = new string[] { hc, mux };
                string comb = _hf.CombineOrdered(tup);
                hg.GetBlankNodeHashes().Add(b, comb);
            }
        }

        public string GetGroundSubGraphHash()
        {
            string b = _blankHash;
            foreach (var t in _data)
            {
                if (!(t.Subject.IsBlankNode()) && !(t.Object.IsBlankNode()))
                {
                    var tup = new string[]
                    {
                        GetHash(t.Subject as Node),
                        GetHash(t.Predicate as Node),
                        GetHash(t.Object as Node)
                    };
                    string o = _hf.CombineOrdered(tup);

                    var tup2 = new string[] { o, b };
                    b = _hf.CombineUnordered(tup2);
                }
            }
            return b;
        }

        public void UpdateBNodeHashes(Dictionary<Node, string> bnodeHashes)
        {
            foreach (var kv in bnodeHashes)
            {
                if (_dynamicHashes.ContainsKey(kv.Key))
                {
                    _dynamicHashes[kv.Key] = kv.Value;
                }
                else
                {
                    _dynamicHashes.Add(kv.Key, kv.Value);
                }
            }
        }

        public void SetBNodeHashes(Dictionary<Node, string> bnodeHashes)
        {
            _dynamicHashes = bnodeHashes;
        }

        public List<Triple> GetData()
        {
            return _data;
        }

        public override string ToString()
        {
            var buf = new StringBuilder();

            foreach (var triple in _data)
            {
                buf.Append(triple.Subject.ToString() + "#" + GetHash(triple.Subject as Node) + " ");
                buf.Append(triple.Predicate.ToString() + "#" + GetHash(triple.Predicate as Node) + " ");
                buf.Append(triple.Object.ToString() + "#" + GetHash(triple.Object as Node) + " ");
                buf.Append(".\n");
            }
            return buf.ToString();
        }

        /**
         * Splits the hashgraph into multiple where each
         * graph represents all triples for each set of connected blank nodes 
         * (blank nodes appearing in the same triple).
         * 
         * Ground triples are removed (if previously present).
         * 
         * Shallow copy: colours are preserved.
         * 
         * @return
         */
        public ICollection<HashGraph> BlankNodePartition()
        {
            var part = new Partition<Node>();

            // first create a partition of blank nodes based
            // on being connected
            foreach (Triple t in _data)
            {
                if (t.Subject.IsBlankNode() && t.Object.IsBlankNode() && !t.Subject.Equals(t.Object))
                {
                    part.AddPair(t.Subject as Node, t.Object as Node);
                }
            }

            var pivotToGraph = new Dictionary<Node, HashGraph>();
            foreach (var t in _data)
            {
                // doesn't matter which we pick, both are in the
                // same partition
                BlankNode b = null;
                if (t.Subject.IsBlankNode())
                {
                    b = t.Subject.AsBlankNode();
                }
                else if (t.Object.IsBlankNode())
                {
                    b = t.Object.AsBlankNode();
                }
                else
                {
                    continue;
                }

                // use the lowest bnode in the partition
                // to map to its graph
                var bp = part.GetPartition(b);
                Node pivot = null;

                if (bp == null || bp.Count == 0)
                {
                    pivot = b; // singleton ... unconnected blank node
                }
                else
                {
                    pivot = bp[0];
                }

                HashGraph hg;
                if (!pivotToGraph.ContainsKey(pivot))
                {
                    hg = new HashGraph(_hf);
                    pivotToGraph.Add(pivot, hg);
                }
                else
                {
                    hg = pivotToGraph[pivot];
                }
                hg.AddTriple(t);
            }
            return pivotToGraph.Values;
        }
    }
}
