using System;
using DotDns.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace DotDns.AspNetCore
{
    public static class DnsAspnetCoreExtensions
    {
        public static IWebHostBuilder AddUdpSupport(this IWebHostBuilder webHostBuilder)
        {
            return webHostBuilder.ConfigureServices(services =>
            {
                services.AddSingleton<IConnectionListenerFactory, DnsSocketTransportFactory>();
            });
        }

        public static void UseDnsMiddleware<T>(this IApplicationBuilder app) where T : IDnsMiddleware
        {
            var dnsProcessor = app.ApplicationServices.GetService<IDnsProcessor>();

            if (dnsProcessor == null) throw new ArgumentException("Application does not have DNS added");

            dnsProcessor.AddMiddleware<T>();
        }

        public static void ConfigureDns(this IApplicationBuilder app)
        {
            var dnsProcessor = app.ApplicationServices.GetService<IDnsProcessor>();

            if (dnsProcessor == null) throw new ArgumentException("Application does not have DNS added");

            dnsProcessor.Build();
        }
    }
}