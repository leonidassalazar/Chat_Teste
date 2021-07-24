using System;
using Chat.Core.Annotations;
using Chat.Core.Models;

namespace Chat.Client.Views.Commands
{
    public class ToCommand : ICommand
    {
        public bool Execute(string complement, [NotNull] ref Message message, Action<string> action)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));
            if (!string.IsNullOrEmpty(complement))
            {
                //TODO: Add user's existence validation
                message.UserDest = complement;
            }
            else
            {
                Console.WriteLine("The command /p needs a complement. View more with /h");
            }
            return false;
        }

        public string CommandDescription()
        {
            return $"/t [recipient user]  -  Send a public message to the recipient user";
        }
    }
}