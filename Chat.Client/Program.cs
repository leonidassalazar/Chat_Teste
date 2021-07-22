using Chat.Client.BL;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using Chat.Client.Views;

namespace Chat.Client
{
    public class Program
    {
        private static string _applicationUrl;

        public static void Main(string[] args)
        {
            ClientInfoStore.ServerUrl = "https://localhost:5001";

            var host= CreateHostBuilder(args).UseConsoleLifetime();
            var serverRequest = new ServerRequest(hostUrl:_applicationUrl);
            ClientInfoStore.ConsoleView = new ConsoleView(serverRequest);

            host.Build().RunAsync();
            ClientInfoStore.ConsoleView.Start();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    var port = new Random().Next(5003, 65535);
                    var url = Environment.GetEnvironmentVariable("ASPNETCORE_URLS")?.Split(";") ??
                                     new[] { "https://localhost", "http://localhost" };
                    url = url.Select(q => q.ToLower().Contains("https") ? $"{q}:{port}" : $"{q}:{port - 1}").ToArray();

                    _applicationUrl = url.FirstOrDefault(q => q.ToLower().Contains("https"));
                    webBuilder.UseStartup<Startup>().UseUrls(url);
                });
    }
}
