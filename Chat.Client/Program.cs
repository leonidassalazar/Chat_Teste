using Chat.Client.BL;
using Chat.Client.Views;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Net.Http;

namespace Chat.Client
{
    public class Program
    {
        private static string _applicationUrl;

        public static void Main(string[] args)
        {
            ClientInfoStore.ServerUrl = "https://localhost:5001";
            var client = new HttpClient
            {
                BaseAddress = new Uri(ClientInfoStore.ServerUrl)
            };

            var host = CreateHostBuilder(args).UseConsoleLifetime();

            ClientInfoStore.ServerRequest = new ServerRequest(hostUrl: _applicationUrl, client: client);

            host.Build().RunAsync();
            new ConsoleView().Start();
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
