using System.Net.WebSockets;

namespace WebSocketApi.WebSockets
{
    public interface IWebSocketHandler
    {
        Task OnConnected(string socketId, WebSocket socket);
        Task OnDisconnected(string socketId, WebSocket socket);
        Task HandleAsync(string socketId, WebSocket socket, string message);
    }
}