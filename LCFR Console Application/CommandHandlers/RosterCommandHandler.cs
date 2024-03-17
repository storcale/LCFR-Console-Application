using System;
using System.Data;

namespace lcfrConsoleApp
{
    public class RosterCommandHandler
    {

        public void HandleCommand(string[] inputParts)
        {
            

            string subCommand = inputParts.Length > 1 ? inputParts[1].ToLower() : "";
            switch (subCommand)
                {
                  default:
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Invalid command under 'roster' type: {subCommand}");
                        Console.ResetColor();
                   break;
                }
            }
            
        }
    }
}