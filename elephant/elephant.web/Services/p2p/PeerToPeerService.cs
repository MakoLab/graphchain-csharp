using System.Collections.Generic;
using elephant.core.exception;
using elephant.core.model.chain;
using elephant.core.model.p2p;
using elephant.core.model.p2p.message;
using elephant.core.service;
using elephant.web.WebSockets;
using Microsoft.Extensions.Logging;

namespace elephant.web.Services.p2p
{
    public class PeerToPeerService
    {
        private ILogger<PeerToPeerService> _logger;
        private PeerToPeerMessageHandler _peerToPeerMessageHandler;
        private BlockContentService _blockContentService;
        private WebSocketsHandler _webSocketsHandler;

        public PeerToPeerService(ILogger<PeerToPeerService> logger, PeerToPeerMessageHandler peerToPeerMessageHandler, BlockContentService blockContentService, WebSocketsHandler webSocketsHandler)
        {
            _logger = logger;
            _peerToPeerMessageHandler = peerToPeerMessageHandler;
            _blockContentService = blockContentService;
            _webSocketsHandler = webSocketsHandler;
        }

        public void CloseAll()
        {
            _webSocketsHandler.CloseAll().Wait();
        }

        public bool AddPeer(string peerAddress)
        {
            try
            {
                _webSocketsHandler.AddPeer(peerAddress).Wait();
                return true;
            }
            catch (PeerToPeerConnectionException ex)
            {
                _logger.LogDebug(ex, "Unable to connect to a peer.");
                return false;
            }
        }

        public List<string> GetAllPeers()
        {
            return _webSocketsHandler.GetAll();
        }

        public PeerToPeerCommand HandleMessage(PeerToPeerMessage message)
        {
            return _peerToPeerMessageHandler.HandleMessage(message);
        }

        public void Broadcast(MessageType messageType)
        {
            PeerToPeerMessage message = new PeerToPeerMessage(messageType);
            var command = _peerToPeerMessageHandler.HandleMessage(message);
            var strMessage = Newtonsoft.Json.JsonConvert.SerializeObject(message);
            _webSocketsHandler.SendMessageToAllAsync(strMessage).Wait();
        }

        public void BroadcastBlockCreation(MessageType messageType, Block block)
        {
            var blockContent = _blockContentService.CalculateBase64(block.BlockContent);
            PeerToPeerMessage message = new PeerToPeerMessage(messageType, new List<BlockHeader>() { block.BlockHeader }, new List<GraphData>() { new GraphData(block.BlockHeader.DataGraphIri, blockContent) });
            var strMessage = Newtonsoft.Json.JsonConvert.SerializeObject(message);
            _webSocketsHandler.SendMessageToAllAsync(strMessage).Wait();
        }
    }
}
