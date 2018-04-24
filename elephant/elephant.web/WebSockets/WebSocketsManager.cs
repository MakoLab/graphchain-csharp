using System.Collections.Concurrent;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace elephant.web.WebSockets
{
    public class WebSocketsManager
    {
        private ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();
        private ConcurrentDictionary<string, ClientWebSocket> _clientSockets = new ConcurrentDictionary<string, ClientWebSocket>();

        public WebSocket GetSocketById(string id)
        {
            return _sockets.ContainsKey(id) ? _sockets[id] : null;
        }

        public WebSocket GetClientSocketById(string id)
        {
            return _clientSockets.ContainsKey(id) ? _sockets[id] : null;
        }

        public ConcurrentDictionary<string, WebSocket> GetAll()
        {
            return _sockets;
        }

        public ConcurrentDictionary<string, ClientWebSocket> GetAllClients()
        {
            return _clientSockets;
        }

        public string GetId(WebSocket socket)
        {
            return _sockets.FirstOrDefault(p => p.Value == socket).Key;
        }

        public string GetClientId(ClientWebSocket socket)
        {
            return _clientSockets.FirstOrDefault(p => p.Value == socket).Key;
        }

        public void AddSocket(string id, WebSocket socket)
        {
            _sockets.TryAdd(id, socket);
        }

        public void AddClientSocket(string id, ClientWebSocket socket)
        {
            _clientSockets.TryAdd(id, socket);
        }

        public async Task RemoveSocket(string id)
        {
            _sockets.TryRemove(id, out WebSocket socket);
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by manager", CancellationToken.None);
        }

        public async Task RemoveClientSocket(string id)
        {
            _clientSockets.TryRemove(id, out ClientWebSocket socket);
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed by manager", CancellationToken.None);
        }

        public async Task RemoveAll()
        {
            foreach (var ws in _sockets)
            {
                await RemoveSocket(ws.Key);
            }
        }

        public async Task RemoveAllClients()
        {
            foreach (var ws in _clientSockets)
            {
                await RemoveClientSocket(ws.Key);
            }
        }
    }
}
