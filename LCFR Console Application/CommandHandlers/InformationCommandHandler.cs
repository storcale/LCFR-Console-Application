using Newtonsoft.Json;
using System;
using System.Data;

namespace lcfrConsoleApp
{
    public class InformationCommandHandler
    {
        private static Dictionary<string, Dictionary<string, string>> commandTypes;

        public void HandleCommand(string[] inputParts)
        {
            LoadCommandsFromJson();

            string subCommand = inputParts.Length > 1 ? inputParts[1].ToLower() : "";
            switch (subCommand)
            {
                case "help":
                    Console.WriteLine($"Help for {commandType} commands:");
                    foreach (var cmd in commands);
                    {
                        Console.WriteLine($"{cmd.Key}: {cmd.Value}");
                    };
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
