using System;
using System.Collections.Generic;
using elephant.core.model.chain;
using elephant.core.model.rdf;

namespace elephant.core.persistence.repository
{
    public interface ITriplestoreRepository
    {
        void Init();

        void ShutDown();

        /**
         * Checks whether the repository is ready to use, e.g. it contains the genesis Block.
         *
         * @return <code>true</code> if the repository is ready to use, <code>false</code> otherwise
         */
        bool IsReadyToUse();

        LastBlockInfo GetLastBlockInfo();

        HashSet<Triple> GetBlockHeaderByIri(string blockIri);

        HashSet<Triple> GetGraphByIri(string graphIri);

        string MakeSparqlQueryAndReturnGraphAsJsonLdString(string sparqlQuery);

        void PersistTriples(String graphIri, HashSet<Triple> triplesToPersist);
        void ClearRepository();
    }
}
