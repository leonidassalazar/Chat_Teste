using Chat.Client.BL;
using Chat.Core.Models;
using System;
using System.Linq;
using System.Text;
using Chat.Client.Controllers;
using Chat.Core.Enum;

namespace Chat.Client.Views
{
    public class ConsoleView
    {
        private static ServerRequest _serverRequest;
        public ConsoleView(ServerRequest serverRequest)
        {
            _serverRequest = serverRequest;
        }

        public void Start()
        {
            Console.WriteLine("*** Welcome to our chat server. Please provide a nickname.");
            string userName = null;

            var newUser = new User();
            while (string.IsNullOrEmpty(value: userName))
            {
                Console.Write("> ");
                userName = Console.ReadLine();
                try
                {
                    if (!_serverRequest.CreateUser(user: out newUser, userName: userName))
                    {
                        Console.WriteLine(
                            $"*** Sorry, the nickname {userName} is already taken. Please choose a different one.");
                        userName = null;
                    }
                    else
                    {
                        ClientInfoStore.User = newUser;
                    }
                }
                catch (ArgumentNullException)
                {
                    Console.WriteLine("Empty nicknames are not accepted.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error on create {userName} user: {ex}");
                }
            }

            Console.Write($"You are registered as {newUser.Name}.");
            try
            {
                var roomGeneral = _serverRequest.EnterRoom(roomName: "general");

                if (roomGeneral == null)
                {
                    Console.WriteLine("Error on access the #general room, please try again latter.");
                    return;
                }

                Console.WriteLine($"Joining #{roomGeneral.Name}.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error joining #general message: {e}");
                throw;
            }

            while (true)
            {
                try
                {
                    Console.Write("> ");
                    var msg = Console.ReadLine();

                }
                catch (ArgumentNullException)
                {
                    Console.WriteLine("Empty text will not be considered.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error on send message: {ex}");
                }
            }

        }

        private void TreatInput(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentNullException(nameof(input));
            }

            var inputParts = input.Split('/');

            var message = new Message();
            var sendMessage = true;

            for (var i = 0; i < inputParts.Length; i++)
            {
                var part = inputParts[i].Trim();
                var spaceIndex = part.IndexOf(' ');

                var command = part.Substring(0, spaceIndex + 1);
                var complement = part.Length > spaceIndex + 1 ? part.Substring(spaceIndex + 1) : string.Empty;

                switch (command)
                {
                    case "/t":
                        {
                            message.UserDest = complement;
                            break;
                        }
                    case "/p":
                        {
                            //TODO: Change the room to a private 
                            break;
                        }
                    case "/lr":
                        {
                            var rooms = _serverRequest.GetRooms();
                            //TODO: Print the rooms on console
                            break;
                        }
                    case "/lu":
                        {
                            var users = _serverRequest.GetUsers();
                            //TODO: Print the users on console
                            break;
                        }
                    case "/exit":
                        {
                            sendMessage = false;
                            if (string.IsNullOrEmpty(complement))
                            {
                                _serverRequest.DeleteUser();
                            }
                            else
                            {
                                //TODO: Exit the room in the complement
                                _serverRequest.ExitRoom(complement);
                                PrintUnreadMessages();
                            }
                            break;
                        }
                    case "/ch":
                        {
                            //TODO: change the room, verify if complement is not null
                            if (!string.IsNullOrEmpty(complement))
                            {
                                _serverRequest.ChangeRoom(complement);
                                PrintUnreadMessages();
                            }
                            break;
                        }
                    case "/cr":
                        {
                            //TODO: create rooms
                            break;
                        }
                    default:
                        {
                            message.Text = complement;
                            break;
                        }
                }
            }

            if (!sendMessage) return;
            if (!_serverRequest.SendMessage(message, out var messageError))
            {
                Console.WriteLine($"Fail to send the message to server: {messageError}");
            }
        }

        public void PrintUnreadMessages(string roomName = null)
        {
            if (!string.IsNullOrEmpty(roomName) && 
                ClientInfoStore.User.Rooms.Any(q => q.State == StateEnum.Active &&
                                                    !string.Equals(q.Name, roomName,
                                                                   StringComparison.CurrentCultureIgnoreCase)))
            {
                return;
            }

            var activeRoom = ClientInfoStore.User.Rooms.FirstOrDefault(q => q.State == StateEnum.Active);
            
            if (activeRoom == null) return;

            var unreadMessages = activeRoom.Messages.Where(m => m.Date > activeRoom.LastView).ToList();

            foreach (var unreadMessage in unreadMessages)
            {
                var text = new StringBuilder();
                text.Append($"#{activeRoom.Name} -- {unreadMessage.UserSource} says");

                text.Append(string.IsNullOrEmpty(unreadMessage.UserSource) ? ": " : $" {unreadMessage.UserSource} says: ");
                text.Append(string.IsNullOrEmpty(unreadMessage.UserDest) ? ": " : $" to {unreadMessage.UserDest}: ");

                text.Append($"{unreadMessage.Text}");

                Console.WriteLine(text.ToString());
            }
        }

    }
}
