using elephant.core.model.rdf.impl;

namespace elephant.core.model.rdf
{
    public interface ISubject
    {
        BlankNode AsBlankNode();

        bool IsBlankNode();

        Iri AsIri();

        bool IsIri();

        string ValueAsString();
    }
}
