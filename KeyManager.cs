namespace Messenger
{
    public class KeyManager
    {
        const string BASE_URL = "http://kayrun.cs.rit.edu:5000/Key/";
        private HttpHelper _httpHelper;

        public KeyManager()
        {
            _httpHelper = new HttpHelper();
        }
        
        // Write the public key for the given email to a file
        public async Task GetKey(string email)
        {
            var response = await _httpHelper.Get($"{BASE_URL}{email}");

            if (response == null)
            {
                Console.WriteLine($"Key does not exist for {email}");
                return;
            }

            File.WriteAllText($"{email}.key", response);
        }
    }
}
