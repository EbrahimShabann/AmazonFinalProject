using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Final_project.Models;

namespace Final_project.Services.EmailService
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        // Add this test method to verify connection
        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                using var client = new SmtpClient();

                _logger.LogInformation($"Testing connection to: {_emailSettings.SmtpServer}:{_emailSettings.SmtpPort}");
                _logger.LogInformation($"Username: {_emailSettings.Username}");
                _logger.LogInformation($"Password length: {_emailSettings.Password?.Length ?? 0}");

                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
                _logger.LogInformation("Connected successfully");

                await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
                _logger.LogInformation("Authentication successful");

                await client.DisconnectAsync(true);
                _logger.LogInformation("Test completed successfully");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Connection test failed: {Message}", ex.Message);
                return false;
            }
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = body
                };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();

                // Enhanced logging
                _logger.LogInformation($"Connecting to SMTP server: {_emailSettings.SmtpServer}:{_emailSettings.SmtpPort}");
                _logger.LogInformation($"Using username: {_emailSettings.Username}");
                _logger.LogInformation($"Password starts with: {(_emailSettings.Password?.Length > 0 ? _emailSettings.Password.Substring(0, Math.Min(4, _emailSettings.Password.Length)) : "N/A")}...");

                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
                _logger.LogInformation("Connected to SMTP server successfully");

                await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
                _logger.LogInformation("Authentication successful");

                await client.SendAsync(message);
                _logger.LogInformation($"Email sent successfully to {toEmail}");

                await client.DisconnectAsync(true);
            }
            catch (MailKit.Security.AuthenticationException ex)
            {
                _logger.LogError(ex, "Authentication failed. Please check:");
                _logger.LogError("1. Two-factor authentication is enabled on your Google account");
                _logger.LogError("2. You're using an App Password (not your regular password)");
                _logger.LogError("3. The App Password is correct and hasn't expired");
                _logger.LogError("4. Less secure app access is not required for App Passwords");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {toEmail}: {ex.Message}");
                throw;
            }
        }

        public async Task SendVerificationEmailAsync(string email, string verificationLink)
        {
            var subject = "✅ Verify Your Email Address - Action Required";
            var body = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Email Verification</title>
    <style>
        /* Gmail CSS Reset */
        u + #body a,
        #MessageViewBody a,
        a[x-apple-data-detectors],
        .ii a[href] {{
            color: inherit !important;
            text-decoration: inherit !important;
            font-size: inherit !important;
            font-family: inherit !important;
            font-weight: inherit !important;
            line-height: inherit !important;
        }}
        
        * {{
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }}
        body {{
            font-family: 'Amazon Ember', 'Helvetica Neue', Roboto, Arial, sans-serif;
            line-height: 1.6;
            color: #0F1111;
            background-color: #F7F8F8;
            -webkit-font-smoothing: antialiased;
            -moz-osx-font-smoothing: grayscale;
        }}
        .email-container {{
            max-width: 600px;
            margin: 0 auto;
            background: white;
            box-shadow: 0 4px 12px rgba(0,0,0,0.05);
        }}
        .header {{
            background: linear-gradient(135deg, #FF9900 0%, #FF6600 100%);
            padding: 40px 30px;
            text-align: center;
            position: relative;
            overflow: hidden;
        }}
        .header::before {{
            content: '';
            position: absolute;
            top: -50%;
            left: -50%;
            width: 200%;
            height: 200%;
            background: url('data:image/svg+xml,<svg xmlns=""http://www.w3.org/2000/svg"" viewBox=""0 0 100 100""><defs><pattern id=""grain"" width=""100"" height=""100"" patternUnits=""userSpaceOnUse""><circle cx=""50"" cy=""50"" r=""1"" fill=""%23ffffff"" opacity=""0.1""/></pattern></defs><rect width=""100"" height=""100"" fill=""url(%23grain)""/></svg>');
            animation: float 20s infinite linear;
        }}
        @keyframes float {{
            0% {{ transform: translate(-50%, -50%) rotate(0deg); }}
            100% {{ transform: translate(-50%, -50%) rotate(360deg); }}
        }}
        .logo {{
            font-size: 32px;
            font-weight: bold;
            color: white;
            margin-bottom: 10px;
            position: relative;
            z-index: 2;
        }}
        .header-subtitle {{
            color: rgba(255,255,255,0.9);
            font-size: 18px;
            position: relative;
            z-index: 2;
        }}
        .content {{
            padding: 50px 40px;
            background: white;
        }}
        .welcome-badge {{
            display: inline-block;
            background: linear-gradient(135deg, #00A8E6 0%, #0073E6 100%);
            color: white;
            padding: 8px 20px;
            border-radius: 20px;
            font-size: 14px;
            font-weight: 600;
            margin-bottom: 30px;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }}
        .content h1 {{
            font-size: 28px;
            color: #0F1111;
            margin-bottom: 20px;
            font-weight: 400;
        }}
        .content p {{
            font-size: 16px;
            color: #565959;
            margin-bottom: 20px;
            line-height: 1.7;
        }}
        
        /* Fixed Button Styles with High Specificity */
        .verify-button,
        a.verify-button,
        a.verify-button:link,
        a.verify-button:visited,
        a.verify-button:hover,
        a.verify-button:active,
        div.email-container a.verify-button,
        div.email-container a.verify-button:link,
        div.email-container a.verify-button:visited {{
            display: inline-block !important;
            background: linear-gradient(135deg, #FF9900 0%, #FF6600 100%) !important;
            color: white !important;
            padding: 16px 32px !important;
            text-decoration: none !important;
            border-radius: 8px !important;
            font-weight: 600 !important;
            font-size: 16px !important;
            margin: 30px 0 !important;
            transition: all 0.3s ease !important;
            box-shadow: 0 4px 12px rgba(255, 153, 0, 0.3) !important;
            text-align: center !important;
            min-width: 200px !important;
            border: none !important;
        }}
        
        .verify-button:hover,
        a.verify-button:hover,
        div.email-container a.verify-button:hover {{
            transform: translateY(-2px) !important;
            box-shadow: 0 6px 20px rgba(255, 153, 0, 0.4) !important;
            color: white !important;
        }}
        
        .button-container {{
            text-align: center;
            margin: 40px 0;
        }}
        .security-notice {{
            background: #F0F8FF;
            border-left: 4px solid #0073E6;
            padding: 20px;
            margin: 30px 0;
            border-radius: 0 8px 8px 0;
        }}
        .security-notice h3 {{
            color: #0073E6;
            font-size: 16px;
            margin-bottom: 10px;
            display: flex;
            align-items: center;
        }}
        .security-notice p {{
            color: #0F1111;
            margin-bottom: 0;
            font-size: 14px;
        }}
        .backup-link {{
            background: #F7F8F8;
            padding: 20px;
            border-radius: 8px;
            margin: 30px 0;
            border: 1px solid #DDD;
        }}
        .backup-link p {{
            margin-bottom: 10px;
            font-size: 14px;
            color: #565959;
        }}
        .backup-link a,
        .backup-link a:link,
        .backup-link a:visited {{
            color: #0073E6 !important;
            word-break: break-all;
            font-family: 'Courier New', monospace;
            font-size: 12px;
            background: white;
            padding: 10px;
            border-radius: 4px;
            display: block;
            border: 1px solid #DDD;
            text-decoration: none !important;
        }}
        .footer {{
            background: #232F3E;
            padding: 40px 30px;
            text-align: center;
            color: #DDD;
        }}
        .footer-links {{
            margin-bottom: 20px;
        }}
        .footer-links a,
        .footer-links a:link,
        .footer-links a:visited {{
            color: #DDD !important;
            text-decoration: none !important;
            margin: 0 15px;
            font-size: 14px;
        }}
        .footer-links a:hover {{
            color: #FF9900 !important;
        }}
        .footer p {{
            font-size: 12px;
            color: #999;
            margin: 5px 0;
        }}
        .social-icons {{
            margin: 20px 0;
        }}
        .social-icons a,
        .social-icons a:link,
        .social-icons a:visited {{
            display: inline-block !important;
            width: 40px;
            height: 40px;
            background: #3A4553 !important;
            border-radius: 50%;
            margin: 0 5px;
            line-height: 40px;
            text-align: center;
            color: #DDD !important;
            text-decoration: none !important;
            transition: background 0.3s ease;
        }}
        .social-icons a:hover {{
            background: #FF9900 !important;
        }}
        @media (max-width: 600px) {{
            .content {{
                padding: 30px 20px;
            }}
            .header {{
                padding: 30px 20px;
            }}
            .content h1 {{
                font-size: 24px;
            }}
            .verify-button {{
                padding: 14px 24px !important;
                font-size: 14px !important;
            }}
        }}
    </style>
</head>
<body>
    <div class='email-container'>
        <div class='header'>
            <div class='logo'>YourApp</div>
            <div class='header-subtitle'>Welcome to the Community</div>
        </div>
        
        <div class='content'>
            <div class='welcome-badge'>Welcome</div>
            
            <h1>Verify Your Email Address</h1>
            
            <p>Welcome! We're excited to have you join our community. To get started and ensure the security of your account, please verify your email address.</p>
            
            <div class='button-container'>
                <a href='{verificationLink}' 
                   class='verify-button'
                   style='display: inline-block !important; background: linear-gradient(135deg, #FF9900 0%, #FF6600 100%) !important; color: white !important; padding: 16px 32px !important; text-decoration: none !important; border-radius: 8px !important; font-weight: 600 !important; font-size: 16px !important; margin: 30px 0 !important; box-shadow: 0 4px 12px rgba(255, 153, 0, 0.3) !important; text-align: center !important; min-width: 200px !important; border: none !important;'>
                   Verify Email Address
                </a>
            </div>
            
            <div class='security-notice'>
                <h3>🔒 Security First</h3>
                <p>This verification helps us keep your account secure and ensures you receive important updates about your account.</p>
            </div>
            
            <div class='backup-link'>
                <p><strong>Having trouble with the button?</strong> Copy and paste this link into your browser:</p>
                <a href='{verificationLink}'>{verificationLink}</a>
            </div>
            
            <p>This verification link will expire in <strong>24 hours</strong> for security reasons.</p>
            
            <p>If you didn't create an account with us, please ignore this email or contact our support team if you have concerns.</p>
        </div>
        
        <div class='footer'>
            <div class='footer-links'>
                <a href='#'>Help Center</a>
                <a href='#'>Privacy Policy</a>
                <a href='#'>Terms of Service</a>
                <a href='#'>Contact Us</a>
            </div>
            
            <div class='social-icons'>
                <a href='#'>f</a>
                <a href='#'>t</a>
                <a href='#'>in</a>
                <a href='#'>ig</a>
            </div>
            
            <p>© 2025 YourApp, Inc. All rights reserved.</p>
            <p>This is an automated message. Please do not reply to this email.</p>
            <p>YourApp, 123 Business Street, City, State 12345</p>
        </div>
    </div>
</body>
</html>";

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendPasswordResetEmailAsync(string email, string resetLink, string userName)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("YourApp Security Team", _emailSettings.FromEmail));
                message.To.Add(new MailboxAddress("", email));
                message.Subject = "🔐 Password Reset Request - Secure Your Account";

                var bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Password Reset</title>
    <style>
        /* Gmail CSS Reset */
        u + #body a,
        #MessageViewBody a,
        a[x-apple-data-detectors],
        .ii a[href] {{
            color: inherit !important;
            text-decoration: inherit !important;
            font-size: inherit !important;
            font-family: inherit !important;
            font-weight: inherit !important;
            line-height: inherit !important;
        }}
        
        * {{
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }}
        body {{
            font-family: 'Amazon Ember', 'Helvetica Neue', Roboto, Arial, sans-serif;
            line-height: 1.6;
            color: #0F1111;
            background-color: #F7F8F8;
            -webkit-font-smoothing: antialiased;
            -moz-osx-font-smoothing: grayscale;
        }}
        .email-container {{
            max-width: 600px;
            margin: 0 auto;
            background: white;
            box-shadow: 0 4px 12px rgba(0,0,0,0.05);
        }}
        .header {{
            background: linear-gradient(135deg, #E74C3C 0%, #C0392B 100%);
            padding: 40px 30px;
            text-align: center;
            position: relative;
            overflow: hidden;
        }}
        .header::before {{
            content: '';
            position: absolute;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background: url('data:image/svg+xml,<svg xmlns=""http://www.w3.org/2000/svg"" viewBox=""0 0 100 100""><defs><pattern id=""dots"" width=""20"" height=""20"" patternUnits=""userSpaceOnUse""><circle cx=""10"" cy=""10"" r=""1"" fill=""%23ffffff"" opacity=""0.1""/></pattern></defs><rect width=""100"" height=""100"" fill=""url(%23dots)""/></svg>');
        }}
        .security-icon {{
            font-size: 48px;
            margin-bottom: 15px;
            position: relative;
            z-index: 2;
            display: inline-block;
            padding: 20px;
            background: rgba(255,255,255,0.1);
            border-radius: 50%;
            backdrop-filter: blur(10px);
        }}
        .logo {{
            font-size: 24px;
            font-weight: bold;
            color: white;
            margin-bottom: 5px;
            position: relative;
            z-index: 2;
        }}
        .header-subtitle {{
            color: rgba(255,255,255,0.9);
            font-size: 16px;
            position: relative;
            z-index: 2;
        }}
        .content {{
            padding: 50px 40px;
            background: white;
        }}
        .alert-badge {{
            display: inline-block;
            background: linear-gradient(135deg, #E74C3C 0%, #C0392B 100%);
            color: white;
            padding: 8px 20px;
            border-radius: 20px;
            font-size: 14px;
            font-weight: 600;
            margin-bottom: 30px;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }}
        .content h1 {{
            font-size: 28px;
            color: #0F1111;
            margin-bottom: 10px;
            font-weight: 400;
        }}
        .username {{
            color: #FF9900;
            font-weight: 600;
        }}
        .content p {{
            font-size: 16px;
            color: #565959;
            margin-bottom: 20px;
            line-height: 1.7;
        }}
        
        /* Fixed Reset Button Styles */
        .reset-button,
        a.reset-button,
        a.reset-button:link,
        a.reset-button:visited,
        a.reset-button:hover,
        a.reset-button:active,
        div.email-container a.reset-button,
        div.email-container a.reset-button:link,
        div.email-container a.reset-button:visited {{
            display: inline-block !important;
            background: linear-gradient(135deg, #E74C3C 0%, #C0392B 100%) !important;
            color: white !important;
            padding: 18px 36px !important;
            text-decoration: none !important;
            border-radius: 8px !important;
            font-weight: 600 !important;
            font-size: 16px !important;
            margin: 30px 0 !important;
            transition: all 0.3s ease !important;
            box-shadow: 0 4px 12px rgba(231, 76, 60, 0.3) !important;
            text-align: center !important;
            min-width: 220px !important;
            border: none !important;
        }}
        
        .reset-button:hover,
        a.reset-button:hover,
        div.email-container a.reset-button:hover {{
            transform: translateY(-2px) !important;
            box-shadow: 0 6px 20px rgba(231, 76, 60, 0.4) !important;
            color: white !important;
        }}
        
        .button-container {{
            text-align: center;
            margin: 40px 0;
        }}
        .security-warning {{
            background: linear-gradient(135deg, #FFF3E0 0%, #FFE0B2 100%);
            border: 2px solid #FF9800;
            padding: 25px;
            margin: 30px 0;
            border-radius: 12px;
            position: relative;
        }}
        .security-warning::before {{
            content: '⚠️';
            position: absolute;
            top: -15px;
            left: 20px;
            background: #FF9800;
            color: white;
            width: 30px;
            height: 30px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 16px;
        }}
        .security-warning h3 {{
            color: #E65100;
            font-size: 18px;
            margin-bottom: 15px;
            margin-top: 10px;
        }}
        .security-warning ul {{
            color: #BF360C;
            margin-left: 20px;
        }}
        .security-warning li {{
            margin-bottom: 8px;
            font-size: 15px;
        }}
        .backup-link {{
            background: #F7F8F8;
            padding: 25px;
            border-radius: 8px;
            margin: 30px 0;
            border: 1px solid #DDD;
        }}
        .backup-link p {{
            margin-bottom: 15px;
            font-size: 14px;
            color: #565959;
        }}
        .backup-link a,
        .backup-link a:link,
        .backup-link a:visited {{
            color: #0073E6 !important;
            word-break: break-all;
            font-family: 'Courier New', monospace;
            font-size: 12px;
            background: white;
            padding: 15px;
            border-radius: 6px;
            display: block;
            border: 1px solid #DDD;
            transition: border-color 0.3s ease;
            text-decoration: none !important;
        }}
        .backup-link a:hover {{
            border-color: #0073E6;
        }}
        .stats-container {{
            display: flex;
            justify-content: space-around;
            margin: 30px 0;
            background: #F8F9FA;
            padding: 20px;
            border-radius: 8px;
        }}
        .stat-item {{
            text-align: center;
        }}
        .stat-number {{
            font-size: 24px;
            font-weight: bold;
            color: #E74C3C;
            display: block;
        }}
        .stat-label {{
            font-size: 12px;
            color: #565959;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }}
        .footer {{
            background: #232F3E;
            padding: 40px 30px;
            text-align: center;
            color: #DDD;
        }}
        .footer-links {{
            margin-bottom: 20px;
        }}
        .footer-links a,
        .footer-links a:link,
        .footer-links a:visited {{
            color: #DDD !important;
            text-decoration: none !important;
            margin: 0 15px;
            font-size: 14px;
        }}
        .footer-links a:hover {{
            color: #FF9900 !important;
        }}
        .footer p {{
            font-size: 12px;
            color: #999;
            margin: 5px 0;
        }}
        .trust-indicators {{
            display: flex;
            justify-content: center;
            gap: 20px;
            margin: 20px 0;
            flex-wrap: wrap;
        }}
        .trust-item {{
            display: flex;
            align-items: center;
            gap: 5px;
            font-size: 12px;
            color: #999;
        }}
        @media (max-width: 600px) {{
            .content {{
                padding: 30px 20px;
            }}
            .header {{
                padding: 30px 20px;
            }}
            .content h1 {{
                font-size: 24px;
            }}
            .reset-button {{
                padding: 16px 28px !important;
                font-size: 14px !important;
            }}
            .stats-container {{
                flex-direction: column;
                gap: 15px;
            }}
            .trust-indicators {{
                flex-direction: column;
                gap: 10px;
            }}
        }}
    </style>
</head>
<body>
    <div class='email-container'>
        <div class='header'>
            <div class='security-icon'>🔐</div>
            <div class='logo'>YourApp Security</div>
            <div class='header-subtitle'>Account Protection Service</div>
        </div>
        
        <div class='content'>
            <div class='alert-badge'>Security Alert</div>
            
            <h1>Password Reset Request</h1>
            
            <p>Hello <span class='username'>{userName}</span>,</p>
            
            <p>We received a request to reset the password for your account. This is a security-sensitive action, so we want to make sure it's really you.</p>
            
            <div class='stats-container'>
                <div class='stat-item'>
                    <span class='stat-number'>24</span>
                    <span class='stat-label'>Hours to Reset</span>
                </div>
                <div class='stat-item'>
                    <span class='stat-number'>🔒</span>
                    <span class='stat-label'>Secure Process</span>
                </div>
                <div class='stat-item'>
                    <span class='stat-number'>1</span>
                    <span class='stat-label'>Time Use Only</span>
                </div>
            </div>
            
            <div class='button-container'>
                <a href='{resetLink}' 
                   class='reset-button'
                   style='display: inline-block !important; background: linear-gradient(135deg, #E74C3C 0%, #C0392B 100%) !important; color: white !important; padding: 18px 36px !important; text-decoration: none !important; border-radius: 8px !important; font-weight: 600 !important; font-size: 16px !important; margin: 30px 0 !important; box-shadow: 0 4px 12px rgba(231, 76, 60, 0.3) !important; text-align: center !important; min-width: 220px !important; border: none !important;'>
                   Reset My Password
                </a>
            </div>
            
            <div class='security-warning'>
                <h3>Important Security Information</h3>
                <ul>
                    <li><strong>Link expires in 24 hours</strong> - This ensures your account stays secure</li>
                    <li><strong>One-time use only</strong> - The link becomes invalid after first use</li>
                    <li><strong>Didn't request this?</strong> You can safely ignore this email</li>
                    <li><strong>Never share this link</strong> - It provides access to your account</li>
                    <li><strong>Check the sender</strong> - This email is from {_emailSettings.FromEmail}</li>
                </ul>
            </div>
            
            <div class='backup-link'>
                <p><strong>Button not working?</strong> Copy and paste this secure link into your browser:</p>
                <a href='{resetLink}'>{resetLink}</a>
            </div>
            
            <p>If you have any questions or concerns about this password reset request, please contact our support team immediately. We're here to help keep your account secure.</p>
            
            <p>Best regards,<br>
            <strong>The YourApp Security Team</strong></p>
        </div>
        
        <div class='footer'>
            <div class='footer-links'>
                <a href='#'>Security Center</a>
                <a href='#'>Contact Support</a>
                <a href='#'>Privacy Policy</a>
                <a href='#'>Account Help</a>
            </div>
            
            <div class='trust-indicators'>
                <div class='trust-item'>
                    <span>🔒</span>
                    <span>SSL Encrypted</span>
                </div>
                <div class='trust-item'>
                    <span>🛡️</span>
                    <span>SOC 2 Compliant</span>
                </div>
                <div class='trust-item'>
                    <span>✅</span>
                    <span>GDPR Compliant</span>
                </div>
            </div>
            
            <p>© 2025 YourApp, Inc. All rights reserved.</p>
            <p>This is an automated security message. Please do not reply to this email.</p>
            <p>YourApp Security Team • 123 Business Street, City, State 12345</p>
        </div>
    </div>
</body>
</html>";

                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation($"Password reset email sent successfully to {email}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send password reset email to {email}");
                throw;
            }
        }

        public async Task SendWelcomeEmailAsync(string email, string userName)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("YourApp Team", _emailSettings.FromEmail));
                message.To.Add(new MailboxAddress("", email));
                message.Subject = "🎉 Welcome to YourApp - Let's Get Started!";

                var bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = $@"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Welcome to YourApp</title>
    <style>
        /* Gmail CSS Reset */
        u + #body a,
        #MessageViewBody a,
        a[x-apple-data-detectors],
        .ii a[href] {{
            color: inherit !important;
            text-decoration: inherit !important;
            font-size: inherit !important;
            font-family: inherit !important;
            font-weight: inherit !important;
            line-height: inherit !important;
        }}
        
        * {{
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }}
        body {{
            font-family: 'Amazon Ember', 'Helvetica Neue', Roboto, Arial, sans-serif;
            line-height: 1.6;
            color: #0F1111;
            background-color: #F7F8F8;
        }}
        .email-container {{
            max-width: 600px;
            margin: 0 auto;
            background: white;
            box-shadow: 0 4px 12px rgba(0,0,0,0.05);
        }}
        .header {{
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            padding: 50px 30px;
            text-align: center;
            position: relative;
            overflow: hidden;
        }}
        .celebration-icon {{
            font-size: 64px;
            margin-bottom: 20px;
            animation: bounce 2s infinite;
        }}
        @keyframes bounce {{
            0%, 20%, 50%, 80%, 100% {{ transform: translateY(0); }}
            40% {{ transform: translateY(-10px); }}
            60% {{ transform: translateY(-5px); }}
        }}
        .logo {{
            font-size: 32px;
            font-weight: bold;
            color: white;
            margin-bottom: 10px;
        }}
        .content {{
            padding: 50px 40px;
        }}
        .welcome-message {{
            text-align: center;
            margin-bottom: 40px;
        }}
        .welcome-message h1 {{
            font-size: 32px;
            color: #0F1111;
            margin-bottom: 15px;
        }}
        .username {{
            color: #667eea;
            font-weight: 600;
        }}
        
        /* Fixed CTA Button Styles */
        .cta-button,
        a.cta-button,
        a.cta-button:link,
        a.cta-button:visited,
        a.cta-button:hover,
        a.cta-button:active,
        div.email-container a.cta-button,
        div.email-container a.cta-button:link,
        div.email-container a.cta-button:visited {{
            display: inline-block !important;
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%) !important;
            color: white !important;
            padding: 16px 32px !important;
            text-decoration: none !important;
            border-radius: 8px !important;
            font-weight: 600 !important;
            font-size: 16px !important;
            margin: 20px 10px !important;
            transition: all 0.3s ease !important;
            box-shadow: 0 4px 12px rgba(102, 126, 234, 0.3) !important;
            border: none !important;
        }}
        
        .cta-button:hover,
        a.cta-button:hover,
        div.email-container a.cta-button:hover {{
            transform: translateY(-2px) !important;
            box-shadow: 0 6px 20px rgba(102, 126, 234, 0.4) !important;
            color: white !important;
        }}
        
        .cta-button.secondary,
        a.cta-button.secondary,
        a.cta-button.secondary:link,
        a.cta-button.secondary:visited,
        div.email-container a.cta-button.secondary {{
            background: linear-gradient(135deg, #28a745 0%, #20c997 100%) !important;
            color: white !important;
        }}
        
        .features-grid {{
            display: grid;
            grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
            gap: 20px;
            margin: 40px 0;
        }}
        .feature-card {{
            background: #F8F9FA;
            padding: 25px;
            border-radius: 12px;
            text-align: center;
            border: 1px solid #E9ECEF;
            transition: transform 0.3s ease;
        }}
        .feature-card:hover {{
            transform: translateY(-5px);
            box-shadow: 0 8px 25px rgba(0,0,0,0.1);
        }}
        .feature-icon {{
            font-size: 36px;
            margin-bottom: 15px;
        }}
        .feature-title {{
            font-size: 18px;
            font-weight: 600;
            color: #0F1111;
            margin-bottom: 10px;
        }}
        .feature-description {{
            font-size: 14px;
            color: #565959;
        }}
        .help-center-link,
        a.help-center-link,
        a.help-center-link:link,
        a.help-center-link:visited {{
            color: #0D47A1 !important;
            text-decoration: none !important;
            font-weight: 600;
        }}
        .footer {{
            background: #232F3E;
            padding: 40px 30px;
            text-align: center;
            color: #DDD;
        }}
        .footer-links {{
            margin-bottom: 20px;
        }}
        .footer-links a,
        .footer-links a:link,
        .footer-links a:visited {{
            color: #DDD !important;
            text-decoration: none !important;
            margin: 0 15px;
            font-size: 14px;
        }}
        .footer-links a:hover {{
            color: #FF9900 !important;
        }}
        .footer p {{
            font-size: 12px;
            color: #999;
            margin: 5px 0;
        }}
        @media (max-width: 600px) {{
            .content {{
                padding: 30px 20px;
            }}
            .header {{
                padding: 40px 20px;
            }}
            .welcome-message h1 {{
                font-size: 28px;
            }}
            .features-grid {{
                grid-template-columns: 1fr;
            }}
        }}
    </style>
</head>
<body>
    <div class='email-container'>
        <div class='header'>
            <div class='celebration-icon'>🎉</div>
            <div class='logo'>YourApp</div>
        </div>
        
        <div class='content'>
            <div class='welcome-message'>
                <h1>Welcome, <span class='username'>{userName}</span>!</h1>
                <p style='font-size: 18px; color: #565959; margin-bottom: 30px;'>
                    Thank you for joining our community. We're thrilled to have you on board!
                </p>
                
                <div style='text-align: center;'>
                    <a href='#' 
                       class='cta-button'
                       style='display: inline-block !important; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%) !important; color: white !important; padding: 16px 32px !important; text-decoration: none !important; border-radius: 8px !important; font-weight: 600 !important; font-size: 16px !important; margin: 20px 10px !important; box-shadow: 0 4px 12px rgba(102, 126, 234, 0.3) !important; border: none !important;'>
                       Get Started
                    </a>
                    <a href='#' 
                       class='cta-button secondary'
                       style='display: inline-block !important; background: linear-gradient(135deg, #28a745 0%, #20c997 100%) !important; color: white !important; padding: 16px 32px !important; text-decoration: none !important; border-radius: 8px !important; font-weight: 600 !important; font-size: 16px !important; margin: 20px 10px !important; box-shadow: 0 4px 12px rgba(40, 167, 69, 0.3) !important; border: none !important;'>
                       Explore Features
                    </a>
                </div>
            </div>
            
            <div class='features-grid'>
                <div class='feature-card'>
                    <div class='feature-icon'>🚀</div>
                    <div class='feature-title'>Quick Setup</div>
                    <div class='feature-description'>Get up and running in minutes with our intuitive onboarding process.</div>
                </div>
                
                <div class='feature-card'>
                    <div class='feature-icon'>🔒</div>
                    <div class='feature-title'>Secure & Private</div>
                    <div class='feature-description'>Your data is protected with enterprise-grade security and encryption.</div>
                </div>
                
                <div class='feature-card'>
                    <div class='feature-icon'>💡</div>
                    <div class='feature-title'>Smart Features</div>
                    <div class='feature-description'>Discover powerful tools designed to make your workflow more efficient.</div>
                </div>
                
                <div class='feature-card'>
                    <div class='feature-icon'>📞</div>
                    <div class='feature-title'>24/7 Support</div>
                    <div class='feature-description'>Our support team is always here to help you succeed.</div>
                </div>
            </div>
            
            <div style='background: linear-gradient(135deg, #E3F2FD 0%, #BBDEFB 100%); padding: 30px; border-radius: 12px; text-align: center; margin: 40px 0;'>
                <h3 style='color: #0D47A1; margin-bottom: 15px;'>Need Help Getting Started?</h3>
                <p style='color: #1565C0; margin-bottom: 20px;'>Check out our comprehensive guides and tutorials to make the most of your experience.</p>
                <a href='#' class='help-center-link'>Visit Help Center →</a>
            </div>
            
            <p style='text-align: center; font-size: 16px; color: #565959;'>
                Ready to dive in? We can't wait to see what you'll accomplish!
            </p>
            
            <p style='text-align: center; margin-top: 30px;'>
                Best regards,<br>
                <strong>The YourApp Team</strong>
            </p>
        </div>
        
        <div class='footer'>
            <div class='footer-links'>
                <a href='#'>Getting Started</a>
                <a href='#'>Help Center</a>
                <a href='#'>Community</a>
                <a href='#'>Contact Us</a>
            </div>
            
            <p>© 2025 YourApp, Inc. All rights reserved.</p>
            <p>YourApp Team • 123 Business Street, City, State 12345</p>
        </div>
    </div>
</body>
</html>";

                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation($"Welcome email sent successfully to {email}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send welcome email to {email}");
                throw;
            }
        }
    }
}