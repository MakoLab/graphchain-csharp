using elephant.core.persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace elephant.web.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class SparqlController : Controller
    {
        private ILogger _logger;
        private RepositoryManager _repositoryManager;

        public SparqlController(ILogger logger, RepositoryManager repositoryManager)
        {
            _logger = logger;
            _repositoryManager = repositoryManager;
        }

        [HttpPost]
        [Route("query")]
        public string MakeQuery([FromBody] string sparqlQuery)
        {
            _logger.LogDebug("[POST] sparql/query");
            _logger.LogTrace("<sparqlQuery>{0}</ sparqlQuery >", sparqlQuery);
            var result = _repositoryManager.MakeSparqlQuery(sparqlQuery);
            return result;
        }
    }
}