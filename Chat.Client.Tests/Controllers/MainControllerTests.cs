using Chat.Client.BL;
using Chat.Core.Enum;
using Chat.Core.Models;
using Newtonsoft.Json;
using RichardSzalay.MockHttp;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Chat.Client.Tests.Controllers
{
    public class MainControllerTests : IClassFixture<TestFixture<Startup>>
    {
        private readonly HttpClient _client;

        public MainControllerTests(TestFixture<Startup> fixture)
        {
            _client = fixture.Client;

            var room = new Room
            {
                Name = "general",
                State = StateEnum.Active,
                Type = RoomType.Public
            };

            ClientInfoStore.User = new User
            {
                Name = "Juca",
                Address = "https://localhost:10000"
            };

            ClientInfoStore.User.AddRoom(room);
        }

        [Fact]
        public async Task TestReceiveMessageSuccess()
        {
            // Arrange
            const string request = "api/Main/ReceiveMessage?roomName=general";

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
            var response = await _client.PostAsync(request, c);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task TestReceiveMessageNotJoinedRoom()
        {
            const string roomName = "banana";
            var endpoint = $"api/ChatMessage/EnterRoom?roomName={roomName}&userName={ClientInfoStore.User.Name}";
            var mockHttp = new MockHttpMessageHandler();

            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When($"http://localost:5001/{endpoint}")
                .Respond("application/json",
                    @"{" +
                                "\"name\": \"banana\"," +
                                "\"type\": 0" +
                            "}"
                    ); // Respond with JSON
            var serverClient = new HttpClient(mockHttp)
            {
                BaseAddress = new Uri("http://localost:5001")
            };

            ClientInfoStore.ServerRequest = new ServerRequest(hostUrl: ClientInfoStore.User.Address, client: serverClient);

            // Arrange
            var request = $"api/Main/ReceiveMessage?roomName={roomName}";

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
            var response = await _client.PostAsync(request, c);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task TestNewUserNotification()
        {
            // Arrange
            const string request = "api/Main/NotifyNewUser";

            var message = new Message
            {
                Text = "Carlinhos entrou na #carlinhos-juquinha"
            };
            var json = JsonConvert.SerializeObject(value: message);
            var c = new StringContent(content: json);
            c.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue(mediaType: "application/json");

            // Act
            var response = await _client.PostAsync(request, c);

            // Assert
            response.EnsureSuccessStatusCode();
        }

    }
}
