using System.Numerics;

namespace Messenger.Models
{
    public class Key
    {
        public BigInteger Modulus { get; set; }
        public BigInteger Exponent { get; set; }

        public override string ToString()
        {
            var E = Exponent.ToByteArray();
            var e = BitConverter.GetBytes(E.Length);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(e);
            var N = Modulus.ToByteArray();
            var n = BitConverter.GetBytes(N.Length);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(n);
            
            var keyArray = e.Concat(E).Concat(n).Concat(N).ToArray();
            return Convert.ToBase64String(keyArray);
        }

        // TODO: Ensure these both work with eachother
        public static Key FromString(string keyString)
        {
            var keyArray = Convert.FromBase64String(keyString);
            
            var e = new byte[4];
            Array.Copy(keyArray, 0, e, 0, 4);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(e);

            var eLen = BitConverter.ToInt32(e);
            var E = new byte[eLen];
            Array.Copy(keyArray, 4, E, 0, eLen);
           
            var n = new byte[4];
            Array.Copy(keyArray, 4 + eLen, n, 0, 4);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(n);

            var nLen = BitConverter.ToInt32(n);
            var N = new byte[nLen];
            Array.Copy(keyArray, 8 + eLen, N, 0, nLen);

            return new Key
            {
                Exponent = new BigInteger(E),
                Modulus = new BigInteger(N)
            };
        }
    }
}
