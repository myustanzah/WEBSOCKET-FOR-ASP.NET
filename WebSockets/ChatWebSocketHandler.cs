using System.Net.WebSockets;
using System.Text;


namespace WebSocketApi.WebSockets;

public class ChatWebSocketHandler : IWebSocketHandler
{
    public Task OnConnected(string socketId, WebSocket socket)
    {
        // Logic to handle a new connection
        Console.WriteLine("New WebSocket connection established.");
        return Task.CompletedTask;
    }

    public Task OnDisconnected(string socketId, WebSocket socket)
    {
        // Logic to handle disconnection
        Console.WriteLine("WebSocket connection closed.");
        return Task.CompletedTask;
    }

    public async Task ReceiveAsync(WebSocket socket, WebSocketReceiveResult result, byte[] buffer)
    {
        if (result.MessageType == WebSocketMessageType.Close)
        {
            await OnDisconnected(socketId, socket);
            await socket.CloseAsync(
                WebSocketCloseStatus.NormalClosure,
                "Connection closed",
                CancellationToken.None);
            return;
        }

        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
        await HandleAsync(socketId, socket, message);
    }

    public async Task HandleAsync(string socketId, WebSocket socket, string message)
    {
        Console.WriteLine($"Received: {message}");

        var response = Encoding.UTF8.GetBytes($"Echo: {message}");
        await socket.SendAsync(
            response,
            WebSocketMessageType.Text,
            true,
            CancellationToken.None);
        
    }
}