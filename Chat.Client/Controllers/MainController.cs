using Chat.Core.Enum;
using Chat.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace Chat.Client.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class MainController : ControllerBase
    {
        private readonly ILogger<MainController> _logger;

        public MainController(ILogger<MainController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public void ReceiveMessage(string roomName, Message message)
        {
            var room = ClientInfoStore.User.Rooms.FirstOrDefault(q =>
                string.Equals(q.Name, roomName, StringComparison.CurrentCultureIgnoreCase));
            if (room == null)
            {
                room = ClientInfoStore.ServerRequest.EnterRoom(roomName);
                if (room == null) return;
            }

            room.AddMessage(message);
        }

        [HttpPost]
        public void NotifyNewUser(Message message)
        {
            var room = ClientInfoStore.User.Rooms.FirstOrDefault(q => q.State == StateEnum.Active);

            room?.AddMessage(message);
        }
    }
}
