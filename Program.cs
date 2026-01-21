using WebSocketApi.WebSockets;
using WebSocketManager = WebSocketApi.WebSockets.WebSocketManager;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IWebSocketHandler,ChatWebSocketHandler>();
builder.Services.AddSingleton<WebSocketManager>();
var app = builder.Build();

app.UseWhen(
    context => context.Request.Path.StartsWithSegments("/ws"),
    appBuilder =>
    {
        appBuilder.UseWebSockets();
        appBuilder.UseMiddleware<WebSocketApi.Middlewares.WebSocketMiddleware>();
    });

app.Run();
