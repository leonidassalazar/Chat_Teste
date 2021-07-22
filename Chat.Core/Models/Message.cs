using System;

namespace Chat.Core.Models
{
    public class Message
    {
        public string UserSource { get; set; }
        public string UserDest { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }

        public Message()
        {
            Date = DateTime.Now;
        }
    }
}
