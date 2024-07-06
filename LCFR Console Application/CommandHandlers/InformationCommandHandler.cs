using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace lcfrConsoleApp
{
    public class InformationCommandHandler
    {
        private RequestManager requestManager;

        public InformationCommandHandler()
        {
            requestManager = new RequestManager();
        }

        public async Task HandleCommand(string[] inputParts, Dictionary<string, Dictionary<string, string>> commandTypes)
        {
            string command = inputParts.Length > 1 ? inputParts[1].ToLower() : "";
            Dictionary<string, string> parameters = requestManager.ConstructParameters(inputParts, 2);

            // Handle each information command
            switch (command)
            {
                case "version":
                    await ExecuteRequest("information", "getVersion", parameters); // Adjust action name
                    break;
                case "help":
                    DisplayHelp(commandTypes);
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Invalid command under 'information' type: {command}");
                    Console.ResetColor();
                    break;
            }
        }


        private async Task ExecuteRequest(string type, string action, Dictionary<string, string> parameters)
        {
            string response = await requestManager.MakeRequest(type, action, parameters);
            if (response != null)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(response);
                Console.ResetColor();
            }
        }

        private void DisplayHelp(Dictionary<string, Dictionary<string, string>> commandTypes)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Available commands:");
            foreach (var commandCategory in commandTypes)
            {
                Console.WriteLine($"\n{commandCategory.Key}:");
                foreach (var command in commandCategory.Value)
                {
                    Console.WriteLine($"  {command.Key} - {command.Value}");
                }
            }
            Console.ResetColor();
        }
    }
}
