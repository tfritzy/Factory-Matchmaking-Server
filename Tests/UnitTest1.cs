using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace ServerTests;


[TestClass]
public class ServerTests
{
    [TestMethod]
    public void AddsClientsToWaitlist()
    {
        TestClient testClient = new TestClient();
        Server server = new Server();
        string strMessage = JsonConvert.SerializeObject(new ClientLookingForHost());
        byte[] bytes = Encoding.UTF8.GetBytes(strMessage);
        IPEndPoint clientEndpoint = new IPEndPoint(IPAddress.Parse("192.168.0.3"), 1234);

        Assert.AreEqual(0, server.waitingClients.Count);
        server.HandleMessage(clientEndpoint, bytes, testClient);
        Assert.AreEqual(1, server.waitingClients.Count);

        WaitingClient waitingClient = server.waitingClients.Dequeue();
        Assert.AreEqual(clientEndpoint, waitingClient.EndPoint);
        Assert.AreEqual(
            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            waitingClient.JoinedAt.ToString("yyyy-MM-dd HH:mm:ss"));
        Assert.AreEqual(1, testClient.SentMessages[clientEndpoint].Count);

        var message = testClient.SentMessages[clientEndpoint][0];
        ClientAck? deser = JsonConvert.DeserializeObject<ClientAck>(Encoding.UTF8.GetString(message));
        Assert.AreEqual(ClientAck.MessageType, deser?.Type);
    }

    [TestMethod]
    public void AddsHostsToWaitlist()
    {
        TestClient testClient = new TestClient();
        Server server = new Server();
        string strMessage = JsonConvert.SerializeObject(new HostCreatingGame());
        byte[] bytes = Encoding.UTF8.GetBytes(strMessage);
        IPEndPoint hostEndpoint = new IPEndPoint(IPAddress.Parse("192.168.0.3"), 1234);

        Assert.AreEqual(0, server.waitingHosts.Count);
        server.HandleMessage(hostEndpoint, bytes, testClient);
        Assert.AreEqual(1, server.waitingHosts.Count);

        WaitingHost waitingHost = server.waitingHosts.Dequeue();
        Assert.AreEqual(hostEndpoint, waitingHost.EndPoint);
        Assert.AreEqual(
            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            waitingHost.JoinedAt.ToString("yyyy-MM-dd HH:mm:ss"));
        Assert.AreEqual(1, testClient.SentMessages[hostEndpoint].Count);

        var message = testClient.SentMessages[hostEndpoint][0];
        HostAck? deser = JsonConvert.DeserializeObject<HostAck>(Encoding.UTF8.GetString(message));
        Assert.AreEqual(HostAck.MessageType, deser?.Type);
    }

    [TestMethod]
    public void TellsHostsAndClientsAboutEachOther()
    {
        TestClient testClient = new TestClient();
        Server server = new Server();

        IPEndPoint clientEndpoint = new IPEndPoint(IPAddress.Parse("192.168.0.3"), 1234);
        IPEndPoint hostEndpoint = new IPEndPoint(IPAddress.Parse("192.168.0.1"), 4321);

        byte[] clientMessage = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new ClientLookingForHost()));
        server.HandleMessage(clientEndpoint, clientMessage, testClient);

        byte[] hostMessage = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new HostCreatingGame()));
        server.HandleMessage(hostEndpoint, hostMessage, testClient);

        Assert.AreEqual(2, testClient.SentMessages[clientEndpoint].Count);
        Assert.AreEqual(2, testClient.SentMessages[hostEndpoint].Count);

        var clientConnectionMessage = testClient.SentMessages[clientEndpoint][1];
        var hostConnectionMessage = testClient.SentMessages[hostEndpoint][1];

        InformOfPeer? informOfHost =
            JsonConvert.DeserializeObject<InformOfPeer>(
                Encoding.UTF8.GetString(clientConnectionMessage));
        Assert.AreEqual(InformOfPeer.MessageType, informOfHost?.Type);
        Assert.AreEqual(hostEndpoint.Address.ToString(), informOfHost?.IpAddress);
        Assert.AreEqual(hostEndpoint.Port, informOfHost?.Port);

        InformOfPeer? informOfClient =
            JsonConvert.DeserializeObject<InformOfPeer>(
                Encoding.UTF8.GetString(hostConnectionMessage));
        Assert.AreEqual(InformOfPeer.MessageType, informOfClient?.Type);
        Assert.AreEqual(clientEndpoint.Address.ToString(), informOfClient?.IpAddress);
        Assert.AreEqual(clientEndpoint.Port, informOfClient?.Port);
    }
}