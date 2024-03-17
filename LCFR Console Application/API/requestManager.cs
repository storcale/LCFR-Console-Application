﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace lcfrConsoleApp
{
    internal class RequestManager
    {
        private static Dictionary<string, Dictionary<string, string>> APIs;

        public async Task<string> MakeRequest(string type, string action, Dictionary<string, string> parameters)
        {
            LoadAPIs();

            if (APIs.TryGetValue(type, out var apiEndpoints))
            {
                if (apiEndpoints.TryGetValue(action, out var apiUrl))
                {
                    // Replace placeholders in the API URL with actual parameter values
                    foreach (var param in parameters)
                    {
                        apiUrl = apiUrl.Replace($"{{{param.Key}}}", param.Value);
                    }

                    // Make HTTP request to the API
                    using (HttpClient client = new HttpClient())
                    {
                        HttpResponseMessage response = await client.GetAsync(apiUrl);
                        if (response.IsSuccessStatusCode)
                        {
                            return await response.Content.ReadAsStringAsync();
                        }
                        else
                        {
                            Console.WriteLine($"Failed to make HTTP request. Status code: {response.StatusCode}");
                            return null;
                        }
                    }
                }
                else
                {
                    Console.WriteLine($"Action '{action}' not found for type '{type}'.");
                    return null;
                }
            }
            else
            {
                Console.WriteLine($"API type '{type}' not found.");
                return null;
            }
        }


        public void LoadAPIs()
        {
            string jsonFilePath = "API/apis.json";

            try
            {
                string json = File.ReadAllText(jsonFilePath);
                APIs = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(json);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"File {jsonFilePath} not found.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading APIs from {jsonFilePath}: {ex.Message}");
            }
        
    }
}
}
