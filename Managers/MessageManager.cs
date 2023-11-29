using Messenger.Models;
using System.Numerics;
using System.Text;
using System.Text.Json;

namespace Messenger.Managers
{
    public class MessageManager
    {
        const string BASE_URL = "http://kayrun.cs.rit.edu:5000/Message/";
        private HttpHelper _httpHelper;

        public MessageManager(HttpHelper httpHelper)
        {
            _httpHelper = httpHelper;
        }

        public async Task GetMessage(string email)
        {
            // Validate that you have a private key for the email being requested, if not, abort.
            var privateKeyStr = File.Exists("private.key") ? File.ReadAllText("private.key") : null;
            if (string.IsNullOrEmpty(privateKeyStr))
            {
                Console.WriteLine($"Key does not exist for {email} null or empty");
                return;
            }
            var privateKey = JsonSerializer.Deserialize<PrivateKey>(privateKeyStr, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (!privateKey.Email.Contains(email))
            {
                Console.WriteLine($"Key does not exist for {email}");
                return;
            }

            var key = Key.FromString(privateKey.Key);

            var response = await _httpHelper.Get($"{BASE_URL}{email}");

            if (string.IsNullOrEmpty(response))
            {
                Console.WriteLine($"No messages for {email}");
                return;
            }

            var message = JsonSerializer.Deserialize<MessageResponse>(response, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var bytes = Convert.FromBase64String(message.Content);
            var cipherInt = new BigInteger(bytes);
            var plainInt = BigInteger.ModPow(cipherInt, key.Exponent, key.Modulus);
            var plainBytes = plainInt.ToByteArray();
            var plainText = Encoding.UTF8.GetString(plainBytes);

            Console.WriteLine(plainText);
        }

        public async Task SendMessage(string email, string message)
        {
            // Ensure you have the public key for the user you are sending a message to, if not, abort
            var publicKeyStr = File.Exists($"{email}.key") ? File.ReadAllText($"{email}.key") : null;
            if (string.IsNullOrEmpty(publicKeyStr))
            {
                Console.WriteLine($"Key does not exist for {email}");
                return;
            }
            var publicKey = JsonSerializer.Deserialize<PublicKey>(publicKeyStr, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (publicKey.Email != email)
            {
                Console.WriteLine($"Key does not exist for {email}");
                return;
            }

            var key = Key.FromString(publicKey.Key);
            var bytes = Encoding.UTF8.GetBytes(message);
            var plainInt = new BigInteger(bytes);
            var cipherInt = BigInteger.ModPow(plainInt, key.Exponent, key.Modulus);
            var cipherBytes = cipherInt.ToByteArray();
            var cipherText = Convert.ToBase64String(cipherBytes);

            var messageObj = new Message
            {
                Email = email,
                Content = cipherText,
            };

            await _httpHelper.Put($"{BASE_URL}{email}", messageObj);

            Console.WriteLine($"Message written");
        }
    }
}
