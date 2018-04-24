using System;
using elephant.core.model.rdf.impl;

namespace elephant.core.vocabulary.xml
{
    public class XMLSchema
    {
        public static String NAMESPACE = "http://www.w3.org/2001/XMLSchema#";

        public static Iri AnyURI = VocabularyUtil.CreateIri(NAMESPACE, "anyURI");
        public static Iri Decimal = VocabularyUtil.CreateIri(NAMESPACE, "decimal");
        public static Iri String = VocabularyUtil.CreateIri(NAMESPACE, "string");
        public static Iri Long = VocabularyUtil.CreateIri(NAMESPACE, "long");
    }
}
