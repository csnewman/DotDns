using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;

namespace DotDns.AspNetCore
{
    public class UdpConnectionListener : IConnectionListener
    {
        private Channel<ConnectionContext> _result;

        public UdpConnectionListener(UdpEndpoint endPoint)
        {
            EndPoint = endPoint;
        }

        public EndPoint EndPoint { get; set; }

        public ValueTask<ConnectionContext> AcceptAsync(CancellationToken cancellationToken = new())
        {
            return _result.Reader.ReadAsync(cancellationToken);
        }

        public ValueTask UnbindAsync(CancellationToken cancellationToken = new())
        {
            return default;
        }

        public ValueTask DisposeAsync()
        {
            return default;
        }

        public void Bind()
        {
            var endpoint = (UdpEndpoint) EndPoint;

            var listenSocket = new Socket(endpoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

            if (Equals(endpoint.Address, IPAddress.IPv6Any)) listenSocket.DualMode = true;

            try
            {
                listenSocket.Bind(endpoint);
            }
            catch (SocketException e) when (e.SocketErrorCode == SocketError.AddressAlreadyInUse)
            {
                throw new AddressInUseException(e.Message, e);
            }

            EndPoint = listenSocket.LocalEndPoint!;

            const int count = 50;

            _result = Channel.CreateBounded<ConnectionContext>(count);
            for (var i = 0; i < count; i++)
            {
                var _ = _result.Writer.WriteAsync(new UdpConnection(EndPoint, listenSocket));
            }
        }
    }
}