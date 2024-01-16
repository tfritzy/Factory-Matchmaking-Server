using System.Net;
using Newtonsoft.Json;

namespace ServerTests;


[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void TestMethod1()
    {
        TestClient testClient = new TestClient();
        Server server = new Server();
        string strMessage = JsonConvert.SerializeObject(new ClientLookingForHost()
        {
            Id = 18,
            Name = "TestClient",
        });
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(strMessage);
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Parse("192.168.0.3"), 1234);

        Assert.AreEqual(0, server.waitingClients.Count);
        server.HandleMessage(remoteEndPoint, bytes, testClient);
        Assert.AreEqual(1, server.waitingClients.Count);

        WaitingClient waitingClient = server.waitingClients.Dequeue();
        Assert.AreEqual(18u, waitingClient.Id);
        Assert.AreEqual("TestClient", waitingClient.Name);
        Assert.AreEqual(remoteEndPoint, waitingClient.EndPoint);
        Assert.AreEqual(
            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            waitingClient.JoinedAt.ToString("yyyy-MM-dd HH:mm:ss"));
    }
}