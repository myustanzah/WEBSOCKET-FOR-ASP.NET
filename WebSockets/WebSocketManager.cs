using System.Collections.Concurrent;
using System.Net.WebSockets;

namespace WebSocketApi.WebSockets;

public class WebSocketManager
{
    private ConcurrentDictionary<string, WebSocket> _sockets = new();

    public string AddSocket(WebSocket socket)
    {
        var socketId = Guid.NewGuid().ToString();
        _sockets.TryAdd(socketId, socket);
        return socketId;
    }

    public async Task RemoveSocket(string socketId)
    {
        if (_sockets.TryRemove(socketId, out var socket))
        {
            if (socket.State == WebSocketState.Open)
            {
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed", CancellationToken.None);  
            }
        }
    }

    public IReadOnlyDictionary<string, WebSocket> GetAll()
        => _sockets;

    public WebSocket? GetById(string socketId)
        => _sockets.TryGetValue(socketId, out var socket) ? socket : null;
}