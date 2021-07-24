using System;
using Chat.Core.Models;

namespace Chat.Client.Views.Commands
{
    public class ChangeRoomCommand : ICommand
    {
        public bool Execute(string complement, ref Message message, Action<string> action)
        {
            //TODO: change the room, verify if complement is not null
            if (!string.IsNullOrEmpty(complement))
            {
                ClientInfoStore.ServerRequest.ChangeRoom(complement);
                action?.Invoke(null);
            }
            else
            {
                Console.WriteLine("The command /ch needs a complement. View more with /h");
            }
            return false;
        }

        public string CommandDescription()
        {
            return $"/ch [room name]  - Change the chat to target room ";
        }
    }
}