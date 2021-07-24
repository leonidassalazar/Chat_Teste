using Chat.Core.Enum;
using Chat.Core.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Net.Http;
using Chat.Server.BL;
using Microsoft.AspNetCore.Http.Connections;

namespace Chat.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ServerInfoStore.Rooms = new SynchronizedCollection<Room>();
            ServerInfoStore.Users = new SynchronizedCollection<User>();

            ServerInfoStore.ClientRequest = new ClientRequest(new HttpClientHandler());

            var generalRoom = new Room("general", RoomType.Public);
            ServerInfoStore.Rooms.Add(generalRoom);

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
