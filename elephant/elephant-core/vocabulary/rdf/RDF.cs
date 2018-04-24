using elephant.core.model.rdf.impl;

namespace elephant.core.vocabulary.rdf
{
    public class RDF
    {
        public const string NAMESPACE = "http://www.w3.org/1999/02/22-rdf-syntax-ns#";

        public const string LANG_STRING = NAMESPACE + "langString";
        public const string TYPE = NAMESPACE + "type";

        /*
         * IRIs - Properties
         */
        public static Iri langString = VocabularyUtil.CreateIri(LANG_STRING);
        public static Iri type = VocabularyUtil.CreateIri(TYPE);
    }
}
