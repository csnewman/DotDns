using System.Net;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace DotDns.AspNetCore
{
    public static class DnsKestrelExtensions
    {
        public static void ListenDnsUdp(this KestrelServerOptions options, UdpEndpoint endpoint)
        {
            options.Listen(endpoint, listenOptions => listenOptions.UseConnectionHandler<DnsUdpConnectionHandler>());
        }

        public static void ListenDnsTcp(this KestrelServerOptions options, IPEndPoint endpoint)
        {
            options.Listen(endpoint, listenOptions => listenOptions.UseConnectionHandler<DnsTcpConnectionHandler>());
        }
    }
}