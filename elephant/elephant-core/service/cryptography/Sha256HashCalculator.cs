using System;
using System.Security.Cryptography;
using System.Text;
using elephant.core.util;

namespace elephant.core.service.cryptography
{
    public class Sha256HashCalculator : IHashCalculator
    {
        private const string SHA256_NAME = "SHA-256";
        private const int HASH_SIZE = 256;

        public int HashSize { get => HASH_SIZE; }

        public string CalculateHash(string input)
        {
            byte[] digest = CalculateHashAsBytes(input);
            return digest.ToHexString();
        }

        public byte[] CalculateHashAsBytes(string input)
        {
            try
            {
                SHA256 md = SHA256.Create(SHA256_NAME);
                return md.ComputeHash(Encoding.UTF8.GetBytes(input));
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Exception occurred while calculating hash.", ex);
            }
        }

        public string CombineOrdered(string[] tup)
        {
            return CalculateHash(String.Concat(tup));
        }

        public string CombineUnordered(string[] tup)
        {
            var sum = CalculateHashAsBytes(tup[0]);
            for (var i = 1; i < tup.Length; i++)
            {
                ByteArrayUtils.AddHashes(sum, CalculateHashAsBytes(tup[i]));
            }
            return sum.ToHexString();
        }
    }
}
