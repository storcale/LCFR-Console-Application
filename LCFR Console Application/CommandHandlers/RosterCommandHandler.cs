using System;
using System.Threading.Tasks;

namespace lcfrConsoleApp
{
    public class RosterCommandHandler
    {
        public async Task HandleCommand(string[] inputParts)
        {
            RequestManager requestManager = new RequestManager();
            await requestManager.LoadAPIs(); // Await the LoadAPIs method before making request

            string subCommand = inputParts.Length > 1 ? inputParts[1].ToLower() : "";
            switch (subCommand)
            {
                case "getversion":
                    var response = await requestManager.MakeRequest("rosterAPI", "getversion", null);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("RosterAPI version: " + response);
                    Console.ResetColor();
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
