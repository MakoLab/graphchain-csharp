namespace elephant.core.model.rdf.impl
{
    public abstract class Node
    {
        public BlankNode AsBlankNode()
        {
            return (BlankNode)this;
        }

        public bool IsBlankNode()
        {
            return this is BlankNode;
        }

        public Iri AsIri()
        {
            return (Iri)this;
        }

        public bool IsIri()
        {
            return this is Iri;
        }

        public Literal AsLiteral()
        {
            return (Literal)this;
        }

        public bool IsLiteral()
        {
            return this is Literal;
        }

        public abstract string ValueAsString();
    }
}
