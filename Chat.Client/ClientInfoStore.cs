using Chat.Client.BL;
using Chat.Client.Views;
using Chat.Core.Models;

namespace Chat.Client
{
    public static class ClientInfoStore
    {
        public static User User { get; set; }
        public static string ServerUrl { get; set; }
        public static ServerRequest ServerRequest { get; set; }
    }
}
