if (args.Length > 1 && args[1] == "server")
{
    var matchmaker = new Matchmaker();
    await matchmaker.Run("http://20.118.203.253:64132/");
}
else
{
    await WebSocketClient.Connect("ws://20.118.203.253:64132/");
}
