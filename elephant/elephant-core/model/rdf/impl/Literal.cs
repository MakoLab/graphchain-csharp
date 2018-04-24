using System.Text;
using elephant.core.util;
using elephant.core.vocabulary.rdf;
using elephant.core.vocabulary.xml;

namespace elephant.core.model.rdf.impl
{
    public class Literal : Node, IObject
    {
        public string LexicalForm { get; private set; }

        public Iri Datatype { get; private set; }

        public string LanguageTag { get; private set; }

        public Literal(string lexicalForm)
        {
            lexicalForm.RequireNonNull("'lexicalForm' parameter must not be null!");
            LexicalForm = lexicalForm;
            Datatype = XMLSchema.String; // This is the default datatype's IRI when literal's datatype is not specified.
            LanguageTag = null;
        }

        public Literal(string lexicalForm, Iri datatype)
        {
            lexicalForm.RequireNonNull("'lexicalForm' parameter must not be null!");
            datatype.RequireNonNull("'datatype' parameter must not be null!");
            LexicalForm = lexicalForm;
            Datatype = datatype;
            LanguageTag = null;
        }

        public Literal(string lexicalForm, string languageTag)
        {
            lexicalForm.RequireNonNull("'lexicalForm' parameter must not be null!");
            languageTag.RequireNonNull("'languageTag' parameter must not be null!");
            LexicalForm = lexicalForm;
            Datatype = RDF.langString; // This is the default datatype's IRI for language-tagged strings.
            LanguageTag = languageTag;
        }

        public override string ValueAsString()
        {
            return LexicalForm;
        }

        public override bool Equals(object o)
        {
            if (this == o)
            {
                return true;
            }
            if (o == null || GetType() != o.GetType())
            {
                return false;
            }
            Literal literal = (Literal)o;
            return Equals(LexicalForm, literal.LexicalForm) &&
                Equals(Datatype, literal.Datatype) &&
                Equals(LanguageTag, literal.LanguageTag);
        }

        public override int GetHashCode()
        {
            return (LexicalForm?.GetHashCode() ?? 0) + (Datatype?.GetHashCode() ?? 0) + (LanguageTag?.GetHashCode() ?? 0);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\"")
                .Append(LexicalForm)
                .Append("\"");
            if (LanguageTag != null)
            {
                sb.Append("@").Append(LanguageTag);
            }
            else
            {
                sb.Append("^^<").Append(Datatype).Append(">");
            }
            return sb.ToString();
        }
    }
}
