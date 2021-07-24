using Chat.Client.BL;
using Chat.Core.Enum;
using Chat.Core.Models;
using Chat.Core.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Chat.Client.Views.Commands;

namespace Chat.Client.Views
{
    public class ConsoleView
    {
        private Room _activeRoom;
        public void Start()
        {
            Console.WriteLine("*** Welcome to our chat server. Please provide a nickname.");
            string userName = null;

            var newUser = new User();
            while (string.IsNullOrEmpty(value: userName))
            {
                Console.Write(" ");
                userName = Console.ReadLine();
                try
                {
                    if (!ClientInfoStore.ServerRequest.CreateUser(user: out newUser, userName: userName))
                    {
                        Console.WriteLine(
                            $"*** Sorry, the nickname {userName} is already taken. Please choose a different one.");
                        userName = null;
                    }
                    else
                    {
                        ClientInfoStore.User = newUser;
                        ClientInfoStore.User.PropertyChanged -= UserOnPropertyChanged;
                        ClientInfoStore.User.PropertyChanged += UserOnPropertyChanged;
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
                var roomGeneral = EnterRoom(roomName: "general");

                if (roomGeneral == null)
                {
                    Console.WriteLine("Error on access the #general room, please try again latter.");
                    return;
                }
                ClientInfoStore.ServerRequest.ChangeRoom("general");

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
                    Console.Write("   ");
                    var msg = Console.ReadLine();
                    TreatInput(msg);
                    if (msg?.ToLower() == "/exit")
                    {
                        return;
                    }
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

        public void PrintUnreadMessages(string roomName = null)
        {
            if (!string.IsNullOrEmpty(roomName) &&
                ClientInfoStore.User.Rooms.Any(q => q.State == StateEnum.Active &&
                                                    !string.Equals(q.Name, roomName,
                                                                   StringComparison.CurrentCultureIgnoreCase)))
            {
                NotifyMessageFromOtherRoom(roomName);
            }

            var activeRoom = ClientInfoStore.User.Rooms.FirstOrDefault(q => q.State == StateEnum.Active);

            if (activeRoom == null) return;

            var unreadMessages = activeRoom.Messages.Where(m => m.Date > activeRoom.LastView).ToList();
            activeRoom.LastView = DateTime.Now;

            foreach (var unreadMessage in unreadMessages)
            {
                var text = new StringBuilder();
                text.Append($"#{activeRoom.Name} -- ");

                if (!string.IsNullOrEmpty(unreadMessage.UserSource))
                {
                    text.Append($"{unreadMessage.UserSource} says");
                    text.Append(string.IsNullOrEmpty(unreadMessage.UserDest) ? ": " : $" to {unreadMessage.UserDest}: ");
                }

                text.Append($"{unreadMessage.Text}");

                Console.WriteLine(text.ToString());
            }
        }

        public Room NotifyMessageFromOtherRoom(string roomName)
        {
            var room = ClientInfoStore.User.Rooms
                .FirstOrDefault(q => string.Equals(q.Name, roomName,
                    StringComparison.CurrentCultureIgnoreCase));
            if (room == null)
            {
                room = EnterRoom(roomName);
                ClientInfoStore.User.AddRoom(room);
            }

            if (room.State == StateEnum.Deactivated)
            {
                Console.WriteLine($"You received a message in #{roomName}");
            }
            return room;
        }

        private void TreatInput(string input)
        {

            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (!input.Trim().StartsWith('/'))
            {
                input = $"/m {input}";
            }

            //var inputParts = input.Split('/');
            var inputParts = Regex.Split(input, @"(/\w+\s*)")
                .Where(q => !string.IsNullOrEmpty(q.Trim())).ToArray();

            var message = new Message();
            var sendMessage = false;

            for (var i = 0; i < inputParts.Length; i += 2)
            {
                var commandParameter = inputParts[i].Trim();
                string complement = null;
                if (!string.IsNullOrEmpty(inputParts.ElementAtOrDefault(i + 1)))
                {
                    complement = inputParts[i + 1].Trim();
                }

                var command = CommandStrategy.GetCommand(commandParameter);
                sendMessage = command.Execute(complement, ref message, PrintUnreadMessages);
            }

            if (!sendMessage) return;

            var activeRoom = ClientInfoStore.User.Rooms.FirstOrDefault(q => q.State == StateEnum.Active);

            if (!ClientInfoStore.ServerRequest.SendMessage(message, out var messageError, roomName: activeRoom.Name))
            {
                Console.WriteLine($"Fail to send the message to server: {messageError}");
            }
        }

        private Room EnterRoom(string roomName)
        {
            var room = ClientInfoStore.ServerRequest.EnterRoom(roomName: roomName);

            if (room == null)
            {
                Console.WriteLine($"Error on access the #{roomName} room, please try again latter.");
                return null;
            }

            room.MessageReceived -= RoomOnMessageReceived;
            room.MessageReceived += RoomOnMessageReceived;

            return room;
        }

        private void UserOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ClientInfoStore.User.Rooms))
            {
                var newActive = GetRoomActive();
                if (newActive == null)
                {
                    _activeRoom = ClientInfoStore.User.Rooms.FirstOrDefault();
                    Console.WriteLine($"Changed to #{_activeRoom.Name}");
                }
                else if (!ReferenceEquals(_activeRoom, newActive))
                {
                    _activeRoom = GetRoomActive();
                    Console.WriteLine($"Changed to #{_activeRoom.Name}");
                }
                //Não tive tempo de melhorar de notificação de novas mensagens para novas salas,
                //aí coloquei para atualizar os eventos de todas as salas
                foreach (var userRoom in ClientInfoStore.User.Rooms)
                {
                    userRoom.MessageReceived -= RoomOnMessageReceived;
                    userRoom.MessageReceived += RoomOnMessageReceived;
                }
            }
        }

        private void RoomOnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (sender is Room room)
            {
                PrintUnreadMessages(room.Name);
            }
        }
        private static Room GetRoomActive()
        {
            return ClientInfoStore.User.Rooms.FirstOrDefault(q => q.State == StateEnum.Active);
        }

    }
}
