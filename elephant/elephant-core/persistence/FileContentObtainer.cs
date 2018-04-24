using System.IO;
using Microsoft.Extensions.Logging;

namespace elephant.core.persistence
{
    public class FileContentObtainer
    {
        private ILogger<FileContentObtainer> _logger;

        public FileContentObtainer(ILogger<FileContentObtainer> logger)
        {
            _logger = logger;
        }

        public string GetFileContent(string filePath)
        {
            try
            {
                return File.ReadAllText(filePath);
            }
            catch (IOException ex)
            {
                _logger.LogWarning(ex, "Exception was thrown while reading query template from a file.");
            }
            return null;
        }
    }
}
