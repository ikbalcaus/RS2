using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace eBooks.Services
{
    public class Helpers
    {
        public static bool IsEmailValid(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return emailRegex.IsMatch(email);
        }

        public static bool IsPasswordValid(string password)
        {
            if (string.IsNullOrWhiteSpace(password)) return false;
            var hasMinimum8Chars = password.Length >= 8;
            var hasUpperChar = Regex.IsMatch(password, "[A-Z]");
            var hasLowerChar = Regex.IsMatch(password, "[a-z]");
            var hasDigit = Regex.IsMatch(password, "[0-9]");
            return hasMinimum8Chars && hasUpperChar && hasLowerChar && hasDigit;
        }

        public static string GenerateSalt(int size = 32)
        {
            var saltBytes = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        public static string GenerateHash(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                var combined = password + salt;
                var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combined));
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}
