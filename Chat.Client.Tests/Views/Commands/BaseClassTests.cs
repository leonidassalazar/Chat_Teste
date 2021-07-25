﻿using Chat.Core.Enum;
using Chat.Core.Models;
using System.Reflection;
using Xunit.Sdk;

namespace Chat.Client.Tests.Views.Commands
{
    public class BaseClassTests : BeforeAfterTestAttribute
    {
        public override void Before(MethodInfo methodUnderTest)
        {
            var room = new Room
            {
                Name = "general",
                State = StateEnum.Deactivated,
                Type = RoomType.Public
            };

            var room2 = new Room
            {
                Name = "Fundão",
                State = StateEnum.Active,
                Type = RoomType.Public
            };

            ClientInfoStore.User = new User
            {
                Name = "Juca",
                Address = "https://localhost:10000"
            };

            ClientInfoStore.User.AddRoom(room);
            ClientInfoStore.User.AddRoom(room2);
        }

    }
}
