using System;
using System.Collections.Generic;
using Chat.Core.Models;

namespace Chat.Client.Views.Commands
{
    public class ListRoomsCommand : ICommand
    {
        public bool Execute(string complement, ref Message message, Action<string> action)
        {
            var rooms = ClientInfoStore.ServerRequest.GetRooms();
            //TODO: Print the rooms on console
            PrintList(rooms);
            return false;
        }

        public string CommandDescription()
        {
            return $"/lr  -  List all public rooms and user's private rooms";
        }

        private void PrintList(IEnumerable<string> data)
        {
            Console.WriteLine("All rooms: ");
            foreach (var d in data)
            {
                Console.WriteLine(d);
            }
        }

    }
}