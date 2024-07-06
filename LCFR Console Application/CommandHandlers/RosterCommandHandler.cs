using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace lcfrConsoleApp
{
    public class RosterCommandHandler
    {
        private RequestManager requestManager;

        public RosterCommandHandler()
        {
            requestManager = new RequestManager();
        }

        public async Task HandleCommand(string[] inputParts)
        {
            await requestManager.LoadAPIs(); // Ensure APIs are loaded before making requests

            string subCommand = inputParts.Length > 1 ? inputParts[1].ToLower() : "";
            Dictionary<string, string> parameters = requestManager.ConstructParameters(inputParts, 2);

            switch (subCommand)
            {
                case "getversion":
                    Console.WriteLine("Sending request...");
                    await ExecuteRequest("roster", "getVersion", null); // Adjust action name
                    break;

                case "adduser":
                    Console.WriteLine("Sending request...");
                    await ExecuteRequest("roster", "addUser", parameters); // Adjust action name
                    break;

                case "searchuser":
                    Console.WriteLine("Sending request...");
                    await ExecuteRequest("roster", "searchUser", parameters); // Adjust action name
                    break;

                case "removeuser":
                    Console.WriteLine("Sending request...");
                    await ExecuteRequest("roster", "removeUser", parameters); // Adjust action name
                    break;

                case "edituser":
                    Console.WriteLine("Sending request...");
                    await ExecuteRequest("roster", "editUser", parameters); // Adjust action name
                    break;

                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Invalid command under 'roster' type: {subCommand}");
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
