if (args.Length > 0 && args[0] == "client")
{
    Console.WriteLine("Starting client...");
    var client = new Client();
    client.Run();
}
else
{
    Console.WriteLine("Starting server...");
    Server server = new Server();
    server.Run();
}