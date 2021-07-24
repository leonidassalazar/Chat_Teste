using System;
using System.Collections.Generic;
using Chat.Core.Models;

namespace Chat.Client.Views.Commands
{
    public class ListUsersCommand : ICommand
    {
        public bool Execute(string complement, ref Message message, Action<string> action)
        {
            var users = ClientInfoStore.ServerRequest.GetUsers();
            //TODO: Print the users on console
            PrintList(users);
            return false;
        }

        public string CommandDescription()
        {
            return $"/lu  -  List all online users";
        }

        private void PrintList(IEnumerable<string> data)
        {
            Console.WriteLine("All users: ");
            foreach (var d in data)
            {
                Console.WriteLine(d);
            }
        }
    }
}