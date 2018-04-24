using System.Collections.Generic;
using elephant.core.model.rdf;

namespace elephant.core.service.normalization
{
    public class NormalizationState
    {
        public IdentifierIssuer CanonicalIssuer { get; set; }
        public Dictionary<string, List<Triple>> BNodesToQuads { get; set; }
        public Dictionary<string, List<string>> HashToBNodes { get; set; }

        public NormalizationState(string prefix = "_:c14n")
        {
            CanonicalIssuer = new IdentifierIssuer(prefix);
            BNodesToQuads = new Dictionary<string, List<Triple>>();
            HashToBNodes = new Dictionary<string, List<string>>();
        }
    }
}
