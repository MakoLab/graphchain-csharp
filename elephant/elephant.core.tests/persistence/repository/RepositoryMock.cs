using System.Collections.Generic;
using elephant.core.model.chain;
using elephant.core.model.rdf;
using elephant.core.model.rdf.impl;
using elephant.core.persistence.repository;
using elephant.core.vocabulary.bc;
using elephant.core.vocabulary.xml;

namespace elephant.core.tests.persistence.repository
{
    public class RepositoryMock : ITriplestoreRepository
    {
        public void Init()
        {
        }
        public void ShutDown()
        {
        }

        public bool IsReadyToUse()
        {
            return true;
        }

        public LastBlockInfo GetLastBlockInfo()
        {
            return new LastBlockInfo(
                    "http://www.ontologies.makolab.com/bcb/block1",
                    "6826923010db1e28ed439a88ec3ef22c1f3a878762ea520118f49959969de822",
                    "1"
                );
        }

        public HashSet<Triple> GetBlockHeaderByIri(string blockIri)
        {
            Iri iri = new Iri("http://www.ontologies.makolab.com/bcb/block1");

            HashSet<Triple> triples = new HashSet<Triple>
            {
                new Triple(
                    iri,
                    GCO.hasHash,
                    new Literal("xyz")
                ),
                new Triple(
                    iri,
                    GCO.hasDataHash,
                    new Literal("abc")
                ),
                new Triple(
                    iri,
                    GCO.hasIndex,
                    new Literal("1")
                ),
                new Triple(
                    iri,
                    GCO.hasPreviousBlock,
                    new Literal("http://www.ontologies.makolab.com/bcb/block1", XMLSchema.AnyURI)
                ),
                new Triple(
                    iri,
                    GCO.hasPreviousHash,
                    new Literal("qwe")
                ),
                new Triple(
                    iri,
                    GCO.hasDataGraphIri,
                    new Literal("http://www.ontologies.makolab.com/bcb/blockData1")
                ),
                new Triple(
                    iri,
                    GCO.hasTimeStamp,
                    new Literal("123456789", XMLSchema.Decimal)
                )
            };
            return triples;
        }

        public HashSet<Triple> GetGraphByIri(string graphIri)
        {
            return new HashSet<Triple>();
        }

        public string MakeSparqlQueryAndReturnGraphAsJsonLdString(string sparqlQuery)
        {
            return null;
        }

        public void PersistTriples(string graphIri, HashSet<Triple> triplesToPersist)
        {
        }

        public void ClearRepository()
        {
        }
    }
}
