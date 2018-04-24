using System.Collections.Generic;
using elephant.core.model.rdf;

namespace elephant.core.model.chain
{
    public class BlockContent
    {
        public string DataGraphIri { get; private set; }
        public HashSet<Triple> Triples { get; private set; }

        public BlockContent(string dataGraphIri, HashSet<Triple> triples)
        {
            DataGraphIri = dataGraphIri;
            Triples = triples;
        }
    }
}
