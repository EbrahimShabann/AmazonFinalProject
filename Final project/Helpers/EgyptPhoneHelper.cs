namespace Final_project.Helpers
{
    public static class EgyptPhoneHelper
    {
        public static readonly string[] EgyptianOperators = {
            "010", "011", "012", "015", // Vodafone
            "010", "014", "016", "017", "019", // Orange
            "010", "018", // Etisalat
            "010", "011", "012", "015", // WE (Telecom Egypt)
        };

        public static bool IsValidEgyptianMobile(string phoneNumber)
        {
            var formatted = FormatToEgyptianStandard(phoneNumber);
            if (string.IsNullOrEmpty(formatted))
                return false;

            // Check if it's a valid Egyptian mobile number (starts with 01X)
            return formatted.Length == 14 &&
                   formatted.StartsWith("+201") &&
                   IsValidOperatorCode(formatted.Substring(4, 3));
        }

        public static string FormatToEgyptianStandard(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                return null;

            // Remove all non-digit characters except +
            var cleaned = System.Text.RegularExpressions.Regex.Replace(phoneNumber, @"[^\d+]", "");

            // Handle various input formats
            if (cleaned.StartsWith("+20"))
            {
                if (cleaned.Length == 14 && cleaned.Substring(3, 2) == "01")
                    return cleaned; // Already correct format
            }
            else if (cleaned.StartsWith("20"))
            {
                if (cleaned.Length == 13 && cleaned.Substring(2, 2) == "01")
                    return "+" + cleaned;
            }
            else if (cleaned.StartsWith("01"))
            {
                if (cleaned.Length == 11)
                    return "+20" + cleaned;
            }
            else if (cleaned.StartsWith("1") && cleaned.Length == 10)
            {
                return "+20" + cleaned;
            }

            return null; // Invalid format
        }

        private static bool IsValidOperatorCode(string operatorCode)
        {
            var validPrefixes = new[] { "010", "011", "012", "014", "015", "016", "017", "018", "019" };
            return validPrefixes.Contains(operatorCode);
        }

        public static string GetOperatorName(string phoneNumber)
        {
            var formatted = FormatToEgyptianStandard(phoneNumber);
            if (string.IsNullOrEmpty(formatted) || formatted.Length < 7)
                return "Unknown";

            var prefix = formatted.Substring(4, 3);
            return prefix switch
            {
                "010" => "Vodafone Egypt", // Can be multiple operators
                "011" => "Etisalat Egypt",
                "012" => "Orange Egypt",
                "014" => "WE (Telecom Egypt)",
                "015" => "WE (Telecom Egypt)",
                "016" => "WE (Telecom Egypt)",
                "017" => "WE (Telecom Egypt)",
                "018" => "Etisalat Egypt",
                "019" => "WE (Telecom Egypt)",
                _ => "Unknown Operator"
            };
        }
    }
}
