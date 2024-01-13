using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

class WebSocketClient
{
    public static async Task Connect(string uri)
    {
        ClientWebSocket client = new ClientWebSocket();
        try
        {
            await client.ConnectAsync(new Uri(uri), CancellationToken.None);
            Console.WriteLine($"Connected to {uri}");

            await Send(client, "Hello from the client!");

            await Receive(client);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            client.Dispose();
            Console.WriteLine("WebSocket closed.");
        }
    }

    private static async Task Send(ClientWebSocket client, string message)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(message);
        await client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
        Console.WriteLine("Message sent to the server");
    }

    private static async Task Receive(ClientWebSocket client)
    {
        byte[] buffer = new byte[1024];
        WebSocketReceiveResult result = await client.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        string receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
        Console.WriteLine($"Message received from the server: {receivedMessage}");
    }
}