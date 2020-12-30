using Microsoft.Extensions.DependencyInjection;

namespace DotDns
{
    public static class DnsExtensions
    {
        public static void AddDns(this IServiceCollection services)
        {
            services.AddSingleton<IDnsReader, DnsReader>();
            services.AddSingleton<IDnsWriter, DnsWriter>();
            services.AddSingleton<IDnsProcessor, DnsProcessor>();
        }
    }
}