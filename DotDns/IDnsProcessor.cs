using System.Threading.Tasks;
using DotDns.Middlewares;

namespace DotDns
{
    public interface IDnsProcessor
    {
        void AddMiddleware<T>() where T : IDnsMiddleware;

        void Build();

        ValueTask Process(DnsPacket request, DnsPacket response);
    }
}