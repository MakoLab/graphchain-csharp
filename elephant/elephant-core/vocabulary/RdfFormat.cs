using System;
using System.Collections.Generic;
using static elephant.core.vocabulary.SemanticWebMimeType;

namespace elephant.core.vocabulary
{
    public class RdfFormat
    {
        public static RdfFormat TURTLE = new RdfFormat("TURTLE", TEXT_TURTLE_VALUE, "ttl", "n3");
        public static RdfFormat N_TRIPLES = new RdfFormat("N-TRIPLE", N_TRIPLES_VALUE, "nt");
        public static RdfFormat N_QUADS = new RdfFormat("N-QUADS", N_QUADS_VALUE, "nq");
        public static RdfFormat TRIG = new RdfFormat("TRIG", TRIG_VALUE, "trig");
        public static RdfFormat RDF_XML = new RdfFormat("RDF/XML", RDF_XML_VALUE, "rdf", "owl");
        public static RdfFormat JSON_LD = new RdfFormat("JSON-LD", JSON_LD_VALUE, "jsonld");
        public static RdfFormat JSON_LD_NON_EXPANDED = new RdfFormat("JSON-LD", JSON_LD_VALUE, "jsonld");
        public static RdfFormat RDF_THRIFT = new RdfFormat("RDF-THRIFT", RDF_THRIFT_VALUE, "trdf", "rt");
        public static RdfFormat RDF_JSON = new RdfFormat("RDF/JSON", RDF_JSON_VALUE, "rj");
        public static RdfFormat TRIX = new RdfFormat("TRIX", TRIX_VALUE, "trix");

        private static RdfFormat[] _values = { TURTLE, N_TRIPLES, N_QUADS, TRIG, RDF_XML, JSON_LD, JSON_LD_NON_EXPANDED, RDF_THRIFT, RDF_JSON, TRIX };

        private string _mimeType;
        private string _jenaName;
        private List<string> _extensions = new List<string>();

        private RdfFormat(string jenaName, string mimeType, params string[] exts)
        {
            _jenaName = jenaName;
            _mimeType = mimeType;
            _extensions.AddRange(exts);
        }

        /**
        * Returns the Jena name for this serialization format. Returned name is used in some of Jena's
        * methods.
        *
        * @return String with Jena name
        */
        public string GetJenaName()
        {
            return _jenaName;
        }

        /**
         * Returns the MIME type for this serialization format.
         *
         * @return String with MIME type
         */
        public string GetMimeType()
        {
            return _mimeType;
        }

        /**
         * Returns a proper RDF format for the extension.
         *
         * @param extension an extension by which RDF format should be found
         * @return serialization format
         */
        public static RdfFormat GetByExtension(string extension)
        {
            foreach (RdfFormat value in _values)
            {
                foreach (string ext in value._extensions)
                {
                    if (ext.Equals(extension))
                    {
                        return value;
                    }
                }
            }

            string exceptionMsg =
                String.Format("There isn't any RDF format with '%s'.", extension);
            throw new ArgumentException(exceptionMsg);
        }

        /**
         * Returns a proper RDF format for the MIME type.
         *
         * @param mimeType a MIME type by which RDF format should be found
         * @return RDF format
         */
        public static RdfFormat getByMimeType(String mimeType)
        {
            foreach (RdfFormat value in _values)
            {
                if (value.GetMimeType().Equals(mimeType))
                {
                    return value;
                }
            }

            String exceptionMsg = String.Format("There is no element with MIME type '%s'.", mimeType);
            throw new ArgumentException(exceptionMsg);
        }
    }
}
