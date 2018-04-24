using elephant.core.model.rdf.impl;

namespace elephant.core.vocabulary
{
    public class VocabularyUtil
    {
        public static Iri CreateIri(string _namespace, string localName)
        {
            return new Iri(_namespace + localName);
        }

        public static Iri CreateIri(string iriString)
        {
            return new Iri(iriString);
        }
    }
}
