using System.Net;

public class WaitingClient
{
    public ulong Id;
    public string Name;
    public IPEndPoint EndPoint;
    public DateTime JoinedAt;

    public WaitingClient(ulong id, string name, IPEndPoint endPoint)
    {
        Id = id;
        Name = name;
        EndPoint = endPoint;
        JoinedAt = DateTime.Now;
    }
}