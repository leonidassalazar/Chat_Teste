using Chat.Core.Enum;
using Chat.Core.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;

namespace Chat.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ServerInfoStore.Rooms = new List<Room>();
            ServerInfoStore.Users = new List<User>();

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
