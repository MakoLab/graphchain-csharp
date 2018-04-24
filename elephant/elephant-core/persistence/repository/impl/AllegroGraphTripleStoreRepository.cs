using System;
using System.Collections.Generic;
using elephant.core.model.chain;
using elephant.core.ramt;
using elephant.core.service;
using elephant.core.util;
using Microsoft.Extensions.Logging;
using VDS.RDF;
using VDS.RDF.Query;
using VDS.RDF.Storage;
using VDS.RDF.Writing;

namespace elephant.core.persistence.repository.impl
{
    public class AllegroGraphTripleStoreRepository : AbstractTripleStoreRepository, ITriplestoreRepository
    {
        private const int NUMBER_OF_ATTEMPTS = 3;

        private AllegroGraphConnector _allegroGraphConnector;
        private AllegroGraphConnectionData _agConnData;
        private PersistentTripleStore _repository;
        private DotNetRdfMapper _rdf4jMapper;

        public AllegroGraphTripleStoreRepository(ILogger<AllegroGraphTripleStoreRepository> logger, QueryTemplatesService queryTemplatesService, AllegroGraphConnectionData agConnData, StoreConfiguration sConfig)
        {
            _logger = logger;
            _chainGraphIri = sConfig.ChainGraphIri;
            _queryTemplatesService = queryTemplatesService;
            _agConnData = agConnData;
            Init();
        }

        public void Init()
        {
            _allegroGraphConnector = new AllegroGraphConnector(_agConnData.Url, _agConnData.StoreId, _agConnData.User, _agConnData.Pass);
            _repository = new PersistentTripleStore(_allegroGraphConnector);
            _rdf4jMapper = new DotNetRdfMapper();
            _logger.LogInformation("[CONFIG] The triple store repository configured correctly.");
        }

        public void ShutDown()
        {
            _repository.Dispose();
            _allegroGraphConnector.Dispose();
        }

        public bool IsReadyToUse() => _repository.HasGraph(new Uri(_chainGraphIri));

        public LastBlockInfo GetLastBlockInfo()
        {
            _logger.LogDebug("Getting last block info...");
            string queryName = "get_last_block_info";
            _logger.LogTrace("Invoking the '{0}' SPARQL query...", queryName);
            string query = String.Format(_queryTemplatesService.GetQueryTemplate(queryName), _chainGraphIri);
            var resultSet = _repository.ExecuteQuery(query) as SparqlResultSet;
            if (resultSet.IsEmpty)
            {
                return null;
            }
            var bindingSet = resultSet.Results[0];
            string lastBlockIri = bindingSet.Value("lastBlockIRI").ToString();
            string lastBlockHash = (bindingSet.Value("lastBlockHash") as ILiteralNode).Value;
            string lastBlockIndex = (bindingSet.Value("lastBlockIndex") as ILiteralNode).Value;

            return new LastBlockInfo(lastBlockIri, lastBlockHash, lastBlockIndex);
        }

        public string MakeSparqlQueryAndReturnGraphAsJsonLdString(string sparqlQuery)
        {
            var result = _repository.ExecuteQuery(sparqlQuery) as IGraph;
            var ts = new TripleStore();
            ts.Add(result);
            return StringWriter.Write(ts, new JsonLdWriter());
        }

        public void PersistTriples(string graphIri, HashSet<model.rdf.Triple> triplesToPersist)
        {
            _logger.LogTrace("Persisting triples into graph with IRI '{0}'. Triples: {1}", graphIri, triplesToPersist);
            IGraph model = _rdf4jMapper.TriplesToGraph(triplesToPersist);
            var tryMore = true;
            model.BaseUri = new Uri(graphIri);
            for (var i = 0; i < NUMBER_OF_ATTEMPTS && tryMore; i++)
            {
                try
                {
                    if (_repository.HasGraph(model.BaseUri))
                    {
                        _allegroGraphConnector.UpdateGraph(model.BaseUri, model.Triples, null);
                    }
                    else
                    {
                        InsertGraph(_allegroGraphConnector, model);
                    }
                    tryMore = false;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception was thrown while adding a model to the repository.");
                }
            }
            _logger.LogTrace("Persisted {0} triples into the <{1}> graph.", triplesToPersist.Count, graphIri);
        }

        protected override HashSet<model.rdf.Triple> GetTriplesForQuery(string query)
        {
            var result = new HashSet<model.rdf.Triple>();
            var graphQueryResult = _repository.ExecuteQuery(query) as IGraph;

            foreach (var triple in graphQueryResult.Triples)
            {
                result.Add(_rdf4jMapper.StatementToTriple(triple));
            } 
            return result;
        }

        public void ClearRepository()
        {
            var query = _queryTemplatesService.GetQueryTemplate("delete_all_triples");
            try
            {
                _repository.ExecuteUpdate(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Delete error");
            }
        }

        private void InsertGraph(AllegroGraphConnector connector, IGraph g)
        {
            var length = 10000;
            if (g.Triples.Count > length)
            {
                var newGraph = new Graph
                {
                    BaseUri = g.BaseUri
                };
                connector.SaveGraph(newGraph);
                var section = new List<Triple>(length);
                foreach (var t in g.Triples)
                {
                    section.Add(t);
                    if (section.Count == length)
                    {
                        connector.UpdateGraph(g.BaseUri, section, null);
                        section = new List<Triple>(length);
                    }
                }
                if (section.Count > 0)
                {
                    connector.UpdateGraph(g.BaseUri, section, null);
                }
            }
            else
            {
                connector.SaveGraph(g);
            }
        }
    }
}
