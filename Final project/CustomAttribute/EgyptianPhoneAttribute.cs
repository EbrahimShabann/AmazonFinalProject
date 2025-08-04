using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Final_project.CustomAttribute
{
    public class EgyptianPhoneAttribute : ValidationAttribute
    {
        private readonly string _pattern = @"^01[0125]\d{8}$";

        public EgyptianPhoneAttribute()
        {
            ErrorMessage = "Phone number must start with 010, 011, 012, or 015 and be exactly 11 digits long.";
        }

        public override bool IsValid(object value)
        {
            if (value == null || string.IsNullOrEmpty(value.ToString()))
            {
                return true; // Let [Required] handle null/empty validation if needed
            }

            string phoneNumber = value.ToString();
            return Regex.IsMatch(phoneNumber, _pattern);
        }
    }
}