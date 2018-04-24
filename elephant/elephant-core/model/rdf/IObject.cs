using elephant.core.model.rdf.impl;

namespace elephant.core.model.rdf
{
    public interface IObject
    {
        BlankNode AsBlankNode();

        bool IsBlankNode();

        Iri AsIri();

        bool IsIri();

        Literal AsLiteral();

        bool IsLiteral();

        string ValueAsString();
    }
}
