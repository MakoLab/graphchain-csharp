namespace elephant.core.model.rdf.impl
{
    public class BlankNode : Node, ISubject, IObject
    {
        public string Identifier { get; private set; }

        public BlankNode(string identifier)
        {
            Identifier = identifier;
        }

        public override string ToString()
        {
            return "BlankNode{" +
                "identifier='" + Identifier + '\'' +
                '}';
        }

        public override string ValueAsString()
        {
            return Identifier;
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
            BlankNode other = (BlankNode)o;
            return Equals(Identifier, other.Identifier);
        }

        public override int GetHashCode()
        {
            return System.Collections.Generic.EqualityComparer<string>.Default.GetHashCode(Identifier);
        }
    }
}
