using Chat.Core.Enum;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Chat.Core.Models
{
    public class Room
    {
        public string Name { get; set; }
        public RoomType Type { get; set; }
        public StateEnum State { get; set; }
        [JsonIgnore]
        public SynchronizedCollection<Message> Messages { get; set; }
        [JsonIgnore]
        public SynchronizedCollection<User> Users { get; set; }
        [JsonIgnore]
        public DateTime LastView { get; set; }

        public Room()
        {
            Messages = new SynchronizedCollection<Message>();
            Users = new SynchronizedCollection<User>();
        }

        public Room(string name, RoomType type) : this()
        {
            Name = name;
            Type = type;
        }

    }
}
