using System.Collections.Generic;
using elephant.core.model.rdf;
using elephant.core.service;
using Microsoft.Extensions.Logging;

namespace elephant.core.persistence.repository
{
    public abstract class AbstractTripleStoreRepository
    {
        protected ILogger<AbstractTripleStoreRepository> _logger;
        protected QueryTemplatesService _queryTemplatesService;
        protected string _chainGraphIri;

        public HashSet<Triple> GetBlockHeaderByIri(string blockIri)
        {
            _logger.LogDebug("Getting block header triples by IRI '{0}'.", blockIri);
            string queryName = "get_block_header_by_iri";
            _logger.LogTrace("Invoking the '{0}' SPARQL query...", queryName);
            string query = string.Format(
                _queryTemplatesService.GetQueryTemplate(queryName),
                blockIri,
                _chainGraphIri,
                blockIri
            );

            return GetTriplesForQuery(query);
        }

        public HashSet<Triple> GetGraphByIri(string graphIri)
        {
            _logger.LogDebug("Getting graph by IRI '{0}'.", graphIri);
            string queryName = "get_graph_by_iri";
            _logger.LogTrace("Invoking the '{0}' SPARQL query...", queryName);
            string query = string.Format(
                _queryTemplatesService.GetQueryTemplate(queryName),
                graphIri
            );

            return GetTriplesForQuery(query);
        }

        protected abstract HashSet<Triple> GetTriplesForQuery(string query);
    }
}
