using EP.Application.Common.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EP.Infrastructure.Services
{
    public class HashService : IHashService
    {
        private static readonly int SaltSize = 128 / 8;
        private static readonly int KeySize = 256 / 8;
        private static readonly int Iterations = 100_000;
        private const char Delimiter = ';';

        /// <summary>
        /// Hashes a password using PBKDF2 with a random salt.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public string Hash(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, KeySize);
            return string.Join(Delimiter, Convert.ToBase64String(salt), Convert.ToBase64String(hash));
        }

        /// <summary>
        /// Verifies a password against a stored hash.
        /// </summary>
        public bool Verify(string passwordHash, string passwordInput)
        {
            if (string.IsNullOrWhiteSpace(passwordHash) || string.IsNullOrWhiteSpace(passwordInput))
                return false;

            string[] elements = passwordHash.Split(Delimiter);
            if (elements.Length != 2)
                return false;

            byte[] salt, hash;
            try
            {
                salt = Convert.FromBase64String(elements[0]);
                hash = Convert.FromBase64String(elements[1]);
            }
            catch
            {
                return false;
            }

            byte[] hashInput = Rfc2898DeriveBytes.Pbkdf2(passwordInput, salt, Iterations, HashAlgorithmName.SHA256, KeySize);
            return CryptographicOperations.FixedTimeEquals(hash, hashInput);
        }
    }
}
