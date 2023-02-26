using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Tokket.Shared.Helpers
{
    public static partial class HashGenerator
    {
        /// <summary>
        ///     Size of salt
        /// </summary>
        private const int SaltSize = 18;
        private const string Letters = "abcdefghijklmnopqrstuvwxyz";
        /// <summary>
        ///     Generates Hash with Salt and Iterations
        /// </summary>
        /// <param name="randomText">Randomized Text</param>
        /// <param name="salt">Generated Salt</param>
        /// <param name="iterations">Number of Iterations</param>
        /// <returns>returns the generated hash (pbkdf2)</returns>
        private static byte[] GenerateHash(string randomText, byte[] salt, int iterations, int hashSize) => new Rfc2898DeriveBytes(randomText, salt, iterations).GetBytes(hashSize);
        private static string CreateSecureHash(string randomText, byte[] salt, int iterations, int hashSize)
        {
            // Create Hash
            var hash = GenerateHash(randomText, salt, iterations, hashSize);
            // Combine Salt and Hash
            var hashBytes = new byte[SaltSize + hashSize];
            Array.Copy(salt, 0, hashBytes, 0, SaltSize);
            Array.Copy(hash, 0, hashBytes, SaltSize, hashSize);
            // Convert to Base64
            var base64Hash = Convert.ToBase64String(hashBytes);
            //format hash with extra information
            return string.Format("{0}", base64Hash);
        }
        /// <summary>
        ///     Generate Random Hash with specific length and number of iterations
        /// </summary>
        /// <param name="iterations">The number of times to iterate</param>
        /// <param name="lenght">Length of the random text</param>
        /// <param name="hashSize">Size of the Hash</param>
        /// <returns></returns>
        public static string GenerateRandomHash(int iterations, int lenght, int hashSize)
        {
            string randomStr = "";
            for (int x = 0; x < lenght; x++)
            {
                string resultChar = "";
                for (int z = 0; z < iterations; z++)
                {
                    int oddeven = new Random().Next(0, 99);
                    resultChar = ((oddeven % 2) == 0 ? Letters[new Random().Next(0, Letters.Length - 1)].ToString() : (new Random().Next(0, 9) + ""));
                }
                randomStr += resultChar;
            }
            //Debug.WriteLine(randomStr);
            return CreateSecureHash(randomStr, GenerateSalt(), iterations, hashSize);
        }
        /// <summary>
        ///     Generate a Hash from a specified key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="iterations"></param>
        /// <param name="hashSize"></param>
        /// <returns></returns>
        public static string GenerateCustomHash(string key, int iterations, int hashSize) => CreateSecureHash(key, GenerateSalt(), iterations, hashSize);
        /// <summary>
        ///     Creates Salt for Random Text
        /// </summary>
        /// <returns></returns>
        private static byte[] GenerateSalt()
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[SaltSize]);
            return salt;
        }
        /// <summary>
        ///     Verify a key against a hash
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="hashedKey">Hashed Key</param>
        /// <returns>returns a validated key</returns>
        public static bool Verify(string key, string hashedKey, int iterations, int hashSize)
        {
            // Extract Iteration and Base64 String
            var base64Hash = hashedKey;
            // Get Hash Bytes
            var hashBytes = Convert.FromBase64String(base64Hash);
            // Get Salt from the Hash
            var salt = new byte[SaltSize];
            Array.Copy(hashBytes, 0, salt, 0, SaltSize);
            // Create Hash with Given Salt
            byte[] hash = GenerateHash(key, salt, iterations, hashSize);
            // Get Result
            for (var i = 0; i < hashSize; i++)
            {
                if (hashBytes[i + SaltSize] != hash[i])
                    return false;
            }
            return true;
        }
    }
}
