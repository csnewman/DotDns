using System.Net;
using Bedrock.Framework;
using DotDns.AspNetCore;
using Microsoft.AspNetCore.Connections;

namespace DotDns.BedrockExample
{
    public static class ServerBuilderExtensions
    {
        public static ServerBuilder ListenDnsUdp(this ServerBuilder builder, UdpEndpoint endPoint)
        {
            return builder.Listen<UdpConnectionListenerFactory>(
                endPoint,
                options => options.UseConnectionHandler<DnsUdpConnectionHandler>()
            );
        }

        public static ServerBuilder ListenDnsTcp(this ServerBuilder builder, IPEndPoint endPoint)
        {
            return builder.UseSockets(
                options => options.Listen(
                    endPoint,
                    connectionBuilder => connectionBuilder.UseConnectionHandler<DnsTcpConnectionHandler>()
                )
            );
        }
    }
}