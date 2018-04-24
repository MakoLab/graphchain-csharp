using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace elephant.web.WebSockets
{
    public class WebSocketsMiddleware
    {
        private RequestDelegate _next;
        private WebSocketsHandler _webSocketsHandler;

        public WebSocketsMiddleware(RequestDelegate next, WebSocketsHandler webSocketsHandler)
        {
            _next = next;
            _webSocketsHandler = webSocketsHandler;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Path == "/ws")
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    var socket = await context.WebSockets.AcceptWebSocketAsync();
                    _webSocketsHandler.OnConnected(context.Connection.RemoteIpAddress.ToString(), socket);
                    await _webSocketsHandler.Receive(socket);
                }
                else
                {
                    context.Response.StatusCode = 400;
                }
            }
            else
            {
                await _next(context);
            }
        }
    }
}
