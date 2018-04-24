using System.Text;

namespace elephant.core.model.rdf
{
    public class Triple
    {
        public ISubject Subject { get; private set; }
        public IPredicate Predicate { get; private set; }
        public IObject Object { get; private set; }

    public Triple(ISubject subject, IPredicate predicate, IObject object_)
        {
            Subject = subject;
            Predicate = predicate;
            Object = object_;
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
            Triple triple = (Triple)o;
            return Equals(Subject, triple.Subject) &&
                Equals(Predicate, triple.Predicate) &&
                Equals(Object, triple.Object);
        }

        public override int GetHashCode()
        {
            return Subject.GetHashCode() + Predicate.GetHashCode() + Object.GetHashCode();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("(")
                .Append(Subject).Append(", ")
                .Append(Predicate).Append(", ")
                .Append(Object)
                .Append(")");
            return sb.ToString();
        }
    }
}
