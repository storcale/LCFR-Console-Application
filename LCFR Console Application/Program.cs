using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace lcfrConsoleApp
{
    class Program
    {
        private const string ConfigFilePath = "data/config.json";
        private static Dictionary<string, Dictionary<string, string>> commandTypes;
        private static DiscordCommandHandler discordCommandHandler;
        private static InformationCommandHandler informationCommandHandler;
        private static RosterCommandHandler rosterCommandHandler;
        private static SecureString id;
        private static SecureString namedToken;
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetConsoleTitle(string lpConsoleTitle);

        static void Main(string[] args)
        {
            // Load commands from JSON file
            LoadCommandsFromJson();

            // Initialize command handlers
            discordCommandHandler = new DiscordCommandHandler();
            informationCommandHandler = new InformationCommandHandler();
            rosterCommandHandler = new RosterCommandHandler();

            // Set the console window title
            SetConsoleTitle("LCFR API Bash");

            // Display title in the console.
            Console.WriteLine("Welcome to the LCFR API");
            Console.WriteLine("------------------------\n");
            Console.WriteLine("Enter 'information help' to see all commands ");

            // Check if it's the first time opening the application or if credentials are not loaded
            if (!LoadCredentials())
            {
                // Prompt user for credentials
                id = ReadSecureString("Please enter your ID:");
                namedToken = ReadSecureString("Please enter your named token:");

                // Encrypt and save credentials
                SaveCredentials();
            }

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine(" ");
                Console.WriteLine("Enter your command: ");
                Console.ResetColor();

                string userInput = Console.ReadLine();

                // Split the input into command and parameters
                string[] inputParts = userInput.Split(' ');
                string commandType = inputParts[0].ToLower();

                // Check if the command type exists in the loaded commands
                if (commandTypes.ContainsKey(commandType))
                {
                    // Switch on the command type
                    switch (commandType)
                    {
                        case "information":
                            informationCommandHandler.HandleCommand(inputParts, commandTypes).Wait();
                            break;
                        case "discord":
                            discordCommandHandler.HandleCommand(inputParts).Wait();
                            break;
                        case "roster":
                            rosterCommandHandler.HandleCommand(inputParts).Wait();
                            break;
                        default:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Invalid command type: {commandType}");
                            Console.ResetColor();
                            break;
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Invalid command type: {commandType}");
                    Console.ResetColor();
                }
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

        static bool LoadCredentials()
        {
            if (File.Exists(ConfigFilePath))
            {
                try
                {
                    string encryptedJson = File.ReadAllText(ConfigFilePath);
                    string decryptedJson = DecryptStringFromBytes_Aes(Convert.FromBase64String(encryptedJson));

                    var credentials = JsonConvert.DeserializeObject<Credentials>(decryptedJson);

                    if (credentials != null)
                    {
                        id = ConvertToSecureString(credentials.Id);
                        namedToken = ConvertToSecureString(credentials.NamedToken);
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading credentials: {ex.Message}");
                }
            }

            return false;
        }

        static void SaveCredentials()
        {
            try
            {
                var credentials = new Credentials
                {
                    Id = ConvertToUnsecureString(id),
                    NamedToken = ConvertToUnsecureString(namedToken)
                };

                string serialized = JsonConvert.SerializeObject(credentials);
                string encrypted = Convert.ToBase64String(EncryptStringToBytes_Aes(serialized));

                File.WriteAllText(ConfigFilePath, encrypted);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving credentials: {ex.Message}");
            }
        }

        static SecureString ReadSecureString(string prompt)
        {
            SecureString secureString = new SecureString();
            Console.WriteLine(prompt);

            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey(true);

                // Ignore any key that is not a backspace
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    secureString.AppendChar(key.KeyChar);
                    Console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace && secureString.Length > 0)
                {
                    secureString.RemoveAt(secureString.Length - 1);
                    Console.Write("\b \b");
                }
            }
            while (key.Key != ConsoleKey.Enter);

            Console.WriteLine();

            // Make the secure string read-only
            secureString.MakeReadOnly();

            return secureString;
        }

        static SecureString ConvertToSecureString(string unsecureString)
        {
            SecureString secureString = new SecureString();
            foreach (char c in unsecureString)
            {
                secureString.AppendChar(c);
            }
            secureString.MakeReadOnly();
            return secureString;
        }

        static string ConvertToUnsecureString(SecureString secureString)
        {
            IntPtr ptr = Marshal.SecureStringToBSTR(secureString);
            try
            {
                return Marshal.PtrToStringBSTR(ptr);
            }
            finally
            {
                Marshal.ZeroFreeBSTR(ptr);
            }
        }


        private static byte[] EncryptStringToBytes_Aes(string plainText)
        {
            // AesCryptoServiceProvider is a managed wrapper around the Windows CryptoAPI
            // Create AesCryptoServiceProvider instance
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.KeySize = 256;
                aesAlg.BlockSize = 128;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                // Generate a key and IV for AES
                aesAlg.GenerateKey();
                aesAlg.GenerateIV();

                // Create an encryptor to perform the stream transform
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream
                            swEncrypt.Write(plainText);
                        }
                        return msEncrypt.ToArray();
                    }
                }
            }
        }

        private static string DecryptStringFromBytes_Aes(byte[] cipherText)
        {
            // AesCryptoServiceProvider is a managed wrapper around the Windows CryptoAPI
            // Create AesCryptoServiceProvider instance
            using (AesCryptoServiceProvider aesAlg = new AesCryptoServiceProvider())
            {
                aesAlg.KeySize = 256;
                aesAlg.BlockSize = 128;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                // Generate a key and IV for AES
                aesAlg.GenerateKey();
                aesAlg.GenerateIV();

                // Create a decryptor to perform the stream transform
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream and place them in a string
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }

        // Classes for JSON serialization
        class Credentials
        {
            public string Id { get; set; }
            public string NamedToken { get; set; }
        }
    }
}
