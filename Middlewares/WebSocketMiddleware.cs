using System.Net.WebSockets;
using System.Text;
using WebSocketApi.WebSockets;
using WebSocketManager = WebSocketApi.WebSockets.WebSocketManager;

namespace WebSocketApi.Middlewares;

    public class WebSocketMiddleware
    {
        private readonly RequestDelegate _next;

        public WebSocketMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ChatWebSocketHandler handler, WebSocketManager webSocketManager)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                 await _next(context);
                return;
            }
            using var socket = await context.WebSockets.AcceptWebSocketAsync();
            var socketId = webSocketManager.AddSocket(socket);
            await handler.OnConnected(socketId, socket);

            var buffer = new byte[1024 * 4];
            try
            {
                while (socket.State == WebSocketState.Open)
                {
                    var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        break;
                    }

                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    await handler.HandleAsync(socketId, socket, message);
                }
            }
            finally
            {
                await handler.OnDisconnected(socketId, socket);
                if(socket.State != WebSocketState.Closed)
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                }

            }
    }
}