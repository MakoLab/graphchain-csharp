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
    public class StardogTripleStoreRepository : AbstractTripleStoreRepository, ITriplestoreRepository
    {
        private const int NUMBER_OF_ATTEMPTS = 3;

        private StardogConnector _stardogGraphConnector;
        private StardogConnectionData _sdConnData;
        private PersistentTripleStore _repository;
        private DotNetRdfMapper _rdf4jMapper;

        public StardogTripleStoreRepository(ILogger<StardogTripleStoreRepository> logger, QueryTemplatesService queryTemplatesService, StardogConnectionData sgConnData, StoreConfiguration sConfig)
        {
            _logger = logger;
            _chainGraphIri = sConfig.ChainGraphIri;
            _queryTemplatesService = queryTemplatesService;
            _sdConnData = sgConnData;
            Init();
        }

        public void Init()
        {
            _stardogGraphConnector = new StardogConnector(_sdConnData.Url, _sdConnData.StoreId, _sdConnData.User, _sdConnData.Pass);
            _repository = new PersistentTripleStore(_stardogGraphConnector);
            _rdf4jMapper = new DotNetRdfMapper();
            _logger.LogInformation("[CONFIG] The triple store repository configured correctly.");
        }

        public void ShutDown()
        {
            _repository.Dispose();
            _stardogGraphConnector.Dispose();
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
                        _stardogGraphConnector.UpdateGraph(model.BaseUri, model.Triples, null);
                    }
                    else
                    {
                        _stardogGraphConnector.SaveGraph(model);
                    }
                    tryMore = false;
                }
                catch (Exception ex)
                {
                    _logger.LogError("Exception was thrown while adding a model to the repository.", ex);
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
    }
}
