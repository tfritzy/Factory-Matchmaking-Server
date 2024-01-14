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
                byte[] receivedBytes = udpClient.Receive(ref remoteEndPoint);

                // Convert bytes to string.
                string receivedText = Encoding.UTF8.GetString(receivedBytes);
                Console.WriteLine($"Received: {receivedText}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}