using System.Net;
using DotDns.AspNetCore;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace DotDns.Example
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .AddUdpSupport()
                .UseKestrel(options =>
                {
                    options.ListenDnsUdp(new UdpEndpoint(IPAddress.Any, 8053));
                    options.ListenDnsTcp(new IPEndPoint(IPAddress.Any, 8053));
                })
                .UseStartup<Startup>();
        }
    }
}