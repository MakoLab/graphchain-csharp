using elephant.core.model.rdf;

namespace elephant.core.ramt.serialization
{
    public class NTriplesFormatHandler
    {
        public string SerializeTriple(Triple triple)
        {
            string subject;
            if (triple.Subject.IsIri())
            {
                subject = "<" + triple.Subject.AsIri().ValueAsString() + ">";
            }
            else
            { // is blank node
              // we are talking currently hold internal serialization of a blank node
                subject = triple.Subject.AsBlankNode().Identifier;
            }

            var predicate = "<" + triple.Predicate.AsIri().ValueAsString() + ">";

            string object_;
            if (triple.Object.IsIri())
            {
                object_ = "<" + triple.Object.AsIri().ValueAsString() + ">";
            }
            else if (triple.Object.IsBlankNode())
            {
                object_ = triple.Object.AsBlankNode().Identifier;
            }
            else
            {
                object_ = triple.Object.AsLiteral().ToString();
            }

            return subject + " " + predicate + " " + object_ + " .";

        }
    }
}
