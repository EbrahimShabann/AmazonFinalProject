using Final_project.Models;
using System.ComponentModel.DataAnnotations;

namespace Final_project.CustomAttribute
{
    public class UniqueEmailAttribute:ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var db = validationContext.GetService<AmazonDBContext>();

            if (db == null)
            {
                throw new Exception("Database in CustomAttribute is not available in ValidationContext.");
            }

            var email = value?.ToString();

            if (string.IsNullOrWhiteSpace(email))
            {
                return new ValidationResult("Name cannot be empty.");
            }

            var existingName = db.Users.FirstOrDefault(e => e.Email == email);

            if (existingName == null)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("This email account is already signed in.");
        }

    }
}
