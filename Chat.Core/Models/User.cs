using Chat.Core.Annotations;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Chat.Core.Models
{
    public class User : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Name
        {
            get => _name;
            set
            {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        public string Address
        {
            get => _address;
            set
            {
                if (value == _address) return;
                _address = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public SynchronizedCollection<Room> Rooms { get; }

        private readonly object _lockRooms;
        private string _name;
        private string _address;

        public User()
        {
            _lockRooms = new object();
            Rooms = new SynchronizedCollection<Room>();
        }

        public void AddRoom(Room room)
        {
            lock (_lockRooms)
            {
                Rooms.Add(room);
                OnPropertyChanged("Rooms");
            }
        }

        public void RemoveRoom(Room room)
        {
            lock (_lockRooms)
            {
                Rooms.Remove(room);
                OnPropertyChanged("Rooms");
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
