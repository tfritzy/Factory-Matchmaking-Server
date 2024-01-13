if (args.Length > 1 && args[1] == "server")
{
    var matchmaker = new Matchmaker();
    await matchmaker.Run("http://localhost:8080/");
}
else
{
    await WebSocketClient.Connect("ws://localhost:8080/");
}
