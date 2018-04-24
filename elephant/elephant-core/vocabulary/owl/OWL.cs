using elephant.core.model.rdf.impl;

namespace elephant.core.vocabulary.owl
{
    public class OWL
    {
        public static string NAMESPACE = "http://www.w3.org/2002/07/owl#";
        public static Iri NamedIndividual = VocabularyUtil.CreateIri(NAMESPACE, "NamedIndividual");
    }
}
