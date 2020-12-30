using System;
using System.Net;
using System.Threading.Tasks;
using Bedrock.Framework;
using DotDns.AspNetCore;
using DotDns.Middlewares;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DotDns.BedrockExample
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            // Manual wire up of the server
            var services = new ServiceCollection();
            services.AddLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Debug);
                builder.AddConsole();
            });

            services.AddDns();

            var serviceProvider = services.BuildServiceProvider();

            var processor = serviceProvider.GetRequiredService<IDnsProcessor>();
            processor.AddMiddleware<ExampleMiddleware>();
            processor.Build();

            var server = new ServerBuilder(serviceProvider)
                .ListenDnsUdp(new UdpEndpoint(IPAddress.Any, 8053))
                .ListenDnsTcp(new IPEndPoint(IPAddress.Any, 8053))
                .Build();

            var logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger<Program>();

            await server.StartAsync();

            foreach (var ep in server.EndPoints) logger.LogInformation("Listening on {EndPoint}", ep);

            var tcs = new TaskCompletionSource<object>();
            Console.CancelKeyPress += (sender, e) => tcs.TrySetResult(null);
            await tcs.Task;

            await server.StopAsync();
        }
    }
}