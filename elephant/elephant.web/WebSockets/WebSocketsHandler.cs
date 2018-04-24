using System;
using System.Collections.Generic;
using System.IO;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using elephant.core.model.p2p;
using elephant.core.model.p2p.message;
using elephant.web.Services.p2p;
using Microsoft.Extensions.Logging;

namespace elephant.web.WebSockets
{
    public class WebSocketsHandler
    {
        private ILogger<WebSocketsHandler> _logger;
        private WebSocketsManager _webSocketsManager;
        private PeerToPeerMessageHandler _peerToPeerMessageHandler;

        public WebSocketsHandler(ILogger<WebSocketsHandler> logger, WebSocketsManager webSocketsManager, PeerToPeerMessageHandler peerToPeerMessageHandler)
        {
            _logger = logger;
            _webSocketsManager = webSocketsManager;
            _peerToPeerMessageHandler = peerToPeerMessageHandler;
        }

        public void OnConnected(string id, WebSocket socket)
        {
            _webSocketsManager.AddSocket(id, socket);
        }

        public async Task OnDisconnected(WebSocket socket)
        {
            await _webSocketsManager.RemoveSocket(_webSocketsManager.GetId(socket));
        }

        public async Task CloseAll()
        {
            await _webSocketsManager.RemoveAll();
        }

        public List<string> GetAll()
        {
            return new List<string>(_webSocketsManager.GetAll().Keys);
        }

        public async Task AddPeer(string url)
        {
            var ws = new ClientWebSocket();
            await ws.ConnectAsync(new Uri(url), CancellationToken.None);
            _webSocketsManager.AddSocket(url, ws);
            var t = Receive(ws);
        }

        public async Task SendMessageAsync(WebSocket socket, string message)
        {
            if (socket.State != WebSocketState.Open)
                return;
            await socket.SendAsync(buffer: new ArraySegment<byte>(array: Encoding.ASCII.GetBytes(message),
                                                                  offset: 0,
                                                                  count: message.Length),
                                   messageType: WebSocketMessageType.Text,
                                   endOfMessage: true,
                                   cancellationToken: CancellationToken.None);
        }

        public async Task SendMessageAsync(string socketId, string message)
        {
            await SendMessageAsync(_webSocketsManager.GetSocketById(socketId), message);
        }

        public async Task SendMessageToAllAsync(string message)
        {
            //foreach (var pair in _webSocketsManager.GetAllClients())
            //{
            //    if (pair.Value.State == WebSocketState.Open)
            //        await SendMessageAsync(pair.Value, message);
            //}
            foreach (var pair in _webSocketsManager.GetAll())
            {
                if (pair.Value.State == WebSocketState.Open)
                    await SendMessageAsync(pair.Value, message);
            }
        }

        public async Task Receive(WebSocket socket)
        {
            var buffer = new ArraySegment<byte>(new byte[1024 * 4]);
            while (socket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result;
                using (var ms = new MemoryStream())
                {
                    do
                    {
                        result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                        ms.Write(buffer.Array, buffer.Offset, result.Count);
                    }
                    while (!result.EndOfMessage);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await OnDisconnected(socket);
                    }
                    else if (result.MessageType == WebSocketMessageType.Text)
                    {
                        ms.Seek(0, SeekOrigin.Begin);
                        using (var reader = new StreamReader(ms, Encoding.UTF8))
                        {
                            await ProcessMessage(socket, reader.ReadToEnd());
                        }
                    }
                }
            }
        }

        public async Task ProcessMessage(WebSocket socket, string messageAsString)
        {
            try
            {
                _logger.LogDebug("Received new message from peer.");
                PeerToPeerMessage message = Newtonsoft.Json.JsonConvert.DeserializeObject<PeerToPeerMessage>(messageAsString);
                _logger.LogDebug("Handling message from peer: {0}", message);
                PeerToPeerCommand command = _peerToPeerMessageHandler.HandleMessage(message);
                _logger.LogDebug("Handling command with type '{0}' and message type '{1}'.", command.CommandType, command.GetMessageType());
                switch (command.CommandType)
                {
                    case CommandType.WRITE:
                        await SendMessageAsync(socket, Newtonsoft.Json.JsonConvert.SerializeObject(command.Message));
                        break;
                    case CommandType.WRITE_ERROR:
                        await SendMessageAsync(socket, Newtonsoft.Json.JsonConvert.SerializeObject(command.Message));
                        break;
                    case CommandType.BROADCAST:
                        await SendMessageAsync(socket, Newtonsoft.Json.JsonConvert.SerializeObject(command.Message));
                        break;
                    case CommandType.DO_NOTHING:
                        _logger.LogDebug("Doing nothing.");
                        break;
                    default:
                        _logger.LogWarning("Unknown command type '{0}'.", command.CommandType);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in message processing.");
            }
        }
    }
}
