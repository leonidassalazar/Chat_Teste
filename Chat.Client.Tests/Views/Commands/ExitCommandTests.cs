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
    public class ExitCommandTests : BaseClassTests
    {
        [Fact]
        public void ExecuteTest()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When($"http://localost:5001/api/ChatMessage/ExitRoom?roomName=Fundão&userName=Juca")
                        .Respond("application/json",
                            @"true"
                            ); // Respond with JSON
            var serverClient = new HttpClient(mockHttp)
            {
                BaseAddress = new Uri("http://localost:5001")
            };

            ClientInfoStore.ServerRequest = new ServerRequest(hostUrl: ClientInfoStore.User.Address, client: serverClient);

            var exitCommand = new ExitCommand();

            var complement = "Fundão";
            var message = new Message();

            var result = exitCommand.Execute(complement, ref message, null);

            Assert.False(result);
            Assert.Single(ClientInfoStore.User.Rooms);
            Assert.Equal(StateEnum.Active, ClientInfoStore.User.Rooms[0].State);
        }

        [Fact]
        public void ExecuteTestNullComplement()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"http://localost:5001/api/ChatMessage/DeleteUser")
                            .Respond("application/json",
                                @"true"); // Respond with JSON

            var serverClient = new HttpClient(mockHttp)
            {
                BaseAddress = new Uri("http://localost:5001")
            };

            ClientInfoStore.ServerRequest = new ServerRequest(hostUrl: ClientInfoStore.User.Address, client: serverClient);

            var exitCommand = new ExitCommand();

            var message = new Message();

            var result = exitCommand.Execute(null, ref message, null);

            Assert.False(result);
            Assert.Null(ClientInfoStore.User);
        }
        [Fact]
        public void ExecuteTestNullMessage()
        {
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.When($"http://localost:5001/api/ChatMessage/ExitRoom?roomName=Fundão&userName=Juca")
                        .Respond("application/json",
                            @"true"
                            ); // Respond with JSON
            var serverClient = new HttpClient(mockHttp)
            {
                BaseAddress = new Uri("http://localost:5001")
            };

            ClientInfoStore.ServerRequest = new ServerRequest(hostUrl: ClientInfoStore.User.Address, client: serverClient);

            var exitCommand = new ExitCommand();

            var complement = "Fundão";
            Message message = null;

            var result = exitCommand.Execute(complement, ref message, null);

            Assert.False(result);
            Assert.Single(ClientInfoStore.User.Rooms);
            Assert.Equal(StateEnum.Active, ClientInfoStore.User.Rooms[0].State);
        }

        [Fact]
        public void ExecuteTestNullComplementNullMessage()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"http://localost:5001/api/ChatMessage/DeleteUser")
                            .Respond("application/json",
                                @"true"); // Respond with JSON

            var serverClient = new HttpClient(mockHttp)
            {
                BaseAddress = new Uri("http://localost:5001")
            };

            ClientInfoStore.ServerRequest = new ServerRequest(hostUrl: ClientInfoStore.User.Address, client: serverClient);

            var exitCommand = new ExitCommand();

            Message message = null;

            var result = exitCommand.Execute(null, ref message, null);

            Assert.False(result);
            Assert.Null(ClientInfoStore.User);
        }
        
    }
}