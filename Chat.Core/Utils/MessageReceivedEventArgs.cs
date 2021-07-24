using Chat.Core.Models;
using System;

namespace Chat.Core.Utils
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public Message Message { get; }
        public string RoomName { get; }

        public MessageReceivedEventArgs(Message message, string roomName)
        {
            Message = message;
            RoomName = roomName;
        }

    }
}
