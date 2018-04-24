using System.Collections.Generic;
using System.Text;
using elephant.core.model.rdf;

namespace elephant.core.model
{
    public class RdfDataset
    {
        public HashSet<Triple> Triples { get; private set; }

        public RdfDataset(HashSet<Triple> triples)
        {
            Triples = triples;
        }

  public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("{");
            bool firstTriple = true;
            foreach (var triple in Triples)
            {
                if (firstTriple)
                {
                    firstTriple = false;
                }
                else
                {
                    sb.Append(", ");
                }
                sb.Append("[");
                sb.Append(triple.Subject).Append(" -> ")
                    .Append(triple.Predicate).Append(" -> ")
                    .Append(triple.Object);
                sb.Append("]");
            }
            sb.Append("}");

            return sb.ToString();
        }
    }
}
