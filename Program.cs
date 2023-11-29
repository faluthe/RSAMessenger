/*
 * Secure RSA Messenger
 * 
 * Patrick LeBlanc, RIT CSCI-251 Fall 2023
*/
using Messenger.Managers;

namespace Messenger
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length < 2 || args.Length > 3)
            {
                PrintHelpMessage();
                return;
            }

            try
            {
                string command = args[0];
                HttpHelper httpHelper = new HttpHelper();
                KeyManager keyManager = new KeyManager(httpHelper);
                MessageManager messageManager = new MessageManager(httpHelper);

                switch (command)
                {
                    case "getKey":
                        await keyManager.GetKey(args[1]);
                        break;
                    case "keyGen":
                        int keysize = int.Parse(args[1]);
                        keyManager.GenerateKeys(keysize);
                        break;
                    case "sendKey":
                        await keyManager.SendKey(args[1]);
                        break;
                    case "getMsg":
                        await messageManager.GetMessage(args[1]);
                        break;
                    case "sendMsg":
                        if (args.Length == 3)
                        {
                            await messageManager.SendMessage(args[1], args[2]);
                            break;
                        }
                        PrintHelpMessage();
                        break;
                    default:
                        PrintHelpMessage();
                        break;
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
        }

        static void PrintHelpMessage()
        {
            Console.WriteLine("Useage: dotnet run <option> <other arguments>");
            Console.WriteLine("Options:");
            Console.WriteLine("  getKey <email> - Gets the public key for the given email");
            Console.WriteLine("  keyGen <keysize> - Generates a new key pair with the given keysize");
            Console.WriteLine("  sendKey <email> - Sends your public key to the given email");
            Console.WriteLine("  getMsg <email> - Gets the message for the given email");
            Console.WriteLine("  sendMsg <email> <message> - Sends the given message to the given email");
        }
    }
}