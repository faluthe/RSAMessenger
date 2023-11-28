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

                switch (command)
                {
                    // Writes the public key for the given email to a file
                    case "getKey":
                        string email = args[1];
                        await keyManager.GetKey(email);
                        break;
                    case "keyGen":
                        int keysize = int.Parse(args[1]);
                        await keyManager.GenerateKey(keysize);
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
            Console.WriteLine("placeholder");
        }
    }
}