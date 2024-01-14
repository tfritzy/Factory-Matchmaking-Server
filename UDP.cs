using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class UdpEchoServer
{
    public void Main()
    {
        // Define the local endpoint (where messages are received).
        int listenPort = 64132;
        IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, listenPort);

        // Create a new UdpClient for reading incoming data.
        using (UdpClient udpClient = new UdpClient(localEndPoint))
        {
            Console.WriteLine($"Listening for UDP messages on port {listenPort}...");

            try
            {
                while (true)
                {
                    // Receive a message from any source.
                    IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    byte[] receivedBytes = udpClient.Receive(ref remoteEndPoint);

                    // Convert bytes to string.
                    string receivedText = Encoding.UTF8.GetString(receivedBytes);
                    Console.WriteLine($"Received: {receivedText}");

                    // Echo the message back to the sender.
                    udpClient.Send(receivedBytes, receivedBytes.Length, remoteEndPoint);
                    Console.WriteLine($"Echoed back to {remoteEndPoint}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}