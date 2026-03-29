using simur_backend.Auth.Contract;
using System.Security.Cryptography;
using System.Text;

namespace simur_backend.Utilities
{
    public class AlgorithmSha256
    {
        public static string HashString(string password)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(password);
            byte[] hashedBytes = SHA256.HashData(inputBytes);
            var builder = new StringBuilder();
            foreach (byte b in hashedBytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }

        public static bool Compare(string password, string hashedPassword)
        {
            return HashString(password) == hashedPassword;
        }
    }
}
