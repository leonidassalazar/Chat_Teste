using Chat.Client.BL;
using Chat.Client.Views.Commands;
using Chat.Core.Enum;
using Chat.Core.Models;
using RichardSzalay.MockHttp;
using System;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using Xunit;
using Assert = Xunit.Assert;

namespace Chat.Client.Tests.Views.Commands
{
    public class ListUsersCommandTests : BaseClassTests
    {
        [Fact]
        public void ExecuteTest()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"http://localost:5001/api/ChatMessage/GetUsers")
                        .Respond("application/json", "[\"Juca\", \"Carlos\", \"Manuel\"]"); // Respond with JSON
            var serverClient = new HttpClient(mockHttp)
            {
                BaseAddress = new Uri("http://localost:5001")
            };

            ClientInfoStore.ServerRequest = new ServerRequest(hostUrl: ClientInfoStore.User.Address, client: serverClient);

            var listUsersCommand = new ListUsersCommand();

            var complement = "Não esta em uso";
            var message = new Message();

            var result = listUsersCommand.Execute(complement, ref message, null);

            Assert.False(result);
        }

        [Fact]
        public void ExecuteTestNullComplement()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"http://localost:5001/api/ChatMessage/GetUsers")
                .Respond("application/json", "[\"Juca\", \"Carlos\", \"Manuel\"]"); // Respond with JSON
            var serverClient = new HttpClient(mockHttp)
            {
                BaseAddress = new Uri("http://localost:5001")
            };

            ClientInfoStore.ServerRequest = new ServerRequest(hostUrl: ClientInfoStore.User.Address, client: serverClient);

            var listUsersCommand = new ListUsersCommand();
            
            var message = new Message();

            var result = listUsersCommand.Execute(null, ref message, null);

            Assert.False(result);
        }
        [Fact]
        public void ExecuteTestNullMessage()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"http://localost:5001/api/ChatMessage/GetUsers")
                .Respond("application/json", "[\"Juca\", \"Carlos\", \"Manuel\"]"); // Respond with JSON
            var serverClient = new HttpClient(mockHttp)
            {
                BaseAddress = new Uri("http://localost:5001")
            };

            ClientInfoStore.ServerRequest = new ServerRequest(hostUrl: ClientInfoStore.User.Address, client: serverClient);

            var listUsersCommand = new ListUsersCommand();

            var complement = "Não esta em uso";
            Message message = null;

            var result = listUsersCommand.Execute(complement, ref message, null);

            Assert.False(result);
        }

        [Fact]
        public void ExecuteTestNullComplementNullMessage()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.When($"http://localost:5001/api/ChatMessage/GetUsers")
                .Respond("application/json", "[\"Juca\", \"Carlos\", \"Manuel\"]"); // Respond with JSON
            var serverClient = new HttpClient(mockHttp)
            {
                BaseAddress = new Uri("http://localost:5001")
            };

            ClientInfoStore.ServerRequest = new ServerRequest(hostUrl: ClientInfoStore.User.Address, client: serverClient);

            var listUsersCommand = new ListUsersCommand();

            Message message = null;

            var result = listUsersCommand.Execute(null, ref message, null);

            Assert.False(result);
        }
        
    }
}