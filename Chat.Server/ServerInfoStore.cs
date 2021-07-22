using Chat.Core.Models;
using System.Collections.Generic;

namespace Chat.Server
{
    public static class ServerInfoStore
    {
        public static List<Room> Rooms { get; set; }
        public static List<User> Users { get; set; }
    }
}
