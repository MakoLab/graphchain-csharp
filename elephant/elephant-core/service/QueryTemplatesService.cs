using System;
using elephant.core.Properties;
using Microsoft.Extensions.Logging;

namespace elephant.core.service
{
    public class QueryTemplatesService
    {
        private ILogger<QueryTemplatesService> _logger;

        public QueryTemplatesService(ILogger<QueryTemplatesService> logger)
        {
            _logger = logger;
        }

        public string GetQueryTemplate(string queryName)
        {
            _logger.LogTrace("Trying to get a query template with name '{0}'.", queryName);
            try
            {
                return Resources.ResourceManager.GetString(queryName);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Exception was thrown while reading query template from a file.");
            }
            return null;
        }
    }
}
