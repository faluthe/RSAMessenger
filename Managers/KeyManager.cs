using Messenger.Models;
using System.Numerics.Extensions;
using System.Text.Json;

namespace Messenger.Managers
{
    public class KeyManager
    {
        const string BASE_URL = "http://kayrun.cs.rit.edu:5000/Key/";
        private HttpHelper _httpHelper;

        public KeyManager(HttpHelper httpHelper)
        {
            _httpHelper = httpHelper;
        }

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

        public async Task SendKey(string email)
        {
            string? publicKeyStr = File.Exists("public.key") ? File.ReadAllText("public.key") : null;
            if (string.IsNullOrEmpty(publicKeyStr))
            {
                Console.WriteLine($"Key does not exist for {email}");
                return;
            }
            string? privateKeyStr = File.Exists("private.key") ? File.ReadAllText("private.key") : null;
            if (string.IsNullOrEmpty(privateKeyStr))
            {
                Console.WriteLine($"Key does not exist for {email}");
                return;
            }

            var publicKey = JsonSerializer.Deserialize<PublicKey>(publicKeyStr, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var privateKey = JsonSerializer.Deserialize<PrivateKey>(privateKeyStr, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            publicKey.Email = email;
            privateKey.Email.Add(email);

            await _httpHelper.Put($"{BASE_URL}{email}", publicKey);
            privateKeyStr = JsonSerializer.Serialize(privateKey);
            File.WriteAllText("private.key", privateKeyStr);

            Console.WriteLine("Key saved");
        }

        public void GenerateKeys(int keysize)
        {
            var pSize = (int)(keysize / 2 * 1.2);
            var p = BigIntegerExtensions.GenerateRandomPrimeBigInteger(pSize);
            var q = BigIntegerExtensions.GenerateRandomPrimeBigInteger(keysize - pSize);

            var N = p * q;
            var r = (p - 1) * (q - 1);
            var E = 65537;
            var D = BigIntegerExtensions.ModInverse(E, r);

            var publicKey = new Key
            {
                Modulus = N,
                Exponent = E
            };

            var privateKey = new Key
            {
                Modulus = N,
                Exponent = D
            };

            var serializedPublicKey = JsonSerializer.Serialize(new PublicKey { Email = "", Key = publicKey.ToString() });
            var serializedPrivateKey = JsonSerializer.Serialize(new PrivateKey { Email = new List<string>(), Key = privateKey.ToString() });

            File.WriteAllText("public.key", serializedPublicKey);
            File.WriteAllText("private.key", serializedPrivateKey);
        }
    }
}
