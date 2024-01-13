using UDP;

UDPSocket s = new UDPSocket();
s.Server("127.0.0.1", 64132);

// UDPSocket c = new UDPSocket();
// c.Client("20.29.48.111", 64132);
// c.Send("TEST!");

Console.ReadKey();