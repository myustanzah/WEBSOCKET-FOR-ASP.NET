using System.Net.WebSockets;

namespace WebSocketApi.WebSockets;

public static class WebSocketExtensions
{
    public static void MapWebSocket<T>(this WebApplication app, string path) where T : IWebSocketHandler
    {
        app.Map(path, async context =>
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                var handler = Activator.CreateInstance<T>();
                var buffer = new byte[1024 * 4];

                while (webSocket.State == WebSocketState.Open)
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                    }
                    else
                    {
                        var message = System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count);
                        await handler.HandleAsync(webSocket, message);
                    }
                }
            }
            else
            {
                context.Response.StatusCode = 400;
                return;
            }
        });
    }
}