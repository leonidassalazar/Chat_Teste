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
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ToPrivatelyCommandTests : BaseClassTests
    {
        [Fact]
        [BaseClassTests]
        public void ExecuteTestNotExistRoom()
        {
            var endpoint = $"api/ChatMessage/CreatePrivateRoom";
            var mockHttp = new MockHttpMessageHandler();

            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When($"http://localost:5001/{endpoint}")
                .Respond("application/json",
                    @"{" +
                                "\"Name\": \"Juca-Marco\"," +
                                "\"type\": 0" +
                            "}"
                    ); // Respond with JSON
            var serverClient = new HttpClient(mockHttp)
            {
                BaseAddress = new Uri("http://localost:5001")
            };

            ClientInfoStore.ServerRequest = new ServerRequest(hostUrl: ClientInfoStore.User.Address, client: serverClient);

            var toPrivatelyCommand = new ToPrivatelyCommand();

            var complement = "Marco";
            var message = new Message();

            var result = toPrivatelyCommand.Execute(complement, ref message, null);

            Assert.False(result);
            Assert.Equal(StateEnum.Deactivated, ClientInfoStore.User.Rooms[0].State);
            Assert.Equal(StateEnum.Deactivated, ClientInfoStore.User.Rooms[1].State);
            Assert.Equal(StateEnum.Active, ClientInfoStore.User.Rooms[2].State);
        }

        [Fact]
        [BaseClassTests]
        public void ExecuteTestExistRoom()
        {
            ClientInfoStore.ServerRequest = new ServerRequest(hostUrl: ClientInfoStore.User.Address, client: new HttpClient());
            ClientInfoStore.User.AddRoom(new Room
            {
                Name = "Juca-Marco",
                Type = RoomType.Private
            });

            var toPrivatelyCommand = new ToPrivatelyCommand();

            var complement = "Marco";
            var message = new Message();

            var result = toPrivatelyCommand.Execute(complement, ref message, null);

            Assert.False(result);
            Assert.Equal(StateEnum.Deactivated, ClientInfoStore.User.Rooms[0].State);
            Assert.Equal(StateEnum.Deactivated, ClientInfoStore.User.Rooms[1].State);
            Assert.Equal(StateEnum.Active, ClientInfoStore.User.Rooms[2].State);
        }

        [Fact]
        [BaseClassTests]
        public void ExecuteTestNullComplement()
        {
            var toPrivatelyCommand = new ToPrivatelyCommand();

            var message = new Message();

            var result = toPrivatelyCommand.Execute(null, ref message, null);

            Assert.False(result);
        }

        [Fact]
        [BaseClassTests]
        public void ExecuteTestNotExistRoomNullMessage()
        {
            var endpoint = $"api/ChatMessage/CreatePrivateRoom";
            var mockHttp = new MockHttpMessageHandler();

            // Setup a respond for the user api (including a wildcard in the URL)
            mockHttp.When($"http://localost:5001/{endpoint}")
                .Respond("application/json",
                    @"{" +
                                "\"Name\": \"Juca-Marco\"," +
                                "\"type\": 0" +
                            "}"
                    ); // Respond with JSON
            var serverClient = new HttpClient(mockHttp)
            {
                BaseAddress = new Uri("http://localost:5001")
            };

            ClientInfoStore.ServerRequest = new ServerRequest(hostUrl: ClientInfoStore.User.Address, client: serverClient);

            var toPrivatelyCommand = new ToPrivatelyCommand();

            var complement = "Marco";
            Message message = null;

            var result = toPrivatelyCommand.Execute(complement, ref message, null);

            Assert.False(result);
            Assert.Equal(StateEnum.Deactivated, ClientInfoStore.User.Rooms[0].State);
            Assert.Equal(StateEnum.Deactivated, ClientInfoStore.User.Rooms[1].State);
            Assert.Equal(StateEnum.Active, ClientInfoStore.User.Rooms[2].State);
        }

        [Fact]
        [BaseClassTests]
        public void ExecuteTestExistRoomNullMessage()
        {
            ClientInfoStore.ServerRequest = new ServerRequest(hostUrl: ClientInfoStore.User.Address, client: new HttpClient());
            ClientInfoStore.User.AddRoom(new Room
            {
                Name = "Juca-Marco",
                Type = RoomType.Private
            });

            var toPrivatelyCommand = new ToPrivatelyCommand();

            var complement = "Marco";
            Message message = null;

            var result = toPrivatelyCommand.Execute(complement, ref message, null);

            Assert.False(result);
            Assert.Equal(StateEnum.Deactivated, ClientInfoStore.User.Rooms[0].State);
            Assert.Equal(StateEnum.Deactivated, ClientInfoStore.User.Rooms[1].State);
            Assert.Equal(StateEnum.Active, ClientInfoStore.User.Rooms[2].State);
        }


    }
}