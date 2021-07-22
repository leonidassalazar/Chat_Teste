using Chat.Core.Enum;
using Chat.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Chat.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class ChatMessageController : ControllerBase
    {
        private readonly ILogger<ChatMessageController> _logger;

        public ChatMessageController(ILogger<ChatMessageController> logger)
        {
            _logger = logger;
        }

        #region Get Methods

        [HttpGet]
        public IEnumerable<string> GetRooms(string userName)
        {
            var localUser = GetUser(userName);

            return ServerInfoStore.Rooms.Where(r => r.Type == RoomType.Public &&
                                                    r.Users.Contains(localUser))
                                        .Select(q => q.Name);
        }

        [HttpGet]
        public IEnumerable<string> GetUsers()
        {
            return ServerInfoStore.Users.Select(q => q.Name);
        }

        [HttpGet]
        public Room EnterRoom(string roomName, string userName)
        {
            var room = ServerInfoStore.Rooms.FirstOrDefault(q =>
                string.Equals(
                    q.Name,
                    roomName,
                    StringComparison.InvariantCultureIgnoreCase));
            if (room != null)
            {
                var user = ServerInfoStore.Users.FirstOrDefault(q =>
                    string.Equals(
                        q.Name,
                        userName,
                        StringComparison.InvariantCultureIgnoreCase));

                if (user == null)
                {
                    return null;
                }

                room.Users.Add(user);

                NotifyNewUser(userName: user.Name, roomName: room.Name);

                return room;
            }

            return null;
        }

        [HttpGet]
        public void ExitRoom(string roomName, string userName)
        {
            if (roomName == null) return;
            if (userName == null) return;

            var room = ServerInfoStore.Rooms.FirstOrDefault(q =>
                string.Equals(
                    q.Name,
                    roomName,
                    StringComparison.InvariantCultureIgnoreCase));
            if (room != null)
            {
                var user = ServerInfoStore.Users.FirstOrDefault(q =>
                    string.Equals(
                        q.Name,
                        userName,
                        StringComparison.InvariantCultureIgnoreCase));

                if (user == null)
                {
                    return;
                }

                room.Users.Remove(user);
                Console.WriteLine($"User {user.Name} has exited #{room.Name}");
            }
        }

        #endregion

        #region Post Methods

        [HttpPost]
        public Room CreatePublicRoom(string roomName)
        {
            if (string.IsNullOrEmpty(roomName)) return null;

            if (!ExistsRoom(roomName: roomName))
            {
                var room = new Room(name: roomName, type: RoomType.Public);
                ServerInfoStore.Rooms.Add(room);
                return room;
            }
        }

        [HttpPost]
        public bool CreateUser(User user)
        {
            if (user == null) return false;
            if (!ExistsUser(userName: user.Name))
            {
                _ = NewUser(user: user);
                Console.WriteLine($"User {user.Name} was created");
                return true;
            }

            return false;
        }

        [HttpPost]
        public bool DeleteUser(User user)
        {
            if (user == null) return false;

            var localUser = GetUser(user.Name);

            if (localUser != null)
            {
                ServerInfoStore.Rooms.ForEach(r =>
                {
                    r.Users.Remove(localUser);
                });

                ServerInfoStore.Users.Remove(localUser);
                Console.WriteLine($"User {user.Name} was deleted");
            }
            return true;
        }

        [HttpPost]
        public void PushMessages(string roomName, Message message)
        {
            if (message == null) return;
            if (string.IsNullOrEmpty(roomName)) return;

            var room = ServerInfoStore.Rooms.FirstOrDefault(q => string.Equals(q.Name, roomName, StringComparison.InvariantCultureIgnoreCase))
                       ?? new Room();

            foreach (var user in room.Users.Where(q => !string.Equals(q.Name, message.UserSource, StringComparison.CurrentCultureIgnoreCase)))
            {
                try
                {
                    var client = new HttpClient
                    {
                        BaseAddress = new Uri(uriString: user.Address)
                    };

                    var json = JsonConvert.SerializeObject(value: message);
                    var c = new StringContent(content: json);
                    c.Headers.ContentType =
                        new System.Net.Http.Headers.MediaTypeHeaderValue(mediaType: "application/json");

                    var result = client.PostAsync(requestUri: $"api/Main/ReceiveMessage?roomName={roomName}", c).Result;

                    Console.WriteLine(result.IsSuccessStatusCode
                        ? $"Message from {user.Name} was sent: {result.StatusCode}"
                        : $"Send message fail from {user.Name}: {result.StatusCode}");

                    Console.WriteLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error on send message to #{room.Name}: {ex}");
                }
            }
        }

        #endregion

        #region Private Methods

        private static bool ExistsRoom(string roomName)
        {
            return ServerInfoStore.Rooms.Any(q =>
                string.Equals(
                    q.Name,
                    roomName,
                    StringComparison.InvariantCultureIgnoreCase
                ));
        }

        private static User GetUser(string userName)
        {
            return ServerInfoStore.Users.FirstOrDefault(q =>
                string.Equals(
                    q.Name,
                    userName,
                    StringComparison.InvariantCultureIgnoreCase
                ));
        }

        private static bool ExistsUser(string userName)
        {
            return GetUser(userName: userName) != null;
        }

        private static User NewUser(User user)
        {
            ServerInfoStore.Users.Add(user);
            return user;
        }

        private static void NotifyNewUser(string roomName, string userName)
        {
            var msg = $"{userName} has joined #{roomName}";

            var room = ServerInfoStore.Rooms.FirstOrDefault(q => string.Equals(q.Name, roomName, StringComparison.InvariantCultureIgnoreCase))
                       ?? new Room();

            foreach (var user in room.Users.Where(q => !string.Equals(q.Name, userName, StringComparison.CurrentCultureIgnoreCase)))
            {
                try
                {
                    var message = new Message { Text = msg };
                    var c = new StringContent(content: JsonConvert.SerializeObject(value: message));
                    c.Headers.ContentType =
                        new System.Net.Http.Headers.MediaTypeHeaderValue(mediaType: "application/json");

                    var client = new HttpClient
                    {
                        BaseAddress = new Uri(uriString: user.Address)
                    };

                    var result = client.PostAsync(requestUri: $"api/Main/NotifyNewUser", c).Result;

                    Console.WriteLine(msg);

                    Console.WriteLine(result.IsSuccessStatusCode
                        ? $"Message from {user.Name} was sent: {result.StatusCode}"
                        : $"Send message fail from {user.Name}: {result.StatusCode}");

                    Console.WriteLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error on notify message to #{room.Name}: {ex}");
                }
            }
        }
        #endregion
    }
}
