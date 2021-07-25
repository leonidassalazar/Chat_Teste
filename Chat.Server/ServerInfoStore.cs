using Chat.Core.Models;
using Chat.Server.BL;
using System.Collections.Generic;

namespace Chat.Server
{
    public static class ServerInfoStore
    {
        public static SynchronizedCollection<Room> Rooms { get; set; }
        public static SynchronizedCollection<User> Users { get; set; }
        public static ClientRequest ClientRequest { get; set; }
    }
}
