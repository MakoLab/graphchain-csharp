using System;
using System.Net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace elephant.web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Write("Enter port number: ");
            var portStr = Console.ReadLine();
            if (Int32.TryParse(portStr, out var port))
            {
                BuildWebHost(args, port).Run();
            }
        }

        public static IWebHost BuildWebHost(string[] args, int port) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseKestrel(options =>
                {
                    options.Listen(IPAddress.Loopback, port);
                })
                .Build();
    }
}
