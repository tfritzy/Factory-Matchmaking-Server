using System.Net;
using System.Text.Json.Serialization;

public class ClientLookingForHost
{
    public const string MessageType = "ClientLookingForHost";
    public string Type = MessageType;
    public ulong Id;
    public string? Name;
}