using System.Security.Cryptography;
using System.Text;

namespace WebApplication1.Helpers
{
    public static class PasswordHelper
    {
        // Create hash and salt for the password
        public static void CreatePasswordHashAndSalt(string password, out string passwordHash, out string passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = Convert.ToBase64String(hmac.Key);  // Store the salt
                passwordHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));  // Store the hash
            }
        }

        // Verify the password hash against stored hash and salt
        public static bool VerifyPasswordHash(string password, string storedHash, string storedSalt)
        {
            using (var hmac = new HMACSHA512(Convert.FromBase64String(storedSalt)))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(computedHash) == storedHash;
            }
        }
    }
}
