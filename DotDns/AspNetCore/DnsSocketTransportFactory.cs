using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DotDns.AspNetCore
{
    public class DnsSocketTransportFactory : IConnectionListenerFactory
    {
        private readonly SocketTransportFactory _socketTransportFactory;

        public DnsSocketTransportFactory(IOptions<SocketTransportOptions> options, ILoggerFactory loggerFactory)
        {
            _socketTransportFactory = new SocketTransportFactory(options, loggerFactory);
        }

        public ValueTask<IConnectionListener> BindAsync(EndPoint endpoint,
            CancellationToken cancellationToken = new())
        {
            return endpoint switch
            {
                UdpEndpoint udp => CreateUdpListener(udp),
                _ => _socketTransportFactory.BindAsync(endpoint, cancellationToken)
            };
        }

        private ValueTask<IConnectionListener> CreateUdpListener(UdpEndpoint udp)
        {
            var transport = new UdpConnectionListener(udp);
            transport.Bind();
            return new ValueTask<IConnectionListener>(transport);
        }
    }
}