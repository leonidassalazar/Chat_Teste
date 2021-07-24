using Chat.Core.Annotations;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Chat.Core.Models
{
    public class User : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string Name { get; set; }
        public string Address { get; set; }
        [JsonIgnore]
        public ObservableCollection<Room> Rooms { get; }

        private readonly object _lockRooms;

        public User()
        {
            _lockRooms = new object();
            Rooms = new ObservableCollection<Room>();
        }

        public void AddRoom(Room room)
        {
            lock (_lockRooms)
            {
                Rooms.Add(room);
            }
        }
        
        public void RemoveRoom(Room room)
        {
            lock (_lockRooms)
            {
                Rooms.Remove(room);
            }
        }
        
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
