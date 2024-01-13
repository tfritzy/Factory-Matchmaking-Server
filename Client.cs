using System.Collections;
using LiteNetLib;

public class Client
{
    public void Run()
    {
        EventBasedNetListener listener = new EventBasedNetListener();
        NetManager client = new NetManager(listener);
        client.Start();
        client.Connect("localhost" /* host ip or name */, 9050 /* port */, "SomeConnectionKey" /* text key or NetDataWriter */);
        listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod, channel) =>
        {
            Console.WriteLine("We got: {0}", dataReader.GetString(100 /* max length of string */));
            dataReader.Recycle();
        };

        while (!Console.KeyAvailable)
        {
            client.PollEvents();
            Thread.Sleep(15);
        }

        client.Stop();
    }
}