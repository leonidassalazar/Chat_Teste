using Chat.Client.BL;
using Chat.Client.Views.Commands;
using Chat.Core.Enum;
using Chat.Core.Models;
using RichardSzalay.MockHttp;
using System;
using System.Net.Http;
using Xunit;
using Assert = Xunit.Assert;

namespace Chat.Client.Tests.Views.Commands
{
    [Collection("Group 1")]
    public class ChangeRoomCommandTests : BaseClassTests
    {
        [Fact]
        [BaseClassTests]
        public void ExecuteTest()
        {
            const string roomName = "banana";
            var endpoint = $"api/ChatMessage/EnterRoom?roomName={roomName}&userName={ClientInfoStore.User.Name}";
            var mockHttp = new MockHttpMessageHandler();

            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When($"http://localost:5001/{endpoint}")
                .Respond("application/json",
                    @"{" +
                                "\"name\": \"Fundão\"," +
                                "\"type\": 0" +
                            "}"
                    ); // Respond with JSON
            var serverClient = new HttpClient(mockHttp)
            {
                BaseAddress = new Uri("http://localost:5001")
            };

            ClientInfoStore.ServerRequest = new ServerRequest(hostUrl: ClientInfoStore.User.Address, client: serverClient);

            var changeRoomCommand = new ChangeRoomCommand();

            var complement = "Fundão";
            var message = new Message();

            var result = changeRoomCommand.Execute(complement, ref message, null);

            Assert.False(result);
            Assert.Equal(StateEnum.Deactivated, ClientInfoStore.User.Rooms[0].State);
            Assert.Equal(StateEnum.Active, ClientInfoStore.User.Rooms[1].State);
        }

        [Fact]
        [BaseClassTests]
        public void ExecuteTestNullComplement()
        {
            const string roomName = "banana";
            var endpoint = $"api/ChatMessage/EnterRoom?roomName={roomName}&userName={ClientInfoStore.User.Name}";
            var mockHttp = new MockHttpMessageHandler();

            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When($"http://localost:5001/{endpoint}")
                .Respond("application/json",
                    @"{" +
                                "\"name\": \"Fundão\"," +
                                "\"type\": 0" +
                            "}"
                    ); // Respond with JSON
            var serverClient = new HttpClient(mockHttp)
            {
                BaseAddress = new Uri("http://localost:5001")
            };

            ClientInfoStore.ServerRequest = new ServerRequest(hostUrl: ClientInfoStore.User.Address, client: serverClient);

            var changeRoomCommand = new ChangeRoomCommand();

            var message = new Message();

            var result = changeRoomCommand.Execute(null, ref message, null);

            Assert.False(result);
            Assert.Equal(StateEnum.Deactivated, ClientInfoStore.User.Rooms[0].State);
            Assert.Equal(StateEnum.Active, ClientInfoStore.User.Rooms[1].State);
        }

        [Fact]
        [BaseClassTests]
        public void ExecuteTestNullMessage()
        {
            const string roomName = "banana";
            var endpoint = $"api/ChatMessage/EnterRoom?roomName={roomName}&userName={ClientInfoStore.User.Name}";
            var mockHttp = new MockHttpMessageHandler();

            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When($"http://localost:5001/{endpoint}")
                .Respond("application/json",
                    @"{" +
                                "\"name\": \"Fundão\"," +
                                "\"type\": 0" +
                            "}"
                    ); // Respond with JSON
            var serverClient = new HttpClient(mockHttp)
            {
                BaseAddress = new Uri("http://localost:5001")
            };

            ClientInfoStore.ServerRequest = new ServerRequest(hostUrl: ClientInfoStore.User.Address, client: serverClient);

            var changeRoomCommand = new ChangeRoomCommand();

            var complement = "Fundão";
            Message message = null;

            var result = changeRoomCommand.Execute(complement, ref message, null);

            Assert.False(result);
            Assert.Equal(StateEnum.Deactivated, ClientInfoStore.User.Rooms[0].State);
            Assert.Equal(StateEnum.Active, ClientInfoStore.User.Rooms[1].State);
        }

    }
}