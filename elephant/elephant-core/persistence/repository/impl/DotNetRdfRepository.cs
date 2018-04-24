﻿using System;
using System.Collections.Generic;
using elephant.core.model.chain;
using elephant.core.ramt;
using elephant.core.service;
using elephant.core.util;
using Microsoft.Extensions.Logging;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using VDS.RDF.Writing;

namespace elephant.core.persistence.repository.impl
{
    public class DotNetRdfRepository : AbstractTripleStoreRepository, ITriplestoreRepository
    {
        private const int NUMBER_OF_ATTEMPTS = 3;

        private TripleStore _repository;
        private DotNetRdfMapper _rdf4jMapper;

        public DotNetRdfRepository(ILogger<DotNetRdfRepository> logger, QueryTemplatesService queryTemplatesService, StoreConfiguration sConfig)
        {
            _logger = logger;
            _chainGraphIri = sConfig.ChainGraphIri;
            _queryTemplatesService = queryTemplatesService;

            Init();
        }

        public void Init()
        {
            _logger.LogDebug("[CONFIG] Creating a triple store repository...");
            _logger.LogDebug("\t[CONFIG] chainGraphIri={0}", _chainGraphIri);
            _repository = new TripleStore();
            _rdf4jMapper = new DotNetRdfMapper();
            _logger.LogInformation("[CONFIG] The triple store repository configured correctly.");
        }

        public void ShutDown()
        {
            _logger.LogInformation("[CONFIG] Shutting down the triple store repository...");
            _repository.Dispose();
        }

        /**
         * Checks whether the repository is ready to use, e.g. it contains the genesis Block.
         *
         * @return <code>true</code> if the repository is ready to use, <code>false</code> otherwise
         */
        public bool IsReadyToUse()
        {
            return _repository.HasGraph(new Uri(_chainGraphIri));
        }

        public LastBlockInfo GetLastBlockInfo()
        {
            _logger.LogDebug("Getting last block info...");
            string queryName = "get_last_block_info";
            _logger.LogTrace("Invoking the '{0}' SPARQL query...", queryName);
            string query = String.Format(_queryTemplatesService.GetQueryTemplate(queryName), _chainGraphIri);
            var parser = new SparqlQueryParser();
            SparqlQuery tupleQuery = parser.ParseFromString(query);
            var processor = new LeviathanQueryProcessor(_repository);
            var resultSet = processor.ProcessQuery(tupleQuery) as SparqlResultSet;

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
            var parser = new SparqlQueryParser();
            SparqlQuery tupleQuery = parser.ParseFromString(sparqlQuery);
            var processor = new LeviathanQueryProcessor(_repository);
            var result = processor.ProcessQuery(tupleQuery) as IGraph;
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
                    _repository.Add(model, true);
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
            var parser = new SparqlQueryParser();
            SparqlQuery tupleQuery = parser.ParseFromString(query);
            var processor = new LeviathanQueryProcessor(_repository);
            var graphQueryResult = processor.ProcessQuery(tupleQuery) as IGraph;

            foreach (var triple in graphQueryResult.Triples)
            {
                result.Add(_rdf4jMapper.StatementToTriple(triple));
            }
            return result;
        }

        public void ClearRepository()
        {
            foreach (var g in _repository.Graphs)
            {
                _repository.Remove(g.BaseUri);
            }
        }
    }
}
