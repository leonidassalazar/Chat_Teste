using Chat.Core.Enum;
using Chat.Core.Models;
using Chat.Server.BL;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Chat.Server.Tests.Controllers
{
    public class ChatMessageControllerTests : IClassFixture<TestFixture<Startup>>
    {
        private readonly HttpClient _clientServer;

        public ChatMessageControllerTests(TestFixture<Startup> fixture)
        {
            _clientServer = fixture.ClientServer;

            var room1 = new Room
            {
                Name = "Sala da Galera",
                Type = RoomType.Public
            };
            var room2 = new Room
            {
                Name = "Fundão",
                Type = RoomType.Public
            };
            var room3 = new Room
            {
                Name = "Família",
                Type = RoomType.Public
            };
            var room4 = new Room
            {
                Name = "Banda",
                Type = RoomType.Public
            };
            var room5 = new Room
            {
                Name = "Juquinha-Carlinhos",
                Type = RoomType.Private
            };

            var user1 = new User()
            {
                Name = "Juquinha",
                Address = "https://localhost:10000"
            };
            var user2 = new User()
            {
                Name = "Carlinhos",
                Address = "https://localhost:10001"
            };
            var user3 = new User()
            {
                Name = "Joaquina",
                Address = "https://localhost:10002"
            };
            var user4 = new User()
            {
                Name = "Amandinha",
                Address = "https://localhost:10003"
            };
            var user5 = new User()
            {
                Name = "Aroaldo",
                Address = "https://localhost:10004"
            };

            room1.Users.Add(user3);
            room1.Users.Add(user4);
            room1.Users.Add(user5);

            room2.Users.Add(user1);
            room2.Users.Add(user2);
            room2.Users.Add(user3);

            room3.Users.Add(user3);
            room3.Users.Add(user5);

            room4.Users.Add(user2);

            room5.Users.Add(user1);
            room5.Users.Add(user2);

            ServerInfoStore.Users = new SynchronizedCollection<User>
            {
                user1,
                user2,
                user3,
                user4,
                user5
            };

            ServerInfoStore.Rooms = new SynchronizedCollection<Room>
            {
                room1,
                room2,
                room3,
                room4,
                room5
            };
        }

        #region HttpGet Tests

        #region GetRooms

        [Fact]
        public async Task TestGetRooms()
        {
            // Arrange
            var request = $"api/ChatMessage/GetRooms?userName={ServerInfoStore.Users[3].Name}";

            // Act
            var response = await _clientServer.GetAsync(request);
            var rooms = JsonConvert.DeserializeObject<List<string>>(response.Content.ReadAsStringAsync().Result);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(4, rooms.Count);
        }

        [Fact]
        public async Task TestGetRoomsNoExistUsers()
        {
            // Arrange
            var request = $"api/ChatMessage/GetRooms?userName=marquito";

            // Act
            var response = await _clientServer.GetAsync(request);
            var rooms = JsonConvert.DeserializeObject<List<string>>(response.Content.ReadAsStringAsync().Result);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(4, rooms.Count);
        }

        #endregion

        #region GetUsers

        [Fact]
        public async Task TestGetUsers()
        {
            // Arrange
            var request = $"api/ChatMessage/GetUsers";

            // Act
            var response = await _clientServer.GetAsync(request);
            var users = JsonConvert.DeserializeObject<List<string>>(response.Content.ReadAsStringAsync().Result);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(5, users.Count);
        }

        #endregion

        #region EnterRoom

        [Fact]
        public async Task TestEnterRoom()
        {
            var roomNumber = 0;
            var userNumber = 0;
            var userName = ServerInfoStore.Users[userNumber].Name;
            var roomName = ServerInfoStore.Rooms[roomNumber].Name;
            var roomNumUsers = ServerInfoStore.Rooms[roomNumber].Users.Count;

            var endpoint = $"api/Main/NotifyNewUser";
            var mockHttp = new MockHttpMessageHandler();

            foreach (var user in ServerInfoStore.Users)
            {
                // Setup a respond for the user api (including a wildcard in the URL)
                mockHttp.When($"{user.Address}/{endpoint}")
                    .Respond("application/json",
                        @"{}"
                    ); // Respond with JSON
            }

            ServerInfoStore.ClientRequest = new ClientRequest(httpMessageHandler: mockHttp);

            // Arrange
            var request = $"api/ChatMessage/EnterRoom?userName={userName}&roomName={roomName}";

            // Act
            var response = await _clientServer.GetAsync(request);
            var room = JsonConvert.DeserializeObject<Room>(response.Content.ReadAsStringAsync().Result);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(roomNumUsers + 1, ServerInfoStore.Rooms[roomNumber].Users.Count);
        }

        [Fact]
        public async Task TestEnterRoomNotExistRoom()
        {
            var userNumber = 0;
            var userName = ServerInfoStore.Users[userNumber].Name;

            // Arrange
            var request = $"api/ChatMessage/EnterRoom?userName={userName}&roomName=banana";

            // Act
            var response = await _clientServer.GetAsync(request);
            var room = JsonConvert.DeserializeObject<Room>(response.Content.ReadAsStringAsync().Result);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Null(room);
        }

        [Fact]
        public async Task TestEnterRoomNotExistUser()
        {
            var roomNumber = 0;
            var roomName = ServerInfoStore.Rooms[roomNumber].Name;

            // Arrange
            var request = $"api/ChatMessage/EnterRoom?userName=Ricky&roomName={roomName}";

            // Act
            var response = await _clientServer.GetAsync(request);
            var room = JsonConvert.DeserializeObject<Room>(response.Content.ReadAsStringAsync().Result);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Null(room);
        }

        #endregion

        #region ExistRoom

        [Fact]
        public async Task TestExistRoom()
        {
            var roomNumber = 0;
            var roomName = ServerInfoStore.Rooms[roomNumber].Name;
            var userName = ServerInfoStore.Rooms[roomNumber].Users[0].Name;
            var roomNumUsers = ServerInfoStore.Rooms[roomNumber].Users.Count;

            // Arrange
            var request = $"api/ChatMessage/ExitRoom?userName={userName}&roomName={roomName}";

            // Act
            var response = await _clientServer.GetAsync(request);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(roomNumUsers - 1, ServerInfoStore.Rooms[roomNumber].Users.Count);
        }

        #endregion

        #endregion

        #region HttpPost Tests

        #region CreateRoom

        [Fact]
        public async Task TestCreateRoomPublic()
        {
            var request = $"api/ChatMessage/CreateRoom";

            var newRoom = new Room
            {
                Name = "Salinha da bagunça"
            };
            // Arrange
            var json = JsonConvert.SerializeObject(value: newRoom);
            var c = new StringContent(content: json);
            c.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue(mediaType: "application/json");

            // Act
            var response = await _clientServer.PostAsync(request, c);
            var room = JsonConvert.DeserializeObject<Room>(response.Content.ReadAsStringAsync().Result);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("Salinha da bagunça", room.Name);
            Assert.Equal(RoomType.Public, room.Type);
            Assert.Empty(room.Users);
        }

        [Fact]
        public async Task TestCreateRoomPublicExistRoom()
        {
            var request = $"api/ChatMessage/CreateRoom";

            var newRoom = new Room
            {
                Name = "Sala da Galera"
            };
            // Arrange
            var json = JsonConvert.SerializeObject(value: newRoom);
            var c = new StringContent(content: json);
            c.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue(mediaType: "application/json");

            // Act
            var response = await _clientServer.PostAsync(request, c);
            var room = JsonConvert.DeserializeObject<Room>(response.Content.ReadAsStringAsync().Result);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("Sala da Galera", room.Name);
            Assert.Equal(RoomType.Public, room.Type);
            Assert.Equal(3, room.Users.Count);
        }

        [Fact]
        public async Task TestCreateRoomPublicNullRoomName()
        {
            var request = $"api/ChatMessage/CreateRoom";

            var newRoom = new Room();
            // Arrange
            var json = JsonConvert.SerializeObject(value: newRoom);
            var c = new StringContent(content: json);
            c.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue(mediaType: "application/json");

            // Act
            var response = await _clientServer.PostAsync(request, c);
            var room = JsonConvert.DeserializeObject<Room>(response.Content.ReadAsStringAsync().Result);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Null(room);
        }

        #endregion

        #region CreatePrivateRoom

        [Fact]
        public async Task TestCreateRoomPrivate()
        {
            var request = $"api/ChatMessage/CreatePrivateRoom";

            var newRoom = new Room
            {
                Name = "Salinha da bagunça",
                Users = new SynchronizedCollection<User>()
                {
                    ServerInfoStore.Users[0],
                    ServerInfoStore.Users[1]
                }
            };
            // Arrange
            var json = JsonConvert.SerializeObject(value: newRoom);
            var c = new StringContent(content: json);
            c.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue(mediaType: "application/json");

            // Act
            var response = await _clientServer.PostAsync(request, c);
            var room = JsonConvert.DeserializeObject<Room>(response.Content.ReadAsStringAsync().Result);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("Salinha da bagunça", room.Name);
            Assert.Equal(RoomType.Private, room.Type);
            Assert.Equal(2, room.Users.Count);
        }

        [Fact]
        public async Task TestCreateRoomPrivateExistRoom()
        {
            var request = $"api/ChatMessage/CreatePrivateRoom";

            var newRoom = new Room
            {
                Name = "Juquinha-Carlinhos"
            };
            // Arrange
            var json = JsonConvert.SerializeObject(value: newRoom);
            var c = new StringContent(content: json);
            c.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue(mediaType: "application/json");

            // Act
            var response = await _clientServer.PostAsync(request, c);
            var room = JsonConvert.DeserializeObject<Room>(response.Content.ReadAsStringAsync().Result);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Null(room);
        }

        [Fact]
        public async Task TestCreateRoomPrivateNullRoomName()
        {
            var request = $"api/ChatMessage/CreatePrivateRoom";

            var newRoom = new Room();
            // Arrange
            var json = JsonConvert.SerializeObject(value: newRoom);
            var c = new StringContent(content: json);
            c.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue(mediaType: "application/json");

            // Act
            var response = await _clientServer.PostAsync(request, c);
            var room = JsonConvert.DeserializeObject<Room>(response.Content.ReadAsStringAsync().Result);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Null(room);
        }

        #endregion

        #region CreateUser

        [Fact]
        public async Task TestCreateUser()
        {
            var request = $"api/ChatMessage/CreateUser";

            var user = new User()
            {
                Name = "Joãozinho",
                Address = "https://localhost:10007"
            };
            // Arrange
            var json = JsonConvert.SerializeObject(value: user);
            var c = new StringContent(content: json);
            c.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue(mediaType: "application/json");

            // Act
            var response = await _clientServer.PostAsync(request, c);
            var result = JsonConvert.DeserializeObject<bool>(response.Content.ReadAsStringAsync().Result);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.True(result);
        }

        [Fact]
        public async Task TestCreateUserNullAddress()
        {
            var request = $"api/ChatMessage/CreateUser";

            var user = new User()
            {
                Name = "Joãozinho"
            };
            // Arrange
            var json = JsonConvert.SerializeObject(value: user);
            var c = new StringContent(content: json);
            c.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue(mediaType: "application/json");

            // Act
            var response = await _clientServer.PostAsync(request, c);
            var result = JsonConvert.DeserializeObject<bool>(response.Content.ReadAsStringAsync().Result);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.False(result);
        }

        [Fact]
        public async Task TestCreateUserNullUser()
        {
            var request = $"api/ChatMessage/CreateUser";

            var user = new User()
            {
                Address = "https://localhost:10007"
            };
            // Arrange
            var json = JsonConvert.SerializeObject(value: user);
            var c = new StringContent(content: json);
            c.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue(mediaType: "application/json");

            // Act
            var response = await _clientServer.PostAsync(request, c);
            var result = JsonConvert.DeserializeObject<bool>(response.Content.ReadAsStringAsync().Result);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.False(result);
        }

        [Fact]
        public async Task TestCreateUserExistUser()
        {
            var request = $"api/ChatMessage/CreateUser";

            var user = new User()
            {
                Name = "Juquinha",
                Address = "https://localhost:10007"
            };
            // Arrange
            var json = JsonConvert.SerializeObject(value: user);
            var c = new StringContent(content: json);
            c.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue(mediaType: "application/json");

            // Act
            var response = await _clientServer.PostAsync(request, c);
            var result = JsonConvert.DeserializeObject<bool>(response.Content.ReadAsStringAsync().Result);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.False(result);
        }

        #endregion

        #region DeleteUser

        [Fact]
        public async Task TestDeleteUserExistUser()
        {
            var request = $"api/ChatMessage/DeleteUser";

            var user = ServerInfoStore.Users[0];
            // Arrange
            var json = JsonConvert.SerializeObject(value: user);
            var c = new StringContent(content: json);
            c.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue(mediaType: "application/json");

            // Act
            var response = await _clientServer.PostAsync(request, c);
            var result = JsonConvert.DeserializeObject<bool>(response.Content.ReadAsStringAsync().Result);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.True(result);
            Assert.Equal(2, ServerInfoStore.Rooms[1].Users.Count);
            Assert.Single(ServerInfoStore.Rooms[4].Users);
        }

        [Fact]
        public async Task TestDeleteUserNotExistUser()
        {
            var request = $"api/ChatMessage/DeleteUser";

            var user = new User { Name = "Dani" };
            // Arrange
            var json = JsonConvert.SerializeObject(value: user);
            var c = new StringContent(content: json);
            c.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue(mediaType: "application/json");

            // Act
            var response = await _clientServer.PostAsync(request, c);
            var result = JsonConvert.DeserializeObject<bool>(response.Content.ReadAsStringAsync().Result);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.True(result);
        }

        [Fact]
        public async Task TestDeleteUserNullUserName()
        {
            var request = $"api/ChatMessage/DeleteUser";

            var user = new User();
            // Arrange
            var json = JsonConvert.SerializeObject(value: user);
            var c = new StringContent(content: json);
            c.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue(mediaType: "application/json");

            // Act
            var response = await _clientServer.PostAsync(request, c);
            var result = JsonConvert.DeserializeObject<bool>(response.Content.ReadAsStringAsync().Result);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.False(result);
        }

        [Fact]
        public async Task TestDeleteUserNullUser()
        {
            var request = $"api/ChatMessage/DeleteUser";

            // Arrange
            var c = new StringContent(content: "{\"user\": null}");
            c.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue(mediaType: "application/json");

            // Act
            var response = await _clientServer.PostAsync(request, c);
            var result = JsonConvert.DeserializeObject<bool>(response.Content.ReadAsStringAsync().Result);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.False(result);
        }

        #endregion

        #region PushMessages

        [Fact]
        public async Task TestPushMessages()
        {
            var roomNumber = 0;
            var userNumber = 0;
            var roomName = ServerInfoStore.Rooms[roomNumber].Name;

            var endpoint = $"api/Main/ReceiveMessage?roomName={roomName}";
            var mockHttp = new MockHttpMessageHandler();

            foreach (var user in ServerInfoStore.Users)
            {
                // Setup a respond for the user api (including a wildcard in the URL)
                mockHttp.When($"{user.Address}/{endpoint}")
                    .Respond("application/json",
                        @"{}"
                    ); // Respond with JSON
            }

            ServerInfoStore.ClientRequest = new ClientRequest(httpMessageHandler: mockHttp);

            // Arrange
            var request = $"api/ChatMessage/PushMessages?roomName={roomName}";

            var message = new Message
            {
                UserSource = "juquinha",
                Text = "eita!! deu ruim"
            };

            var json = JsonConvert.SerializeObject(value: message);
            var c = new StringContent(content: json);
            c.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue(mediaType: "application/json");

            // Act
            var response = await _clientServer.PostAsync(request, c);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task TestPushMessagesMessageNull()
        {
            var roomNumber = 0;
            var roomName = ServerInfoStore.Rooms[roomNumber].Name;
            
            // Arrange
            var request = $"api/ChatMessage/PushMessages?roomName={roomName}";
            
            var c = new StringContent(content: "{\"message\":null}");
            c.Headers.ContentType =
                    new System.Net.Http.Headers.MediaTypeHeaderValue(mediaType: "application/json");

            // Act
            var response = await _clientServer.PostAsync(request, c);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task TestPushMessagesNullRoomName()
        {
            // Arrange
            var request = $"api/ChatMessage/PushMessages?roomName=";

            var message = new Message
            {
                UserSource = "juquinha",
                Text = "eita!! deu ruim"
            };

            var json = JsonConvert.SerializeObject(value: message);
            var c = new StringContent(content: json);
            c.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue(mediaType: "application/json");

            // Act
            var response = await _clientServer.PostAsync(request, c);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        #endregion

        #endregion


    }
}
