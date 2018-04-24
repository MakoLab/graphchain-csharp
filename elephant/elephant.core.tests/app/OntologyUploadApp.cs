using System.IO;
using elephant.core.service;
using elephant.core.vocabulary;

namespace elephant.core.tests.app
{
    public class OntologyUploadApp
    {
        private BlockService _blockService;

        public OntologyUploadApp(BlockService blockService)
        {
            _blockService = blockService;
        }

        public void Run()
        {
            var rawRdf = File.ReadAllText("c:\\data\\ontologies\\go.nt");
            _blockService.CreateBlock("http://go.obo.org/", rawRdf, RdfFormat.N_TRIPLES);
        }
    }
}
