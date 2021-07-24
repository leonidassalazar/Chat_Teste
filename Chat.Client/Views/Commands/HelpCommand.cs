using Chat.Core.Models;
using System;

namespace Chat.Client.Views.Commands
{
    public class HelpCommand : ICommand
    {
        public bool Execute(string complement, ref Message message, Action<string> action)
        {
            foreach (var command in CommandStrategy.CommandStrategies.Values)
            {
                Console.WriteLine(command.CommandDescription());
            }
            return false;
        }

        public string CommandDescription()
        {
            return $"/h  - List description of all commands";
        }
    }
}
