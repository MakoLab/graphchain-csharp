using elephant.core.model.rdf.impl;

namespace elephant.core.vocabulary.bc
{
    public class ElephantVocabulary
    {
        public const string NAMESPACE = "http://www.ontologies.makolab.com/mammoth/";

        /*
         * Classes
         */
        public const string LAST_BLOCK = NAMESPACE + "LastBlock";

        /*
         * Properties
         */
        public const string HAS_INTERNAL_LABEL = NAMESPACE + "hasInternalLabel";

        /*
         * IRIs - Classes
         */
        public static Iri LastBlock = VocabularyUtil.CreateIri(LAST_BLOCK);

        /*
         * IRIs - Properties
         */
        public static Iri HasInternalLabel = VocabularyUtil.CreateIri(HAS_INTERNAL_LABEL);
    }
}
