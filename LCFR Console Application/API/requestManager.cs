using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace lcfrConsoleApp
{
    internal class RequestManager
    {
        private static Dictionary<string, Dictionary<string, RequestInfo>> APIs;
        private string userID;
        private string namedToken;
        private static readonly string encryptionKey = "kw4u+rQZPEtQzyKk3z5nEQL/cJ3GyK23ZjOTaHryOhc="; // Your base64 encoded encryption key
        private static readonly byte[] keyBytes = Convert.FromBase64String(encryptionKey);

        public RequestManager()
        {
            LoadCredentials();
        }

        private void LoadCredentials()
        {
            try
            {
                var config = LoadConfigFromFile();
                var encryptedID = Convert.FromBase64String(config["userID"]);
                var encryptedToken = Convert.FromBase64String(config["namedToken"]);

                var userID = DecryptStringFromBytes_Aes(encryptedID, keyBytes);
                var namedToken = DecryptStringFromBytes_Aes(encryptedToken, keyBytes);

                Console.WriteLine($"Loaded UserID: {userID}, NamedToken: {namedToken}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading credentials: {ex.Message}");
            }
        }

        private Dictionary<string, string> LoadConfigFromFile()
        {
            string configFilePath = "data/config.json";

            try
            {
                string json = File.ReadAllText(configFilePath);
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Config file '{configFilePath}' not found.");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading config file '{configFilePath}': {ex.Message}");
                throw;
            }
        }
        private static string DecryptStringFromBytes_Aes(byte[] cipherText, byte[] Key)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException(nameof(cipherText));
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException(nameof(Key));

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    // Get the IV from the encrypted stream
                    byte[] iv = new byte[aesAlg.IV.Length];
                    msDecrypt.Read(iv, 0, iv.Length);

                    aesAlg.IV = iv;

                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }

        public static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key)
        {
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException(nameof(plainText));
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException(nameof(Key));

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;

                aesAlg.GenerateIV();

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    msEncrypt.Write(aesAlg.IV, 0, aesAlg.IV.Length);

                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                    }

                    return msEncrypt.ToArray();
                }
            }
        }


        public async Task<string> MakeRequest(string type, string action, Dictionary<string, string> parameters)
        {
            await LoadAPIs();

            if (APIs.TryGetValue(type, out var requestActions))
            {
                if (requestActions.TryGetValue(action, out var requestInfo))
                {
                    string apiUrl = requestInfo.Url;

                    // Replace placeholders in the API URL with actual parameter values if parameters are provided
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            apiUrl = apiUrl.Replace($"{{{param.Key}}}", param.Value);
                        }
                    }

                    // Make HTTP request to the API
                    using (HttpClient client = new HttpClient())
                    {
                        HttpResponseMessage response;

                        // Handle POST requests with token
                        if (requestInfo.Method.ToUpper() == "POST")
                        {
                            var token = await GetToken(action, type, parameters.ContainsKey("dept") ? parameters["dept"] : "lcfr");
                            parameters["authorization"] = $"{userID}:{token}";

                            var content = new FormUrlEncodedContent(parameters);
                            response = await client.PostAsync(apiUrl, content);
                        }
                        else // Use GET method otherwise
                        {
                            response = await client.GetAsync(apiUrl);
                        }

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

        private async Task<string> GetToken(string actions, string actionType, string dept)
        {
            string expire = ((DateTimeOffset)DateTime.UtcNow.AddHours(1)).ToUnixTimeSeconds().ToString(); // Example: Token expires in 1 hour
            string url = $"https://script.google.com/macros/s/AKfycbwFgeZZXghgVcgYnkubt2UrpJaAiY8aZyHzWkeYWVWhOThrGdNK08HNp8Phookh53SS/exec?type=getToken&expire={expire}&actions={actions}&actionType={actionType}&authorization={userID}:{namedToken}&action=getToken&dept={dept}";

            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    throw new Exception($"Failed to get token. Status code: {response.StatusCode}");
                }
            }
        }

        public async Task LoadAPIs()
        {
            string jsonFilePath = "API/apis.json";

            try
            {
                string json = await File.ReadAllTextAsync(jsonFilePath);
                APIs = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, RequestInfo>>>(json);
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

        public Dictionary<string, string> ConstructParameters(string[] inputParts, int startIndex)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            int paramIndex = 1; // Starting index for generated parameter keys
            for (int i = startIndex; i < inputParts.Length; i++)
            {
                if (inputParts[i].Contains('='))
                {
                    // Split each input part into key and value using the '=' separator
                    string[] keyValue = inputParts[i].Split('=');
                    if (keyValue.Length == 2)
                    {
                        // Add key-value pair to the parameters dictionary
                        parameters[keyValue[0]] = keyValue[1];
                    }
                    else
                    {
                        // Handle invalid input format
                        Console.WriteLine($"Invalid parameter format: {inputParts[i]}");
                    }
                }
                else
                {
                    // Generate parameter key sequentially as 'param1', 'param2', ...
                    parameters["param" + paramIndex++] = inputParts[i];
                }
            }
            return parameters;
        }
    }

    
        

    internal class RequestInfo
    {
        public string Method { get; set; }
        public string Url { get; set; }
    }

}
