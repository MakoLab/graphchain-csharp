using elephant.core.model.rdf.impl;

namespace elephant.core.vocabulary.bc
{
    public class GCO
    {
        public const string MAKOLAB_BC = "http://www.ontologies.makolab.com/bc";
        public const string MAKOLAB_BC_NAMESPACE = "http://www.ontologies.makolab.com/bc/";

        /*
         * Classes
         */
        public const string BLOCK = MAKOLAB_BC_NAMESPACE + "Block";
        public const string GENESIS_BLOCK = MAKOLAB_BC_NAMESPACE + "GenesisBlock";

        /*
         * Properties
         */
        public const string HAS_DATA_GRAPH_IRI = MAKOLAB_BC_NAMESPACE + "hasDataGraphIRI";
        public const string HAS_DATA_HASH = MAKOLAB_BC_NAMESPACE + "hasDataHash";
        public const string HAS_HASH = MAKOLAB_BC_NAMESPACE + "hasHash";
        public const string HAS_INDEX = MAKOLAB_BC_NAMESPACE + "hasIndex";
        public const string HAS_PREVIOUS_BLOCK = MAKOLAB_BC_NAMESPACE + "hasPreviousBlock";
        public const string HAS_PREVIOUS_HASH = MAKOLAB_BC_NAMESPACE + "hasPreviousHash";
        public const string HAS_TIME_STAMP = MAKOLAB_BC_NAMESPACE + "hasTimeStamp";

        /*
         * IRIs - Classes
         */
        public static Iri Block = VocabularyUtil.CreateIri(BLOCK);
        public static Iri GenesisBlock = VocabularyUtil.CreateIri(GENESIS_BLOCK);

        /*
         * IRIs - Properties
         */
        public static Iri hasDataGraphIri = VocabularyUtil.CreateIri(HAS_DATA_GRAPH_IRI);
        public static Iri hasDataHash = VocabularyUtil.CreateIri(HAS_DATA_HASH);
        public static Iri hasHash = VocabularyUtil.CreateIri(HAS_HASH);
        public static Iri hasIndex = VocabularyUtil.CreateIri(HAS_INDEX);
        public static Iri hasPreviousBlock = VocabularyUtil.CreateIri(HAS_PREVIOUS_BLOCK);
        public static Iri hasPreviousHash = VocabularyUtil.CreateIri(HAS_PREVIOUS_HASH);
        public static Iri hasTimeStamp = VocabularyUtil.CreateIri(HAS_TIME_STAMP);
    }
}
