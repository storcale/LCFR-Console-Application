using System;

namespace lcfrConsoleApp
{
    public class DiscordCommandHandler
    {
        public async Task HandleCommand(string[] inputParts)
        {
            string subCommand = inputParts.Length > 1 ? inputParts[1].ToLower() : "";
            switch (subCommand)
            {
                case "shiftadd":
                    if (inputParts.Length < 3)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Please specify at least two parameters: host, username");
                        Console.ResetColor();
                        break;
                    }
                    string host = inputParts[1].ToLower();
                    string username = inputParts[2].ToLower();
                    shiftAdd(host, username);
                    break;
                // Add more cases for other commands under the discord type
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Invalid command under 'discord' type: {subCommand}");
                    Console.ResetColor();
                    break;
            }
        }

        private void shiftAdd(string host, string username)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n  ShiftAdd: {host} {username}");
            Console.ResetColor();
        }
    }
}
