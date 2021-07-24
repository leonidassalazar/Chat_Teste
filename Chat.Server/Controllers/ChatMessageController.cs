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

            return ServerInfoStore.Rooms.Where(r => r.Type == RoomType.Public ||
                                                    (r.Users.Contains(localUser) && r.Type == RoomType.Private))
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

                if (!room.Users.Any(q => string.Equals(q.Name, userName, StringComparison.CurrentCultureIgnoreCase)))
                {
                    room.Users.Add(user);
                }

                ServerInfoStore.ClientRequest.NotifyNewUser(userName: user.Name, roomName: room.Name);

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
        public Room CreateRoom(Room room)
        {
            if (string.IsNullOrEmpty(room.Name)) return null;

            var existentRoom = GetRoom(room.Name);
            if (existentRoom != null) return existentRoom;
            var newRoom = new Room(name: room.Name, type: RoomType.Public);
            ServerInfoStore.Rooms.Add(newRoom);
            return newRoom;

        }

        [HttpPost]
        public Room CreatePrivateRoom(Room room)
        {
            if (string.IsNullOrEmpty(room.Name)) return null;

            var existentRoom = GetRoom(room.Name);
            if (existentRoom != null) return null;
            var newRoom = new Room(name: room.Name, type: room.Type);

            foreach (var user in room.Users)
            {
                var localUser = GetUser(user.Name);
                newRoom.Users.Add(localUser);
            }

            ServerInfoStore.Rooms.Add(newRoom);
            return newRoom;

        }

        [HttpPost]
        public bool CreateUser(User user)
        {
            if (user == null) return false;
            if (string.IsNullOrEmpty(user.Name)) return false;
            if (string.IsNullOrEmpty(user.Address)) return false;
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
            if (string.IsNullOrEmpty(user.Name)) return false;

            var localUser = GetUser(user.Name);

            if (localUser != null)
            {
                foreach (var room in ServerInfoStore.Rooms)
                {
                    room.Users.Remove(localUser);
                }

                ServerInfoStore.Users.Remove(localUser);
                Console.WriteLine($"User {user.Name} was deleted");
            }
            return true;
        }

        [HttpPost]
        public void PushMessages(string roomName, Message message)
        {
            if (message == null) return;
            if (string.IsNullOrEmpty(message.Text)) return;
            if (string.IsNullOrEmpty(roomName)) return;

            ServerInfoStore.ClientRequest.PushMessages(roomName, message);
        }

        #endregion

        #region Private Methods

        private static bool ExistsRoom(string roomName)
        {
            return GetRoom(roomName) != null;
        }

        private static Room GetRoom(string roomName)
        {
            return ServerInfoStore.Rooms.FirstOrDefault(q =>
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

        #endregion
    }
}
