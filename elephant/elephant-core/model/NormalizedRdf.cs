using System.Collections.Generic;
using elephant.core.model.rdf;
using elephant.core.util;
using elephant.core.vocabulary;

namespace elephant.core.model
{
    public class NormalizedRdf
    {
        public RdfDataset RdfDataset { get; private set; }
        public string SerializedNormalizedObject { get; private set; }
        public RdfFormat SerializationFormat { get; private set; }

        public NormalizedRdf(RdfDataset rdfDataset, string serializedNormalizedObject, RdfFormat serializationFormat)
        {
            RdfDataset = rdfDataset;
            SerializedNormalizedObject = serializedNormalizedObject;
            SerializationFormat = serializationFormat;
        }

        public List<Triple> GetListOfNormalizedTriples()
        {
            var result = new List<Triple>();

            result.AddRange(RdfDataset.Triples);
            result.Sort(new NormalizingComparator());

            return result;
        }
    }
}
