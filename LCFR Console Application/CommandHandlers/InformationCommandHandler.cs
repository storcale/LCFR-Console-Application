using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;

namespace lcfrConsoleApp
{
    public class InformationCommandHandler
    {
        private static Dictionary<string, Dictionary<string, string>> information;


        public async Task HandleCommand(string[] inputParts, Dictionary<string, Dictionary<string, string>> commands)
        {
            

            if (commands == null)
            {
                Console.WriteLine("No commands loaded.");
                return;
            }

            string subCommand = inputParts.Length > 1 ? inputParts[1].ToLower() : "";
            switch (subCommand)
            {
                case "help":
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine($"\n Help for commands:");
                    Console.ResetColor();
                    Console.WriteLine($"------------------------\n");
                    Console.WriteLine("use [type] [command] (parameters) to use a command");

                    var informationCommands = commands["information"];

                    foreach(var type in commands)
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine($"\n Commands for {type.Key} type: ");
                        foreach(var command in type.Value)
                        { 
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine($"- {command.Key}: {command.Value}");
                        }
                        Console.ResetColor();
                    }
                    break;

                case "version":
                    Console.WriteLine("Display version information:");
                    // Handle version command for Information type
                    break;
                // Add more cases for other commands under the information type
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Invalid command under 'information' type: {subCommand}");
                    Console.ResetColor();
                    break;
            }
        }
        

    }
}

