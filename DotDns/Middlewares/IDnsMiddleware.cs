using System.Threading.Tasks;

namespace DotDns.Middlewares
{
    public interface IDnsMiddleware
    {
        delegate ValueTask MiddlewareDelegate(DnsPacket request, DnsPacket response);

        ValueTask ProcessAsync(DnsPacket request, DnsPacket response);
    }
}