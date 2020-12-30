using System.Net;
using System.Threading.Tasks;
using DotDns.Records;

namespace DotDns.Middlewares
{
    public class ExampleMiddleware : IDnsMiddleware
    {
        private readonly IDnsMiddleware.MiddlewareDelegate _next;

        public ExampleMiddleware(IDnsMiddleware.MiddlewareDelegate next)
        {
            _next = next;
        }

        public async ValueTask ProcessAsync(DnsPacket request, DnsPacket response)
        {
            response.Authoritative = true;
            response.AddAnswer(new ARecord("example.com", 123, IPAddress.Parse("127.5.0.1").MapToIPv4()));

            await _next(request, response);
        }
    }
}