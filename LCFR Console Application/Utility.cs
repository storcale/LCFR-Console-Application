using Newtonsoft.Json;
using System;
using System.Data;

namespace lcfrConsoleApp
{
    public class Utility
    {
        // Load variables
        private static Dictionary<string, Dictionary<string, string>> commandTypes;
        private static Dictionary<string, Dictionary<string, string>> information;
        


        public void loadCommands()
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
        public void LoadInfo()
        {
            string jsonFilePath = "data/appInfo.json";

            try
            {
                string json = File.ReadAllText(jsonFilePath);
                information = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(json);
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