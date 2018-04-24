using System.Collections.Generic;
using elephant.core.model.rdf;

namespace elephant.core.service.hashing
{
    public interface IHashingService
    {
        string CalculateHash(HashSet<Triple> triples);
        HashSet<Triple> HandleTriplesBeforePersisting(HashSet<Triple> triples);
        HashSet<Triple> HandleTriplesBeforeHashing(HashSet<Triple> triples);
    }
}
