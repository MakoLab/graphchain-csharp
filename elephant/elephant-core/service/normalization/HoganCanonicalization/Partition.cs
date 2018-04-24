using System.Collections.Generic;

namespace elephant.core.service.normalization.HoganCanonicalization
{
    public class Partition<E> : MapTreeSet<E, E>
    {
        int _partitions;

        public Partition() : base() { }

        public Partition(MapTreeSet<E, E> map) : base()
        {
            foreach (var set in map.Values)
            {
                foreach (var el in set)
                {
                    Add(el, set);
                }
            }
        }

        public List<E> GetPartition(E e)
        {
            if (ContainsKey(e))
            {
                return this[e];
            }
            return null;
        }

        public int CountPartitions()
        {
            return _partitions;
        }

        public int CountElements()
        {
            return Count;
        }

        public bool AddPair(E a, E b)
        {
            List<E> ha;
            List<E> hb;

            if (!ContainsKey(a) && !ContainsKey(b))
            {
                ha = new List<E>();
                _partitions++;
                ha.Add(a);
                ha.Add(b);
                Add(a, ha);
                Add(b, ha);
            }
            else if (!ContainsKey(a))
            {
                hb = this[b];
                hb.Add(a);
                Add(a, hb);
            }
            else if (!ContainsKey(b))
            {
                ha = this[a];
                ha.Add(b);
                Add(b, ha);
            }
            else
            {
                ha = this[a];
                hb = this[b];
                if (ha != hb)
                {
                    List<E> small;
                    List<E> big;
                    if (ha.Count > hb.Count)
                    {
                        small = hb;
                        big = ha;
                    }
                    else
                    {
                        big = hb;
                        small = ha;
                    }
                    foreach (var s in small)
                    {
                        big.Add(s);
                        Add(s, big);
                    }
                    _partitions--;
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
    }
}
