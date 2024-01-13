using System.Net;
using System.Net.WebSockets;

class Matchmaker
{
    public async Task Run(string uri)
    {
        HttpListener httpListener = new HttpListener();
        httpListener.Prefixes.Add(uri);
        httpListener.Start();
        Console.WriteLine($"WebSocket Server started at {uri}");

        while (true)
        {
            HttpListenerContext httpContext = await httpListener.GetContextAsync();

            if (httpContext.Request.IsWebSocketRequest)
            {
                HttpListenerWebSocketContext webSocketContext = await httpContext.AcceptWebSocketAsync(null);
                WebSocket webSocket = webSocketContext.WebSocket;

                // Handle the WebSocket connection here
                await HandleConnection(webSocket);
            }
            else
            {
                httpContext.Response.StatusCode = 400;
                httpContext.Response.Close();
            }
        }
    }

    private static async Task HandleConnection(WebSocket webSocket)
    {
        byte[] buffer = new byte[1024];
        while (webSocket.State == WebSocketState.Open)
        {
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            // Handle incoming messages here
            Console.WriteLine($"Received message: {System.Text.Encoding.UTF8.GetString(buffer, 0, result.Count)}");

            // Send a response (optional)
            string response = "Message received";
            await webSocket.SendAsync(
                new ArraySegment<byte>(System.Text.Encoding.UTF8.GetBytes(response)),
                result.MessageType,
                result.EndOfMessage,
                CancellationToken.None);
        }
    }


}