using System.Net;

namespace DotDns.AspNetCore
{
    public class UdpEndpoint : IPEndPoint
    {
        public UdpEndpoint(long address, int port) : base(address, port)
        {
        }

        public UdpEndpoint(IPAddress address, int port) : base(address, port)
        {
        }
    }
}