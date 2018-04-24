using elephant.core.model.rdf.impl;

namespace elephant.core.model.rdf
{
    public interface IPredicate
    {
        Iri AsIri();
        bool IsIri();
        string ValueAsString();
    }
}
