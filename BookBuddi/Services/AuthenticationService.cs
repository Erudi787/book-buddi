using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace BookBuddi.Services
{
    public class AuthenticationService
    {
        /// <summary>
        /// Hashes a password using PBKDF2 with a random salt
        /// </summary>
        public string HashPassword(string password)
        {
            // Generate a random salt
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Hash the password with PBKDF2
            byte[] hash = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8);

            // Combine salt and hash
            byte[] hashBytes = new byte[salt.Length + hash.Length];
            Array.Copy(salt, 0, hashBytes, 0, salt.Length);
            Array.Copy(hash, 0, hashBytes, salt.Length, hash.Length);

            // Convert to base64
            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// Verifies a password against a stored hash
        /// </summary>
        public bool VerifyPassword(string password, string storedHash)
        {
            // Convert stored hash from base64
            byte[] hashBytes = Convert.FromBase64String(storedHash);

            // Extract salt (first 16 bytes)
            byte[] salt = new byte[128 / 8];
            Array.Copy(hashBytes, 0, salt, 0, salt.Length);

            // Hash the provided password with the extracted salt
            byte[] hash = KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8);

            // Compare the computed hash with the stored hash
            for (int i = 0; i < hash.Length; i++)
            {
                if (hashBytes[i + salt.Length] != hash[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Validates password strength
        /// </summary>
        public (bool isValid, string errorMessage) ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return (false, "Password is required.");
            }

            if (password.Length < 8)
            {
                return (false, "Password must be at least 8 characters long.");
            }

            if (!password.Any(char.IsUpper))
            {
                return (false, "Password must contain at least one uppercase letter.");
            }

            if (!password.Any(char.IsLower))
            {
                return (false, "Password must contain at least one lowercase letter.");
            }

            if (!password.Any(char.IsDigit))
            {
                return (false, "Password must contain at least one digit.");
            }

            if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                return (false, "Password must contain at least one special character.");
            }

            return (true, string.Empty);
        }
    }
}
