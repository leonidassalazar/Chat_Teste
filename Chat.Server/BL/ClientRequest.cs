using Chat.Core.Models;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;

namespace Chat.Server.BL
{
    public class ClientRequest
    {
        private readonly HttpMessageHandler _httpMessageHandler;

        public ClientRequest(HttpMessageHandler httpMessageHandler)
        {
            _httpMessageHandler = httpMessageHandler;
        }

        //TODO: Refatorar os metodos NotifyNewUser e PushMessages, para que usem um metodo em comum
        //TODO: uma vez que seus codigo são praticamente iguais

        public void NotifyNewUser(string roomName, string userName)
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

                    var client = new HttpClient(_httpMessageHandler)
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

        public void PushMessages(string roomName, Message message)
        {
            var room = ServerInfoStore.Rooms.FirstOrDefault(q => string.Equals(q.Name, roomName, StringComparison.InvariantCultureIgnoreCase))
                       ?? new Room();

            foreach (var user in room.Users.Where(q => !string.Equals(q.Name, message.UserSource, StringComparison.CurrentCultureIgnoreCase)))
            {
                try
                {
                    var json = JsonConvert.SerializeObject(value: message);
                    var c = new StringContent(content: json);
                    c.Headers.ContentType =
                        new System.Net.Http.Headers.MediaTypeHeaderValue(mediaType: "application/json");

                    var client = new HttpClient(_httpMessageHandler)
                    {
                        BaseAddress = new Uri(uriString: user.Address)
                    };
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
    }
}
