using System.Text.Json.Serialization;

public class HostAck
{
    [JsonPropertyName("type")]
    public const string Type = "HostAck";
}