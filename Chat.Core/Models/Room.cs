using Chat.Core.Annotations;
using Chat.Core.Enum;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Chat.Core.Utils;

namespace Chat.Core.Models
{
    public class Room : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        private string _name;
        private DateTime _lastView;
        private SynchronizedCollection<Message> _messages;
        private SynchronizedCollection<User> _users;
        private StateEnum _state;
        private RoomType _type;

        private readonly object _lockMessages;

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

        public RoomType Type
        {
            get => _type;
            set
            {
                if (value == _type) return;
                _type = value;
                OnPropertyChanged();
            }
        }

        public StateEnum State
        {
            get => _state;
            set
            {
                if (value == _state) return;
                _state = value;
                OnPropertyChanged();
            }
        }

        public SynchronizedCollection<User> Users
        {
            get => _users;
            set
            {
                if (Equals(value, _users)) return;
                _users = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public SynchronizedCollection<Message> Messages
        {
            get => _messages;
            set
            {
                if (Equals(value, _messages)) return;
                _messages = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public DateTime LastView
        {
            get => _lastView;
            set
            {
                if (value.Equals(_lastView)) return;
                _lastView = value;
                OnPropertyChanged();
            }
        }

        public Room()
        {
            Messages = new SynchronizedCollection<Message>();
            Users = new SynchronizedCollection<User>();
            _lockMessages = new object();
        }

        public Room(string name, RoomType type) : this()
        {
            Name = name;
            Type = type;
        }

        public void AddMessage(Message message)
        {
            lock (_lockMessages)
            {
                Messages.Add(message);
                OnMessageReceived(message);
            }
        }


        [NotifyPropertyChangedInvocator]
        protected virtual void OnMessageReceived(Message message)
        {
            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(message, Name));
        }



        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
