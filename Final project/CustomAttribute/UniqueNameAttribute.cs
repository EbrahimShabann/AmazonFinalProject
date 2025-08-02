using System.ComponentModel.DataAnnotations;
using Final_project.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Final_project.CustomAttribute
{
    public class UniqueNameAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var db = validationContext.GetService<AmazonDBContext>();

            if (db == null)
            {
                throw new Exception("Database in CustomAttribute is not available in ValidationContext.");
            }

            var name = value?.ToString();

            if (string.IsNullOrWhiteSpace(name))
            {
                return new ValidationResult("Name cannot be empty.");
            }

            var existingName = db.Users.FirstOrDefault(e => e.UserName == name);

            if (existingName == null)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("This username is already taken.");
        }
    
    }
}
