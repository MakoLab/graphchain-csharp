using System;
using System.Collections.Generic;
using System.Linq;
using elephant.core.exception;
using elephant.core.model.chain;
using elephant.core.model.p2p;
using elephant.core.model.p2p.message;
using elephant.core.model.rdf;
using elephant.core.ramt;
using elephant.core.service;
using elephant.core.service.base64;
using elephant.core.vocabulary;
using Microsoft.Extensions.Logging;

namespace elephant.web.Services.p2p
{
    public class PeerToPeerMessageHandler
    {
        private ILogger<PeerToPeerMessageHandler> _logger;
        private BlockService _blockService;
        private Base64Handler _base64Handler;
        private BlockContentService _blockContentService;
        private DotNetRdfSerializationHandler _rdf4jSerializationHandler;
        private DotNetRdfMapper _rdf4jMapper;

        public PeerToPeerMessageHandler(ILogger<PeerToPeerMessageHandler> logger, BlockService blockService, Base64Handler base64Handler, BlockContentService blockContentService)
        {
            _logger = logger;
            _blockService = blockService;
            _base64Handler = base64Handler;
            _blockContentService = blockContentService;
            _rdf4jSerializationHandler = new DotNetRdfSerializationHandler();
            _rdf4jMapper = new DotNetRdfMapper();
        }

        public PeerToPeerCommand HandleMessage(PeerToPeerMessage message)
        {
            switch (message.MessageType)
            {
                case MessageType.QUERY_LATEST:
                    return QueryLatestBlock();
                case MessageType.QUERY_ALL:
                    return QueryAllBlocks();
                case MessageType.RESPONSE_BLOCKCHAIN:
                    return ResponseBlockchain(message);
                default:
                    var msg = String.Format("Unknown type of P2P message: {0}", message.MessageType);
                    throw new ArgumentException(msg);
            }
        }

        private PeerToPeerCommand QueryLatestBlock()
        {
            try
            {
                var block = _blockService.GetLatestBlock();
                var message = new PeerToPeerMessage(MessageType.RESPONSE_BLOCKCHAIN, new List<BlockHeader>() { block.BlockHeader }, PrepareGraphData(new List<BlockContent>() { block.BlockContent }));
                return new PeerToPeerCommand(CommandType.WRITE, message);
            }
            catch (ReadingBlockException ex)
            {
                var errorMessage = new ErrorMessage(ex.Message);
                return new PeerToPeerCommand(CommandType.WRITE_ERROR, errorMessage);
            }
        }

        private PeerToPeerCommand QueryAllBlocks()
        {
            try
            {
                var allBlocks = _blockService.GetAllBlocks();
                var allBlockContents = _blockService.GetAllBlockContents();
                Message message = new PeerToPeerMessage(MessageType.RESPONSE_BLOCKCHAIN, allBlocks, PrepareGraphData(allBlockContents));
                return new PeerToPeerCommand(CommandType.WRITE, message);
            }
            catch (ReadingBlockException ex)
            {
                _logger.LogDebug("Exception was thrown: ", ex);
                Message errorMessage = new ErrorMessage(ex.Message);
                return new PeerToPeerCommand(CommandType.WRITE_ERROR, errorMessage);
            }
        }

        private PeerToPeerCommand ResponseBlockchain(PeerToPeerMessage message)
        {
            try
            {
                var latestBlock = _blockService.GetLatestBlock();
                if (message.Data != null && message.Data.Any())
                {
                    var orderedData = message.Data.OrderBy(b => Int32.Parse(b.Index)).ToList();
                    var latestBlockReceived = orderedData[orderedData.Count - 1];
                    if (latestBlockReceived.GetIndexAsInt() > latestBlock.BlockHeader.GetIndexAsInt())
                    {
                        _logger.LogDebug("The index of the received latest block is bigger than the local one.");

                        if (latestBlock.BlockHeader.Hash.Equals(latestBlockReceived.PreviousHash))
                        {
                            _logger.LogDebug("The hash of the latest block is equal to the previous hash of the received block. " + "Adding new block...");
                            try
                            {
                                _blockService.AddBlock(
                                    latestBlockReceived,
                                    ObtainBlockContent(message.Graph[message.Graph.Count - 1]));
                            }
                            catch (RdfSerializationException ex)
                            {
                                _logger.LogWarning("Unable to deserialize received graph as Turtle.", ex);
                            }
                        }
                        else if (orderedData.Count == 1)
                        {
                            _logger.LogDebug("The received chain has only one link. Asking for whole chain.");
                            Message returnMessage = new PeerToPeerMessage(MessageType.QUERY_ALL);
                            return new PeerToPeerCommand(CommandType.BROADCAST, returnMessage);
                        }
                        else
                        {
                            _logger.LogDebug("Replacing the whole blockchain.");
                            try
                            {
                                _blockService.ReplaceBlockchain(orderedData, ObtainBlockContents(message.Graph));
                            }
                            catch (RdfSerializationException ex)
                            {
                                _logger.LogDebug("Exception was thrown while serialization RDF graph.", ex);
                                Message returnMessage = new PeerToPeerMessage(MessageType.ERROR);
                                return new PeerToPeerCommand(CommandType.WRITE_ERROR, returnMessage);
                            }
                            catch (CreatingBlockException ex)
                            {
                                _logger.LogDebug("Exception was thrown while creating blocks.", ex);
                                Message returnMessage = new PeerToPeerMessage(MessageType.ERROR);
                                return new PeerToPeerCommand(CommandType.WRITE_ERROR, returnMessage);
                            }
                        }
                    }
                    else
                    {
                        _logger.LogDebug("The index of the received latest block is not bigger than the local one. Ignoring.");
                    }
                }
            }
            catch (ReadingBlockException ex)
            {
                _logger.LogWarning("Exception was thrown while reading block.", ex);
            }
            return new PeerToPeerCommand(CommandType.DO_NOTHING);
        }

        private List<GraphData> PrepareGraphData(List<BlockContent> blockContents)
        {
            var result = new List<GraphData>();
            foreach (var blockContent in blockContents)
            {
                result.Add(new GraphData(blockContent.DataGraphIri, _blockContentService.CalculateBase64(blockContent)));
            }
            return result;
        }

        private BlockContent ObtainBlockContent(GraphData graphData)
        {
            HashSet<Triple> triples = GetTriplesFromBase64(graphData.graphContent);
            return new BlockContent(graphData.graphIri, triples);
        }

        private List<BlockContent> ObtainBlockContents(List<GraphData> graphs)
        {
            var result = new List<BlockContent>();
            foreach (var graphData in graphs)
            {
                result.Add(ObtainBlockContent(graphData));
            }
            return result;
        }

        private HashSet<Triple> GetTriplesFromBase64(String graphContentAsBase64)
        {
            var graphContentAsTurtle = _base64Handler.ToNormalString(graphContentAsBase64);
            var model = _rdf4jSerializationHandler.DeserializeGraph(graphContentAsTurtle, RdfFormat.TURTLE);
            return _rdf4jMapper.GraphToTriples(model);
        }
    }
}
