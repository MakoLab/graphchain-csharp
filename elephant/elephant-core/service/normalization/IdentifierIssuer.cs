using System.Collections.Generic;

namespace elephant.core.service.normalization
{
    public class IdentifierIssuer
    {
        private readonly string _identifierPrefix;

        public int IdentifierCounter { get; private set; }

        public Dictionary<string, string> IssuedIdentifiers { get; private set; }

        public IdentifierIssuer(string prefix = "_:c14n")
        {
            _identifierPrefix = prefix;
            IdentifierCounter = 0;
            IssuedIdentifiers = new Dictionary<string, string>();
        }

        public string IssueIdentifier(string identifier)
        {
            if (IssuedIdentifiers.ContainsKey(identifier))
            {
                return IssuedIdentifiers[identifier];
            }
            var ci = _identifierPrefix + IdentifierCounter++;
            IssuedIdentifiers.Add(identifier, ci);
            return ci;
        }

        public bool IsIssued(string identifier)
        {
            return IssuedIdentifiers.ContainsKey(identifier);
        }

        public string GetIssuedIdentifier(string identifier)
        {
            if (IsIssued(identifier))
            {
                return IssuedIdentifiers[identifier];
            }
            return null;
        }

        public IdentifierIssuer GetCopy()
        {
            return new IdentifierIssuer(this);
        }

        private IdentifierIssuer(IdentifierIssuer i)
        {
            _identifierPrefix = i._identifierPrefix;
            IdentifierCounter = i.IdentifierCounter;
            IssuedIdentifiers = new Dictionary<string, string>(i.IssuedIdentifiers);
        }
    }
}
