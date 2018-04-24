using System;
using System.Collections.Generic;
using elephant.core.model.rdf;
using elephant.core.model.rdf.impl;
using elephant.core.vocabulary;

namespace elephant.core.ramt
{
    public class DotNetRdfMapper
    {
        public Triple StatementToTriple(VDS.RDF.Triple statement)
        {
            ISubject subject;
            if (statement.Subject is VDS.RDF.IBlankNode)
            {
                subject = new BlankNode(((VDS.RDF.IBlankNode)statement.Subject).InternalID);
            }
            else
            {
                subject = new Iri(statement.Subject.ToString());
            }

            IPredicate predicate = new Iri(statement.Predicate.ToString());

            IObject object_;
            if (statement.Object is VDS.RDF.IBlankNode)
            {
                object_ = new BlankNode(((VDS.RDF.IBlankNode)statement.Object).InternalID);
            }
            else if (statement.Object is VDS.RDF.ILiteralNode literal)
            {
                string lexicalForm = literal.Value;
                if (!string.IsNullOrEmpty(literal.Language))
                {
                    object_ = new Literal(lexicalForm, literal.Language);
                }
                else if (literal.DataType != null)
                {
                    object_ = new Literal(lexicalForm, new Iri(literal.DataType.AbsoluteUri));
                }
                else
                {
                    object_ = new Literal(lexicalForm);
                }
            }
            else
            {
                object_ = new Iri(statement.Object.ToString());
            }

            return new Triple(subject, predicate, object_);
        }

        public HashSet<Triple> GraphToTriples(VDS.RDF.IGraph model)
        {
            HashSet<Triple> triples = new HashSet<Triple>();

            foreach (var statement in model.Triples)
            {
                triples.Add(StatementToTriple(statement));
            }

            return triples;
        }

        public VDS.RDF.IGraph TriplesToGraph(HashSet<Triple> triples)
        {
            var model = new VDS.RDF.Graph();

            foreach (Triple triple in triples)
            {
                model.Assert(TripleToStatement(triple));
            }

            return model;
        }

        public VDS.RDF.Triple TripleToStatement(Triple triple)
        {
            VDS.RDF.INode subject;
            var g = new VDS.RDF.Graph();
            if (triple.Subject.IsIri())
            {
                subject = g.CreateUriNode(new Uri(triple.Subject.ValueAsString()));
            }
            else
            {
                // If not an IRI, then subject has to be a blank node.
                subject = g.CreateBlankNode(triple.Subject.ValueAsString());
            }

            var predicate = g.CreateUriNode(new Uri(triple.Predicate.ValueAsString()));

            VDS.RDF.INode object_;
            if (triple.Object.IsBlankNode())
            {
                object_ = g.CreateBlankNode(triple.Object.ValueAsString());
            }
            else if (triple.Object.IsIri())
            {
                object_ = g.CreateUriNode(new Uri(triple.Object.ValueAsString()));
            }
            else
            {
                // If not a blank node or an IRI, then object has to be a literal.
                var literal = triple.Object.AsLiteral();
                if (!string.IsNullOrEmpty(literal.LanguageTag))
                {
                    object_ = g.CreateLiteralNode(literal.LexicalForm, literal.LanguageTag);
                }
                else
                {
                    object_ = g.CreateLiteralNode(literal.LexicalForm, literal.Datatype.ValueAsUri());
                }
            }

            return new VDS.RDF.Triple(subject, predicate, object_);
        }

        public string GraphToSerialization(VDS.RDF.IGraph model, object rdfFormat)
        {
            if (rdfFormat is VDS.RDF.IStoreWriter)
            {
                var ts = new VDS.RDF.TripleStore();
                ts.Add(model);
                return VDS.RDF.Writing.StringWriter.Write(ts, (VDS.RDF.IStoreWriter)rdfFormat);
            }
            return VDS.RDF.Writing.StringWriter.Write(model, (VDS.RDF.IRdfWriter)rdfFormat);
        }

        public string TriplesToSerialization(HashSet<Triple> triples, RdfFormat rdfFormat)
        {
            var graph = TriplesToGraph(triples);
            return GraphToSerialization(graph, RdfFormatsMapper.ToRdfWriter(rdfFormat));
        }
    }
}
