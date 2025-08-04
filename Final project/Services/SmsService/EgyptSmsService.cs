using Twilio;
using Twilio.Rest.Api.V2010.Account;
using System.Threading.Tasks;

namespace Final_project.Services.SmsService
{
    public class EgyptSmsService : ISmsService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EgyptSmsService> _logger;
        private bool _isTrialAccount = false;
        private bool _accountTypeChecked = false;

        public EgyptSmsService(IConfiguration configuration, ILogger<EgyptSmsService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            // Initialize Twilio
            var accountSid = _configuration["TwilioSettings:AccountSid"];
            var authToken = _configuration["TwilioSettings:AuthToken"];
            TwilioClient.Init(accountSid, authToken);
        }

        private async Task CheckAccountTypeAsync()
        {
            if (!_accountTypeChecked)
            {
                try
                {
                    var account = await Twilio.Rest.Api.V2010.AccountResource.FetchAsync();
                    _isTrialAccount = account.Type.ToString() == "Trial";
                    _accountTypeChecked = true;

                    if (_isTrialAccount)
                    {
                        _logger.LogWarning("Twilio trial account detected. SMS can only be sent to verified numbers.");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to check Twilio account type");
                }
            }
        }

        public async Task<bool> SendSmsAsync(string phoneNumber, string message)
        {
            try
            {
                await CheckAccountTypeAsync();

                // Format Egyptian phone number
                var formattedNumber = FormatEgyptianPhoneNumber(phoneNumber);
                if (string.IsNullOrEmpty(formattedNumber))
                {
                    _logger.LogError($"Invalid Egyptian phone number format: {phoneNumber}");
                    return false;
                }

                var twilioPhoneNumber = _configuration["TwilioSettings:PhoneNumber"];

                // Create message with Egypt-specific settings
                var messageResource = await MessageResource.CreateAsync(
                    body: message,
                    from: new Twilio.Types.PhoneNumber(twilioPhoneNumber),
                    to: new Twilio.Types.PhoneNumber(formattedNumber),
                    // Optional: Use Alphanumeric Sender ID if registered
                    messagingServiceSid: _configuration["TwilioSettings:MessagingServiceSid"]
                );

                _logger.LogInformation($"SMS sent successfully to Egypt number {formattedNumber}. SID: {messageResource.Sid}");
                return true;
            }
            catch (Twilio.Exceptions.ApiException ex) when (ex.Code == 21608)
            {
                _logger.LogWarning($"Cannot send SMS to unverified number {phoneNumber} on trial account. Error: {ex.Message}");

                if (_isTrialAccount)
                {
                    _logger.LogInformation($"To fix this: Verify {phoneNumber} at https://console.twilio.com/us1/develop/phone-numbers/manage/verified");
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send SMS to Egyptian number {phoneNumber}");
                return false;
            }
        }

        public async Task<bool> SendVerificationCodeAsync(string phoneNumber, string code)
        {
            await CheckAccountTypeAsync();

            if (_isTrialAccount)
            {
                _logger.LogWarning($"Trial account - ensure {phoneNumber} is verified in Twilio console");
            }

            // Arabic-friendly message (optional)
            var useArabic = _configuration.GetValue<bool>("SmsSettings:UseArabicText", false);

            string message;
            if (useArabic)
            {
                message = $"رمز التحقق الخاص بك هو: {code}. هذا الرمز صالح لمدة 5 دقائق.";
            }
            else
            {
                message = $"Your verification code is: {code}. This code will expire in 5 minutes.";
            }

            return await SendSmsAsync(phoneNumber, message);
        }

        public async Task<bool> IsPhoneNumberUsableAsync(string phoneNumber)
        {
            await CheckAccountTypeAsync();

            if (!_isTrialAccount)
            {
                return IsValidEgyptianPhoneNumber(phoneNumber);
            }

            // For trial accounts, we can't really check if a number is verified
            // without trying to send to it, so we return the format validity
            return IsValidEgyptianPhoneNumber(phoneNumber);
        }

        public bool IsTrialAccount => _isTrialAccount;

        private string FormatEgyptianPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                return null;

            // Remove all non-digit characters
            var digitsOnly = System.Text.RegularExpressions.Regex.Replace(phoneNumber, @"[^\d]", "");

            // Handle different Egyptian phone number formats
            if (digitsOnly.Length == 11 && digitsOnly.StartsWith("01"))
            {
                // Format: 01XXXXXXXXX -> +201XXXXXXXXX
                return "+20" + digitsOnly;
            }
            else if (digitsOnly.Length == 10 && digitsOnly.StartsWith("1"))
            {
                // Format: 1XXXXXXXXX -> +201XXXXXXXXX
                return "+20" + digitsOnly;
            }
            else if (digitsOnly.Length == 13 && digitsOnly.StartsWith("201"))
            {
                // Format: 201XXXXXXXXX -> +201XXXXXXXXX
                return "+" + digitsOnly;
            }
            else if (digitsOnly.Length == 12 && digitsOnly.StartsWith("01"))
            {
                // Format: 201XXXXXXXXX (missing +) -> +201XXXXXXXXX
                return "+" + digitsOnly;
            }

            // If already in correct format
            if (phoneNumber.StartsWith("+201") && phoneNumber.Length == 14)
            {
                return phoneNumber;
            }

            _logger.LogWarning($"Unrecognized Egyptian phone number format: {phoneNumber}");
            return null;
        }

        public bool IsValidEgyptianPhoneNumber(string phoneNumber)
        {
            return !string.IsNullOrEmpty(FormatEgyptianPhoneNumber(phoneNumber));
        }
    }
}