using System.Collections.Generic;
using elephant.core.model;
using elephant.core.model.rdf;

namespace elephant.core.service.normalization
{
    public interface INormalizationService
    {
        NormalizedRdf NormalizeRdf(HashSet<Triple> triples);
    }
}
