using Chat.Core.Enum;
using Chat.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace Chat.Client.BL
{
    public class ServerRequest
    {
        private readonly string _hostUrl;
        private readonly HttpClient _client;

        public ServerRequest(string hostUrl, HttpClient client)
        {
            _hostUrl = hostUrl;

            _client = client;
        }

        #region Private request methods

        public bool CreateUser(string userName, out User user)
        {
            user = new User
            {
                Name = userName,
                Address = _hostUrl
            };
            if (string.IsNullOrEmpty(user.Name))
            {
                throw new ArgumentNullException(nameof(userName));
            }

            var serializedUser = JsonConvert.SerializeObject(user);
            var c = new StringContent(serializedUser);
            c.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(mediaType: "application/json");

            _client.DefaultRequestHeaders.Clear();
            var result = _client.PostAsync(requestUri: "api/ChatMessage/CreateUser", c).Result;

            var tryParse = bool.TryParse(result.Content.ReadAsStringAsync().Result, out var success);

            return tryParse && success;
        }

        public bool DeleteUser()
        {
            var serializedUser = JsonConvert.SerializeObject(ClientInfoStore.User);
            var c = new StringContent(serializedUser);
            c.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(mediaType: "application/json");

            _client.DefaultRequestHeaders.Clear();
            var result = _client.PostAsync(requestUri: "api/ChatMessage/DeleteUser", c).Result;

            var tryParse = bool.TryParse(result.Content.ReadAsStringAsync().Result, out var success);

            ClientInfoStore.User = null;

            return tryParse && success;
        }

        public List<string> GetRooms()
        {
            _client.DefaultRequestHeaders.Clear();
            var result = _client.GetAsync(requestUri: $"api/ChatMessage/GetRooms?userName={ClientInfoStore.User}").Result;

            var rooms = JsonConvert.DeserializeObject<List<string>>(result.Content.ReadAsStringAsync().Result);

            return rooms;
        }

        public List<string> GetUsers()
        {
            _client.DefaultRequestHeaders.Clear();
            var result = _client.GetAsync(requestUri: "api/ChatMessage/GetUsers").Result;

            var rooms = JsonConvert.DeserializeObject<List<string>>(result.Content.ReadAsStringAsync().Result);

            return rooms;
        }

        public Room CreateRoom(Room room)
        {
            var serializedUser = JsonConvert.SerializeObject(room);
            var c = new StringContent(serializedUser);

            c.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(mediaType: "application/json");

            _client.DefaultRequestHeaders.Clear();
            var result = _client.PostAsync($"api/ChatMessage/CreateRoom", c)
                .Result;

            var createdRoom = JsonConvert.DeserializeObject<Room>(result.Content.ReadAsStringAsync().Result);

            if (createdRoom == null)
            {
                throw new ArgumentNullException($"The room name can't be null or empty.");
            }

            if (!ClientInfoStore.User.Rooms
                .Any(q => string.Equals(q.Name, createdRoom.Name,
                                            StringComparison.CurrentCultureIgnoreCase))
            )
            {
                ClientInfoStore.User.AddRoom(createdRoom);
            }
            ActivateRoom(createdRoom);

            return createdRoom;
        }

        public Room CreatePrivateRoom(Room room)
        {
            var serializedUser = JsonConvert.SerializeObject(room);
            var c = new StringContent(serializedUser);

            c.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(mediaType: "application/json");

            var result = _client.PostAsync($"api/ChatMessage/CreatePrivateRoom", c)
                .Result;

            var createdRoom = JsonConvert.DeserializeObject<Room>(result.Content.ReadAsStringAsync().Result);

            if (createdRoom == null)
            {
                throw new ArgumentNullException($"The room name can't be null or empty.");
            }

            if (!ClientInfoStore.User.Rooms
                .Any(q => string.Equals(q.Name, createdRoom.Name,
                    StringComparison.CurrentCultureIgnoreCase))
            )
            {
                ClientInfoStore.User.AddRoom(createdRoom);
            }
            ActivateRoom(createdRoom);

            return createdRoom;
        }

        public Room EnterRoom(string roomName)
        {
            var userName = ClientInfoStore.User.Name;

            _client.DefaultRequestHeaders.Clear();
            var result = _client.GetAsync($"api/ChatMessage/EnterRoom?roomName={roomName}&userName={userName}")
                .Result;

            var room = JsonConvert.DeserializeObject<Room>(result.Content.ReadAsStringAsync().Result);

            ClientInfoStore.User.AddRoom(room);

            return room;
        }

        public bool ExitRoom(string roomName)
        {
            var room = ClientInfoStore.User.Rooms.FirstOrDefault(q =>
                string.Equals(q.Name, roomName, StringComparison.CurrentCultureIgnoreCase));
            if (room == null)
            {
                throw new KeyNotFoundException($"{roomName} not found in user's rooms");
            }

            if (roomName.ToLower() == "general")
            {
                throw new AccessViolationException("it's not possible remove #general room");
            }

            var userName = ClientInfoStore.User.Name;

            _client.DefaultRequestHeaders.Clear();
            var result = _client.GetAsync($"api/ChatMessage/ExitRoom?roomName={roomName}&userName={userName}")
                .Result;

            var tryParse = bool.TryParse(result.Content.ReadAsStringAsync().Result, out var success);

            if (tryParse && success)
            {
                ClientInfoStore.User.RemoveRoom(room);

                if (room.State != StateEnum.Active) return true;

                var newRoom = ClientInfoStore.User.Rooms.FirstOrDefault();
                if (newRoom != null) newRoom.State = StateEnum.Active;
            }

            return tryParse && success;
        }

        public void ChangeRoom(string newRoom)
        {
            var room = ClientInfoStore.User.Rooms.FirstOrDefault(q =>
                            string.Equals(q.Name, newRoom, StringComparison.CurrentCultureIgnoreCase)) ??
                       EnterRoom(newRoom);

            ActivateRoom(room);
        }

        public bool SendMessage(Message message, out string messageError, string roomName = null)
        {
            messageError = null;
            message.UserSource = ClientInfoStore.User.Name;

            var c = new StringContent(JsonConvert.SerializeObject(message));
            c.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(mediaType: "application/json");

            _client.DefaultRequestHeaders.Clear();
            var result = _client.PostAsync(requestUri: $"api/ChatMessage/PushMessages?roomName={roomName}", c)
                .Result;

            if (!result.IsSuccessStatusCode)
            {
                messageError = result.ReasonPhrase;
            }

            return result.IsSuccessStatusCode;
        }

        #endregion

        #region Private methods

        private static void ActivateRoom(Room activeRoom)
        {
            var lastRoom = ClientInfoStore.User.Rooms.FirstOrDefault(q => q.State == StateEnum.Active);
            if (lastRoom != null)
            {
                lastRoom.LastView = DateTime.Now;
            }

            activeRoom.State = StateEnum.Active;

            ClientInfoStore.User.Rooms.Where(q => !string.Equals(q.Name, activeRoom.Name, StringComparison.CurrentCultureIgnoreCase)).ToList()
                .ForEach(r =>
                {
                    r.State = StateEnum.Deactivated;
                });
        }

        #endregion

    }
}
