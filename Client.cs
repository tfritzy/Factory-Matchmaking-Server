using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class UdpEchoClient
{
    public void Main()
    {
        using (UdpClient udpClient = new UdpClient())
        {
            try
            {
                // Send a message to the server.
                string message = "Hello, Server!";
                byte[] bytesToSend = Encoding.UTF8.GetBytes(message);

                // Send the message.
                Console.WriteLine($"Sending: {message}");
                udpClient.Send(bytesToSend, bytesToSend.Length, Constants.ServerIP, Constants.Port);

                // Receive the server's response (which should be an echo).
                IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

                while (true)
                {
                    byte[] receivedBytes = udpClient.Receive(ref remoteEndPoint);

                    // Convert bytes to string.
                    string receivedText = Encoding.UTF8.GetString(receivedBytes);
                    Console.WriteLine($"Received: {receivedText}");

                    // Check if the response is the one we're waiting for.
                    if (receivedText.Contains("Connect to:"))
                    {
                        var address = receivedText.Split("Connect to:")[1];
                        Console.WriteLine($"Connecting to peer: {address}...");

                        byte[] helloPeer = Encoding.UTF8.GetBytes("Hello, Peer! I'm your friend.");

                        var ip = address.Split(':')[0];
                        var port = int.Parse(address.Split(':')[1]);
                        udpClient.Send(helloPeer, helloPeer.Length, ip, port);
                        break;
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