using Chat.Core.Enum;
using Chat.Core.Models;
using System;

namespace Chat.Client.Views.Commands
{
    public class CreateRoomCommand : ICommand
    {
        public bool Execute(string complement, ref Message message, Action<string> action)
        {
            //TODO: create rooms
            if (!string.IsNullOrEmpty(complement))
            {
                var room = new Room()
                {
                    Name = complement,
                    Type = RoomType.Public
                };
                var newRoom = ClientInfoStore.ServerRequest.CreateRoom(room);
                ClientInfoStore.ServerRequest.ChangeRoom(newRoom.Name);
            }
            else
            {
                Console.WriteLine("The command /cr needs a complement. View more with /h");
            }
            return false;
        }

        public string CommandDescription()
        {
            return $"/cr [room name]  -  Create a new room";
        }
    }
}