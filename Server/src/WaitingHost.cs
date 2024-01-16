using System.Net;

public class WaitingHost
{
    public IPEndPoint EndPoint;
    public DateTime JoinedAt;
    public WaitingHost(IPEndPoint endPoint)
    {
        EndPoint = endPoint;
    }
}