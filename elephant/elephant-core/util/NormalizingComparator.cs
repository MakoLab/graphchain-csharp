using System.Collections.Generic;
using elephant.core.model.rdf;

namespace elephant.core.util
{
    public class NormalizingComparator : IComparer<Triple>
    {
        public int Compare(Triple o1, Triple o2)
        {
            var comparisonResult = CompareSubject(o1.Subject, o2.Subject);

            if (comparisonResult == 0)
            {
                comparisonResult = ComparePredicate(o1.Predicate, o2.Predicate);

                if (comparisonResult == 0)
                {
                    return CompareObject(o1.Object, o2.Object);
                }
                else
                {
                    return comparisonResult;
                }
            }
            else
            {
                return comparisonResult;
            }
        }

        private int CompareObject(IObject o1, IObject o2)
        {
            // TODO
            return o1.ValueAsString().CompareTo(o2.ValueAsString());
        }

        private int ComparePredicate(IPredicate o1, IPredicate o2)
        {
            // TODO
            return o1.ValueAsString().CompareTo(o2.ValueAsString());
        }

        private int CompareSubject(ISubject o1, ISubject o2)
        {
            // TODO
            return o1.ValueAsString().CompareTo(o2.ValueAsString());
        }
    }
}
