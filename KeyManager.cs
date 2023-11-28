using Messenger.Models;
using System.Numerics;
using System.Numerics.Extensions;

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

        // Generate a new key pair and write the public and private keys to files
        public async Task GenerateKey(int keysize)
        {
            var pSize = (int)((keysize / 2) + (0.2 * keysize));
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
        }
    }
}
