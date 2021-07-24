using System;
using Chat.Core.Annotations;
using Chat.Core.Models;

namespace Chat.Client.Views.Commands
{
    public class MessageCommand : ICommand
    {
        public bool Execute([NotNull] string complement, [NotNull] ref Message message, [NotNull] Action<string> action)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (string.IsNullOrEmpty(complement))
            {
                Console.WriteLine("The message can't be empty");
                return false;
            }

            message.Text = complement;
            return true;
        }

        public string CommandDescription()
        {
            return $"/m [message text]  -  Indicate that the message starts, must be used when using with other command";
        }
    }
}