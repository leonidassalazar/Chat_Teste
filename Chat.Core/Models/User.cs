using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Chat.Core.Models
{
    public class User
    {
        public string Name { get; set; }
        public string Address { get; set; }
        [JsonIgnore]
        public SynchronizedCollection<Room> Rooms { get; }

        public User()
        {
            Rooms = new SynchronizedCollection<Room>();
        }

    }
}
