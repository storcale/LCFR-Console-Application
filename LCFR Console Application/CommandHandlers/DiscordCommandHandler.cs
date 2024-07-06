using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace lcfrConsoleApp
{
    internal class DiscordCommandHandler
    {
        private RequestManager requestManager;

        public DiscordCommandHandler()
        {
            requestManager = new RequestManager();
        }

        public async Task HandleCommand(string[] inputParts)
        {
            string command = inputParts[1].ToLower();
            Dictionary<string, string> parameters = requestManager.ConstructParameters(inputParts, 2);

            // Handle each discord command
            switch (command)
            {
                case "shiftadd":
                    await ExecuteRequest("discord", "shiftAdd", parameters);
                    break;
                case "report":
                    await ExecuteRequest("discord", "report", parameters);
                    break;
                case "notice":
                    await ExecuteRequest("discord", "notice", parameters);
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Invalid discord command: {command}");
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
    }
}
