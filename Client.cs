using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class NatPunchthroughClient
{
    private UdpClient udpClient;
    private IPEndPoint otherClientEndpoint;

    public NatPunchthroughClient(string otherClientIP, int otherClientPort)
    {
        udpClient = new UdpClient();
        otherClientEndpoint = new IPEndPoint(IPAddress.Parse(otherClientIP), otherClientPort);
    }

    public void StartPunchthroughProcess()
    {
        Task.Run(() => SendMessagesAsync());
        ReceiveMessagesAsync();
    }

    private async Task SendMessagesAsync()
    {
        try
        {
            while (true) // Or some condition to stop sending
            {
                byte[] message = Encoding.ASCII.GetBytes("Hello from client");
                Console.WriteLine("Sending message");
                await udpClient.SendAsync(message, message.Length, otherClientEndpoint);
                await Task.Delay(1000); // Send every second, adjust as needed
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions
        }
    }

    private void ReceiveMessagesAsync()
    {
        try
        {
            while (true) // Or some condition to stop receiving
            {
                var result = udpClient.ReceiveAsync().Result;
                string receivedData = Encoding.ASCII.GetString(result.Buffer);
                Console.WriteLine($"Received data: {receivedData}");
                // Process received data
            }
        }
        catch (Exception ex)
        {
            // Handle exceptions
        }
    }
}
