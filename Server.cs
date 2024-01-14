using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class UdpEchoServer
{
    public void Main()
    {
        IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, Constants.Port);
        IPEndPoint? waitingClient = null;

        // Create a new UdpClient for reading incoming data.
        using (UdpClient udpClient = new UdpClient(localEndPoint))
        {
            Console.WriteLine($"Listening for UDP messages on port {Constants.Port}...");

            try
            {
                while (true)
                {
                    // Receive a message from any source.
                    IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    byte[] receivedBytes = udpClient.Receive(ref remoteEndPoint);
                    Console.WriteLine($"Received a message from {remoteEndPoint.Address}:{remoteEndPoint.Port}.");

                    string receivedText = Encoding.UTF8.GetString(receivedBytes);
                    Console.WriteLine($"They said: {receivedText}");

                    if (waitingClient == null)
                    {
                        byte[] message =
                            Encoding.UTF8.GetBytes("Welcome! There's no pair for you currently. Waiting for a fren...");
                        udpClient.Send(message, message.Length, remoteEndPoint);
                        waitingClient = remoteEndPoint;
                    }
                    else if (waitingClient != remoteEndPoint)
                    {
                        byte[] messageFor1 =
                            Encoding.UTF8.GetBytes(
                                $"Connect to:{remoteEndPoint.Address}:{remoteEndPoint.Port}.");
                        byte[] messageFor2 =
                            Encoding.UTF8.GetBytes(
                                $"Connect to:{waitingClient.Address}:{waitingClient.Port}.");
                        udpClient.Send(messageFor1, messageFor1.Length, waitingClient);
                        udpClient.Send(messageFor2, messageFor2.Length, remoteEndPoint);
                        waitingClient = null;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}