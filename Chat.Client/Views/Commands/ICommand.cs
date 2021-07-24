using System;
using Chat.Core.Models;

namespace Chat.Client.Views.Commands
{
    public interface ICommand
    {
        bool Execute(string complement, ref Message message, Action<string> action);

        string CommandDescription();
    }
}
