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

        static void Main(string[] args)
        {
            // Load commands from JSON file
            LoadCommandsFromJson();

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
                string command = inputParts[0].ToLower();

                // Check if the command type exists in the loaded commands
                if (commandTypes.ContainsKey(command))
                {
                    // Switch on the command type
                    switch (command)
                    {
                        
                        case "information":
                            HandleInformationCommands(inputParts);
                            break;
                        case "discord":
                            HandleDiscordCommands(inputParts);
                            break;
                        case "roster":
                            HandleRosterCommands(inputParts);
                            break;
                        default:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Invalid command type: {command}");
                            Console.ResetColor();
                            break;
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Invalid command type: {command}");
                    Console.ResetColor();
                }
            }
        }

        static void HandleInformationCommands(string[] inputParts)
        {
            string subCommand = inputParts.Length > 1 ? inputParts[1].ToLower() : "";
            switch (subCommand)
            {
                case "help":
                    Console.WriteLine("Help for information commands:");
                    // Display help information for information commands
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

        static void HandleRosterCommands(string[] inputParts)
        {

        }
        static void HandleDiscordCommands(string[] inputParts)
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

        static void shiftAdd(string host, string username)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n  ShiftAdd: {host} {username}");
            Console.ResetColor();
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
