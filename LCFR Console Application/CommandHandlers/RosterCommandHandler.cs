using System;

namespace lcfrConsoleApp
{
    public class RosterCommandHandler
    {
        public void HandleCommand(string[] inputParts)
        {
            RequestManager requestManager = new RequestManager();
            requestManager.LoadAPIs(); // Load APIs before making request

            string subCommand = inputParts.Length > 1 ? inputParts[1].ToLower() : "";
            switch (subCommand)
            {
                case "getversion":
                    var response = requestManager.MakeRequest("roster", "getversion", null).Result; 
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
