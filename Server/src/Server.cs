using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class Server
{
    public const string HostAck = "AckHost";
    public Queue<WaitingClient> waitingClients = new Queue<WaitingClient>();
    public Queue<WaitingHost> waitingHosts = new Queue<WaitingHost>();

    public async Task Run()
    {
        IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, Constants.Port);
        IClient udpClient = new Client(localEndPoint);

        Console.WriteLine($"Listening for UDP messages on port {Constants.Port}...");

        try
        {
            while (true)
            {
                // Receive a message from any source.
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                UdpReceiveResult result = await udpClient.ReceiveAsync(CancellationToken.None);
                Console.WriteLine($"Received a message from {remoteEndPoint.Address}:{remoteEndPoint.Port}.");
                HandleMessage(remoteEndPoint, result.Buffer, udpClient);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    public void HandleMessage(IPEndPoint remoteEndPoint, byte[] bytes, IClient client)
    {
        string receivedText = Encoding.UTF8.GetString(bytes);
        Console.WriteLine($"They said: {receivedText}");

        JObject? json = JObject.Parse(receivedText);
        string type = json?["Type"]?.ToString() ?? "";

        if (type == ClientLookingForHost.MessageType)
        {
            ClientLookingForHost? clientLookingForHost =
                JsonConvert.DeserializeObject<ClientLookingForHost>(receivedText);
            if (clientLookingForHost != null)
            {
                var waitingClient =
                    new WaitingClient(
                        clientLookingForHost.Id,
                        clientLookingForHost.Name,
                        remoteEndPoint);
                Console.WriteLine($"Player {waitingClient.Name} has joined the queue.");
                waitingClients.Enqueue(waitingClient);
            }
        }
        else if (type == HostCreatingGame.Type)
        {
            HostCreatingGame? hostCreatingGame =
                JsonConvert.DeserializeObject<HostCreatingGame>(receivedText);

            if (hostCreatingGame != null)
            {
                byte[] response = Encoding.UTF8.GetBytes(HostAck);
                client.Send(response, response.Length, remoteEndPoint);

                var waitingHost = new WaitingHost(remoteEndPoint);
                waitingHosts.Enqueue(waitingHost);
            }
        }
    }
}