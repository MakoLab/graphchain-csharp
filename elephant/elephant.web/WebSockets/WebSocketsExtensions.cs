using Microsoft.AspNetCore.Builder;

namespace elephant.web.WebSockets
{
    public static class WebSocketsExtensions
    {
        public static IApplicationBuilder UseWebSocketsMiddleware(this IApplicationBuilder builder, WebSocketsHandler handler)
        {
            return builder.UseMiddleware<WebSocketsMiddleware>(handler);
        }
    }
}
