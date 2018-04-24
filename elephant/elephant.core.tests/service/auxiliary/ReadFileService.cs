using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Logging;

namespace elephant.core.tests.service.auxiliary
{
    public class ReadFileService
    {
        private ILogger<ReadFileService> _logger;

        public ReadFileService(ILogger<ReadFileService> logger)
        {
            _logger = logger;
        }

        public Dictionary<string, string> ReadFilesContent(string filePath, int size)
        {
            _logger.LogDebug("Reading files content from the '{0}' path.", filePath);

            var result = new Dictionary<string, string>();
            foreach (string fileName in Directory.EnumerateFiles(filePath + size + "\\"))
            {
                string graphIri = ObtainGraphIri(fileName);
                _logger.LogDebug("Reading graph '{0}' from file '{1}'...", graphIri, fileName);
                string fileContent = File.ReadAllText(fileName, System.Text.Encoding.UTF8);
                result.Add(graphIri, fileContent);
            }
            return result;
        }

        private string ObtainGraphIri(string fileName)
        {
            return "http://lei.info/" + Path.GetFileNameWithoutExtension(fileName);
        }
    }
}
