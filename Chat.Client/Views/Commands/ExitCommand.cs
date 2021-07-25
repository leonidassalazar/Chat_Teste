using Chat.Core.Models;
using System;

namespace Chat.Client.Views.Commands
{
    public class ExitCommand : ICommand
    {
        public bool Execute(string complement, ref Message message, Action<string> action)
        {
            if (string.IsNullOrEmpty(complement))
            {
                ClientInfoStore.ServerRequest.DeleteUser();
                Console.WriteLine("*** Disconnected. Bye!");
            }
            else
            {
                //TODO: Exit the room in the complement
                ClientInfoStore.ServerRequest.ExitRoom(complement);
                action?.Invoke(null);
            }
            return false;
        }

        public string CommandDescription()
        {
            return @"/exit [room name]  -  Exit a room
exit              -  Close chat";
        }
    }
}