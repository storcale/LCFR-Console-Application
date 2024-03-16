using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System.Data;
using System.Reflection;
using System.Runtime.InteropServices;

namespace lcfrConsoleApp
{
    class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleTitle(string lpConsoleTitle);
        private static Dictionary<string, Dictionary<string, string>> commandTypes;
        private static DiscordCommandHandler discordCommandHandler;
        private static InformationCommandHandler informationCommandHandler;
        private static RosterCommandHandler rosterCommandHandler;



        static void Main(string[] args)
        {
            // Load commands from JSON file
            LoadCommandsFromJson();

            // Initialize command handlers
            discordCommandHandler = new DiscordCommandHandler();
            informationCommandHandler = new InformationCommandHandler();
            rosterCommandHandler = new RosterCommandHandler();

            // Set the console window title
            SetConsoleTitle("LCFR API Bash");



            // Display title in the console.
            Console.WriteLine("Welcome to the LCFR API");
            Console.WriteLine("------------------------\n");
            Console.WriteLine("Enter 'ihelp' to see all commands ");

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(" ");
                Console.WriteLine("Enter your command: ");
                Console.ResetColor();

                string userInput = Console.ReadLine();

                // Split the input into command and parameters
                string[] inputParts = userInput.Split(' ');
                string commandType = inputParts[0].ToLower();
                

                
                // Check if the command type exists in the loaded commands
                if (commandTypes.ContainsKey(commandType))
                {
                    Dictionary<string, string> commands = commandTypes[commandType];
                    // Switch on the command type
                    switch (commandType)
                    {
                        
                            
                        case "information":
                            informationCommandHandler.HandleCommand(inputParts);

                            break;
                        case "discord":
                            discordCommandHandler.HandleCommand(inputParts);
                            break;
                        case "roster":
                            rosterCommandHandler.HandleCommand(inputParts);
                            break;
                        default:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Invalid command type: {commandType}");
                            Console.ResetColor();
                            break;
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Invalid command type: {commandType}");
                    Console.ResetColor();
                }
            }
        }


       

        static void LoadCommandsFromJson()
        {
            string jsonFilePath = "data/commands.json";

            try
            {
                string json = File.ReadAllText(jsonFilePath);
                commandTypes = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(json);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"File {jsonFilePath} not found.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading commands from {jsonFilePath}: {ex.Message}");
            }
        }

    }
}
