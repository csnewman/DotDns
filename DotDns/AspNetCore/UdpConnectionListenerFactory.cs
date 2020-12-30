using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Connections;

namespace DotDns.AspNetCore
{
    public class UdpConnectionListenerFactory : IConnectionListenerFactory
    {
        public ValueTask<IConnectionListener> BindAsync(EndPoint endpoint,
            CancellationToken cancellationToken = new())
        {
            var listener = new UdpConnectionListener((UdpEndpoint) endpoint);
            listener.Bind();
            return new ValueTask<IConnectionListener>(listener);
        }
    }
}