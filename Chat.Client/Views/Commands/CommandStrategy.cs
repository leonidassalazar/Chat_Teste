using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chat.Core.Models;

namespace Chat.Client.Views.Commands
{
    public class CommandStrategy
    {
        internal static readonly Dictionary<string, ICommand> CommandStrategies = new();

        static CommandStrategy()
        {
            CommandStrategies.Add("/t", new ToCommand());
            CommandStrategies.Add("/p", new ToPrivatelyCommand());
            CommandStrategies.Add("/lr", new ListRoomsCommand());
            CommandStrategies.Add("/lu", new ListUsersCommand());
            CommandStrategies.Add("/exit", new ExitCommand());
            CommandStrategies.Add("/ch", new ChangeRoomCommand());
            CommandStrategies.Add("/cr", new CreateRoomCommand());
            CommandStrategies.Add("/m", new MessageCommand());
            CommandStrategies.Add("/h", new HelpCommand());
        }
        
        public static ICommand GetCommand(string command)
        {
            return CommandStrategies[command];
        }
    }
}
