using System;

namespace lcfrConsoleApp
{
    public class RosterCommandHandler
    {
        public void HandleCommand(string[] inputParts)
        {
            string subCommand = inputParts.Length > 1 ? inputParts[1].ToLower() : "";
            switch (subCommand)
            {
                case "help":
                    Console.WriteLine("Help for roster commands:");
                    // Display help information for roster commands
                    break;
                
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Invalid command under 'roster' type: {subCommand}");
                    Console.ResetColor();
                    break;
            }
        }
    }
}