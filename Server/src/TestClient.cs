using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

public class TestClient : IClient
{
    public Dictionary<IPEndPoint, List<byte[]>> SentMessages = new();
    public readonly Queue<UdpReceiveResult> ReceivedMessages = new();
    private readonly SemaphoreSlim _messageAvailable = new(0);

    public TestClient() : base()
    {
    }

    public async Task<UdpReceiveResult> ReceiveAsync(CancellationToken token)
    {
        // Wait until a message is available
        await _messageAvailable.WaitAsync(token);

        lock (ReceivedMessages)
        {
            if (ReceivedMessages.TryDequeue(out var result))
            {
                return result;
            }
        }

        return default;
    }

    public void EnqueueReceivedMessage(UdpReceiveResult message)
    {
        lock (ReceivedMessages)
        {
            ReceivedMessages.Enqueue(message);
            _messageAvailable.Release();
        }
    }

    public int Send(byte[] dgram, int bytes, IPEndPoint? endPoint)
    {
        if (endPoint == null)
        {
            throw new System.ArgumentNullException(nameof(endPoint));
        }

        if (!SentMessages.ContainsKey(endPoint))
        {
            SentMessages.Add(endPoint, new List<byte[]>());
        }

        SentMessages[endPoint].Add(dgram);

        return bytes;
    }
}