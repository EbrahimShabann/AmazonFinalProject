namespace Final_project.Services.EmailService
{
    public interface IEmailService
    {
        public Task SendEmailAsync(string toEmail, string subject, string body);
        public Task SendVerificationEmailAsync(string email, string verificationLink);
        Task SendPasswordResetEmailAsync(string email, string resetLink, string userName);
    }
}
