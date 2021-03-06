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
    public class ListRoomsCommandTests : BaseClassTests
    {
        [Fact]
        public void ExecuteTest()
        {
            var mockHttp = new MockHttpMessageHandler();
            var rooms = JsonConvert.SerializeObject(ClientInfoStore.User.Rooms.Select(q => q.Name));
            mockHttp.When($"http://localost:5001/api/ChatMessage/GetRooms?userName={ClientInfoStore.User}")
                        .Respond("application/json", rooms); // Respond with JSON
            var serverClient = new HttpClient(mockHttp)
            {
                BaseAddress = new Uri("http://localost:5001")
            };

            ClientInfoStore.ServerRequest = new ServerRequest(hostUrl: ClientInfoStore.User.Address, client: serverClient);

            var listRoomsCommand = new ListRoomsCommand();

            var complement = "Não esta em uso";
            var message = new Message();

            var result = listRoomsCommand.Execute(complement, ref message, null);

            Assert.False(result);
        }

        [Fact]
        public void ExecuteTestNullComplement()
        {
            var mockHttp = new MockHttpMessageHandler();
            var rooms = JsonConvert.SerializeObject(ClientInfoStore.User.Rooms.Select(q => q.Name));
            mockHttp.When($"http://localost:5001/api/ChatMessage/GetRooms?userName={ClientInfoStore.User}")
                .Respond("application/json", rooms); // Respond with JSON
            var serverClient = new HttpClient(mockHttp)
            {
                BaseAddress = new Uri("http://localost:5001")
            };

            ClientInfoStore.ServerRequest = new ServerRequest(hostUrl: ClientInfoStore.User.Address, client: serverClient);

            var listRoomsCommand = new ListRoomsCommand();
            
            var message = new Message();

            var result = listRoomsCommand.Execute(null, ref message, null);

            Assert.False(result);
        }
        [Fact]
        public void ExecuteTestNullMessage()
        {
            var mockHttp = new MockHttpMessageHandler();
            var rooms = JsonConvert.SerializeObject(ClientInfoStore.User.Rooms.Select(q => q.Name));
            mockHttp.When($"http://localost:5001/api/ChatMessage/GetRooms?userName={ClientInfoStore.User}")
                .Respond("application/json", rooms); // Respond with JSON
            var serverClient = new HttpClient(mockHttp)
            {
                BaseAddress = new Uri("http://localost:5001")
            };

            ClientInfoStore.ServerRequest = new ServerRequest(hostUrl: ClientInfoStore.User.Address, client: serverClient);

            var listRoomsCommand = new ListRoomsCommand();

            var complement = "Não esta em uso";
            Message message = null;

            var result = listRoomsCommand.Execute(complement, ref message, null);

            Assert.False(result);
        }

        [Fact]
        public void ExecuteTestNullComplementNullMessage()
        {
            var mockHttp = new MockHttpMessageHandler();
            var rooms = JsonConvert.SerializeObject(ClientInfoStore.User.Rooms.Select(q => q.Name));
            mockHttp.When($"http://localost:5001/api/ChatMessage/GetRooms?userName={ClientInfoStore.User}")
                .Respond("application/json", rooms); // Respond with JSON
            var serverClient = new HttpClient(mockHttp)
            {
                BaseAddress = new Uri("http://localost:5001")
            };

            ClientInfoStore.ServerRequest = new ServerRequest(hostUrl: ClientInfoStore.User.Address, client: serverClient);

            var listRoomsCommand = new ListRoomsCommand();
            
            Message message = null;

            var result = listRoomsCommand.Execute(null, ref message, null);

            Assert.False(result);
        }
        
    }
}