using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class Server
{
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
                        remoteEndPoint);
                waitingClients.Enqueue(waitingClient);
                var clientAck = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new ClientAck()));
                client.Send(clientAck, clientAck.Length, remoteEndPoint);
            }
        }
        else if (type == HostCreatingGame.MessageType)
        {
            HostCreatingGame? hostCreatingGame =
                JsonConvert.DeserializeObject<HostCreatingGame>(receivedText);

            if (hostCreatingGame != null)
            {
                byte[] response = Encoding.UTF8.GetBytes(
                    JsonConvert.SerializeObject(new HostAck()));
                client.Send(response, response.Length, remoteEndPoint);

                var waitingHost = new WaitingHost(remoteEndPoint);
                waitingHosts.Enqueue(waitingHost);
            }
        }

        if (waitingClients.Count > 0 && waitingHosts.Count > 0)
        {
            WaitingClient waitingClient = waitingClients.Dequeue();
            WaitingHost waitingHost = waitingHosts.Dequeue();

            var informOfPeer = new InformOfPeer(waitingHost.EndPoint.Address.ToString(), waitingHost.EndPoint.Port);
            byte[] informOfPeerBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(informOfPeer));
            client.Send(informOfPeerBytes, informOfPeerBytes.Length, waitingClient.EndPoint);

            informOfPeer = new InformOfPeer(waitingClient.EndPoint.Address.ToString(), waitingClient.EndPoint.Port);
            informOfPeerBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(informOfPeer));
            client.Send(informOfPeerBytes, informOfPeerBytes.Length, waitingHost.EndPoint);
        }
    }
}