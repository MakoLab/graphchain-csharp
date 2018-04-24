using System;

namespace elephant.core.model.rdf.impl
{
    public class Iri : Node, ISubject, IPredicate, IObject
    {
        private string _iriInternalValue;

        public Iri(string iri)
        {
            _iriInternalValue = iri;
        }

        public override string ValueAsString()
        {
            return _iriInternalValue;
        }

        public Uri ValueAsUri()
        {
            return new Uri(_iriInternalValue);
        }

        public override string ToString()
        {
            return _iriInternalValue;
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
            Iri other = (Iri)o;
            return Equals(_iriInternalValue, other._iriInternalValue);
        }

        public override int GetHashCode()
        {
            return System.Collections.Generic.EqualityComparer<string>.Default.GetHashCode(_iriInternalValue);
        }
    }
}
