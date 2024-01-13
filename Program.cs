if (args.Length > 1 && args[1] == "server")
{
    var matchmaker = new Matchmaker();
    await matchmaker.Run("https://localhost:64132/");
}
else
{
    await WebSocketClient.Connect("wss://50.46.242.76:64132/");
}
