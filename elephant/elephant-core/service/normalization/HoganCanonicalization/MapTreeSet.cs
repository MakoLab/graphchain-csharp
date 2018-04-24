using System.Collections.Generic;

namespace elephant.core.service.normalization.HoganCanonicalization
{
    public class MapTreeSet<E, F> : Dictionary<E, List<F>>
    {
        public MapTreeSet() : base() { }

        public MapTreeSet(IDictionary<F, E> toInvert) : base()
        {
            foreach (var kv in toInvert)
            {
                AddEntry(kv.Value, kv.Key);
            }
        }

        public void AddEntry(E a, F b)
        {
            List<F> set;
            if (ContainsKey(a))
            {
                set = this[a];
            }
            else
            {
                set = new List<F>();
                Add(a, set);
            }
            set.Add(b);
        }
    }
}
