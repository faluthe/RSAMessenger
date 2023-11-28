using Messenger.Models;
using System.Numerics.Extensions;
using System.Text.Json;

namespace Messenger
{
    public class KeyManager
    {
        const string BASE_URL = "http://kayrun.cs.rit.edu:5000/Key/";
        private HttpHelper _httpHelper;

        public KeyManager(HttpHelper httpHelper)
        {
            _httpHelper = httpHelper;
        }

        // Write the public key for the given email to a file
        public async Task GetKey(string email)
        {
            var response = await _httpHelper.Get($"{BASE_URL}{email}");

            if (string.IsNullOrEmpty(response))
            {
                Console.WriteLine($"Key does not exist for {email}");
                return;
            }

            File.WriteAllText($"{email}.key", response);
        }

        // Send the public key saved on disk to the server
        public async Task SendKey(string email)
        {
            string? publicKeyStr = File.Exists("public.key") ? File.ReadAllText("public.key") : null;
            if (string.IsNullOrEmpty(publicKeyStr))
            {
                Console.WriteLine($"Key does not exist for {email}");
                return;
            }

            var publicKey = JsonSerializer.Deserialize<Key>(publicKeyStr);
            await _httpHelper.Post($"{BASE_URL}{email}", publicKey);

            Console.WriteLine("Key saved");
        }

        // Generate a new key pair and write the public and private keys to files
        public void GenerateKeys(int keysize)
        {
            var pSize = (int)((keysize / 2) * 1.2);
            var p = BigIntegerExtensions.GenerateRandomPrimeBigInteger(pSize);
            var q = BigIntegerExtensions.GenerateRandomPrimeBigInteger(keysize - pSize);

            var N = p * q;
            var r = (p - 1) * (q - 1);
            var E = 65537;
            var D = BigIntegerExtensions.ModInverse(E, r);

            var publicKey = new Key
            {
                modulus = N,
                exponent = E
            };

            var privateKey = new Key
            {
                modulus = N,
                exponent = D
            };

            var serializedPublicKey = JsonSerializer.Serialize(new PublicKey { Email = "", Key = publicKey.ToString() });
            var serializedPrivateKey = JsonSerializer.Serialize(new PrivateKey { Email = new List<string>(), Key = privateKey.ToString() });

            // Write these in the proper format
            File.WriteAllText("public.key", serializedPublicKey);
            File.WriteAllText("private.key", serializedPrivateKey);
        }
    }
}
