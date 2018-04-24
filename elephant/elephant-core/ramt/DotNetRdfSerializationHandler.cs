using System;
using System.IO;
using elephant.core.exception;
using elephant.core.vocabulary;
using VDS.RDF;

namespace elephant.core.ramt
{
    public class DotNetRdfSerializationHandler
    {
        public IGraph DeserializeGraph(string serializedModel, RdfFormat rdfFormat)
        {
            return DeserializeGraph("", serializedModel, rdfFormat);
        }

        /// <summary>
        /// Converts rdf string to dotNetRdf graph.
        /// </summary>
        /// <param name="baseUri"></param>
        /// <param name="serializedModel">Literal string read from file.</param>
        /// <param name="rdfFormat"></param>
        /// <returns></returns>
        public IGraph DeserializeGraph(string baseUri, string serializedModel, RdfFormat rdfFormat)
        {
            // TODO: Handle 'Optional' properly
            var reader = RdfFormatsMapper.ToRdfReader(rdfFormat);
            try
            {
                if (reader is IRdfReader)
                {
                    var g = new Graph();
                    if (!String.IsNullOrEmpty(baseUri))
                    {
                        g.BaseUri = new Uri(baseUri);
                    }
                    g.LoadFromString(serializedModel, (IRdfReader)reader);
                    return g;
                }
                if (reader is IStoreReader)
                {
                    var ts = new TripleStore();
                    ts.LoadFromString(serializedModel, (IStoreReader)reader);
                    var g = ts.Graphs[null];
                    if (!String.IsNullOrEmpty(baseUri))
                    {
                        g.BaseUri = new Uri(baseUri);
                    }
                    return g;
                }
                return null;
            }
            catch (IOException ex)
            {
                string msg = String.Format("Unable to parse serialized RDF from '%s' format.", rdfFormat.GetJenaName());
                throw new RdfSerializationException(msg, ex);
            }
        }
    }
}
