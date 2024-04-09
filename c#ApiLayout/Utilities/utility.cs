using System.Security.Cryptography;
using System.Text;

namespace c_ApiLayout.Utilities
{
    public static class RandomStringGenerator
    {
        public static string GenerateRandomString(int length)
        {
            const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var randomBytes = new byte[length];

            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomBytes);
            }

            var result = new StringBuilder(length);

            foreach (byte b in randomBytes)
            {
                result.Append(validChars[b % validChars.Length]);
            }

            return result.ToString();
        }
    }
}
