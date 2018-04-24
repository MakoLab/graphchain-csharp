using System;
using elephant.core.vocabulary;
using VDS.RDF.Parsing;
using VDS.RDF.Writing;

namespace elephant.core.ramt
{
    public class RdfFormatsMapper
    {
        public static RdfFormat ToNativeFormat(object rdf4jFormat)
        {
            if (rdf4jFormat is CompressingTurtleWriter)
            {
                return RdfFormat.TURTLE;
            }
            else if (rdf4jFormat is CsvWriter)
            {
                throw new NotSupportedException();
            }
            else if (rdf4jFormat is NQuadsWriter)
            {
                return RdfFormat.N_QUADS;
            }
            else if (rdf4jFormat is PrettyRdfXmlWriter)
            {
                return RdfFormat.RDF_XML;
            }
            else
            {
                throw new RdfParseException("Unknown rdf format");
            }
        }

        public static object ToRdfWriter(RdfFormat rdfFormat)
        {
            switch (rdfFormat.GetJenaName())
            {
                case "RDF/XML":
                    return new PrettyRdfXmlWriter();
                case "JSON-LD":
                    return new JsonLdWriter();
                case "TURTLE":
                    return new CompressingTurtleWriter();
                case "N-QUADS":
                    return new NQuadsWriter();
                default:
                    throw new RdfParseException("Unknown rdf format");
            }
        }

        public static object ToRdfReader(RdfFormat rdfFormat)
        {
            switch (rdfFormat.GetJenaName())
            {
                case "RDF/XML":
                    return new RdfXmlParser();
                case "JSON-LD":
                    return new JsonLdParser();
                case "TURTLE":
                    return new TurtleParser();
                case "N-QUADS":
                    return new NQuadsParser();
                case "N-TRIPLE":
                    return new NTriplesParser();
                default:
                    throw new RdfParseException("Unknown rdf format");
            }
        }
    }
}
