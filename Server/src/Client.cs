using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

public class Client : IClient
{
    private readonly UdpClient client;

    public Client(IPEndPoint endpoint) : base()
    {
        client = new UdpClient(endpoint);
    }

    public Task<UdpReceiveResult> ReceiveAsync()
    {
        return client.ReceiveAsync();
    }

    public Task<UdpReceiveResult> ReceiveAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public int Send(byte[] dgram, int bytes, IPEndPoint? endPoint)
    {
        return client.Send(dgram, bytes, endPoint);
    }
}