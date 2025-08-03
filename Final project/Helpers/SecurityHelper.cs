using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text;

namespace Final_project.Helpers
{
    public static class SecurityHelper
    {
        public static string GenerateSecureCode(int length = 6)
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var bytes = new byte[4];
                rng.GetBytes(bytes);
                var random = new Random(BitConverter.ToInt32(bytes, 0));

                var code = "";
                for (int i = 0; i < length; i++)
                {
                    code += random.Next(0, 10).ToString();
                }
                return code;
            }
        }

        public static string HashDeviceFingerprint(string fingerprint)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(fingerprint));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                return false;

            // Basic phone number validation
            var pattern = @"^\+?[1-9]\d{1,14}$";
            return Regex.IsMatch(phoneNumber, pattern);
        }

        public static string FormatPhoneNumber(string phoneNumber)
        {
            // Remove all non-digit characters except +
            var cleaned = Regex.Replace(phoneNumber, @"[^\d+]", "");

            // Ensure it starts with +
            if (!cleaned.StartsWith("+"))
            {
                cleaned = "+" + cleaned;
            }

            return cleaned;
        }
    }

}
