using System;
using elephant.core.model.chain;
using elephant.core.ramt;
using elephant.core.service.base64;
using elephant.core.vocabulary;

namespace elephant.core.service
{
    public class BlockContentService
    {
        private Base64Handler _base64Handler;
        private DotNetRdfMapper _rdf4jMapper;

        public BlockContentService(Base64Handler base64Handler)
        {
            _base64Handler = base64Handler;
            _rdf4jMapper = new DotNetRdfMapper();
        }

        public String CalculateBase64(BlockContent blockContent)
        {
            String serializedModel = _rdf4jMapper.TriplesToSerialization(blockContent.Triples, RdfFormat.TURTLE);
            return _base64Handler.ToBase64(serializedModel);
        }
    }
}
