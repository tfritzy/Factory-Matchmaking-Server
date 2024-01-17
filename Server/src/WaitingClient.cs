using System.Net;

public class WaitingClient
{
    public IPEndPoint EndPoint;
    public DateTime JoinedAt;

    public WaitingClient(IPEndPoint endPoint)
    {
        EndPoint = endPoint;
        JoinedAt = DateTime.Now;
    }
}