using System;
using System.Linq;
using Chat.Core.Enum;
using Chat.Core.Models;

namespace Chat.Client.Views.Commands
{
    public class ToPrivatelyCommand : ICommand
    {
        public bool Execute(string complement, ref Message message, Action<string> action)
        {
            // TODO: Change the room to a private 
            if (!string.IsNullOrEmpty(complement))
            {
                var roomName = $"{complement}-{ClientInfoStore.User.Name}";
                var newRoom =
                    ClientInfoStore.User.Rooms
                        .FirstOrDefault(q => string.Equals(q.Name, roomName,
                            StringComparison.CurrentCultureIgnoreCase));

                if (newRoom == null)
                {
                    var room = new Room()
                    {
                        Name = roomName,
                        Type = RoomType.Private
                    };
                    room.Users.Add(new User
                    {
                        Name = complement
                    });
                    room.Users.Add(ClientInfoStore.User);
                    newRoom = ClientInfoStore.ServerRequest.CreatePrivateRoom(room);
                }
                else
                {
                    ClientInfoStore.ServerRequest.ChangeRoom(roomName);
                }

            }
            else
            {
                Console.WriteLine("The command /p needs a complement. View more with /h");
            }

            return false;

        }

        public string CommandDescription()
        {
            return $"/p [recipient user]  - Change the chat to a private room with the recipient user";
        }
    }
}
