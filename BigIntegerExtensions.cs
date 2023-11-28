using System.Security.Cryptography;

namespace System.Numerics.Extensions
{
    public static class BigIntegerExtensions
    {
        public static bool IsProbablyPrime(this BigInteger n, int k = 10)
        {
            if (n <= 3 || n % 2 == 0)
                return n == 2 || n == 3;

            // Write n as 2^r*d + 1 with d odd (by factoring out powers of 2 from n - 1)
            BigInteger d = n - 1;
            int r = 0;
            while ((d % 2) == 0)
            {
                r++;
                d /= 2;
            }

            for (int i = 0; i < k; i++)
            {
                BigInteger a = GenerateRandomBigInteger(2, n - 2);
                BigInteger x = BigInteger.ModPow(a, d, n);

                if (x == 1 || x == n - 1)
                    continue;

                int j;
                for (j = 0; j < r - 1; j++)
                {
                    x = BigInteger.ModPow(x, 2, n);
                    if (x == n - 1)
                        break;
                }

                // Only return false if we didn't break out of the loop
                if (j == r - 1)
                    return false;
            }

            return true; // probably prime
        }

        public static BigInteger GenerateRandomBigInteger(BigInteger min, BigInteger max)
        {
            byte[] byteArray = max.ToByteArray();
            RandomNumberGenerator.Fill(byteArray);

            BigInteger result = new BigInteger(byteArray);
            if (result < 0)
                result = -result;
            return min + result % (max - min + 1);
        }

        public static BigInteger GenerateRandomPrimeBigInteger(int bits)
        {
            var lockObj = new object();
            BigInteger? found = null;

            Parallel.For(0, int.MaxValue, (i, state) =>
            {
                var bytes = RandomNumberGenerator.GetBytes(bits / 8);
                // Ensure non-negative
                bytes[bytes.Length - 1] = 0;
                var bi = new BigInteger(bytes);

                if (bi.IsProbablyPrime())
                {
                    lock (lockObj)
                    {
                        found = bi;
                    }
                }
            });

            return found.Value;
        }

        public static BigInteger ModInverse(this BigInteger a, BigInteger n)
        {
            BigInteger i = n, v = 0, d = 1;
            while (a > 0)
            {
                BigInteger t = i / a, x = a;
                a = i % x;
                i = x;
                x = d;
                d = v - t * x;
                v = x;
            }
            v %= n;
            if (v < 0)
                v = (v + n) % n;
            return v;
        }
    }
}
