using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace Final_project.Services.SmsService
{
  
    public static class SmsSender
    {
        public static void SendOrderConfirmation(string toPhoneNumber, string orderId)
        {
            const string accountSid = "AC14344b98bdcaf64e3a444365897aeade";
            const string authToken = "24a426a0c57e9d6eea6347ce9410631a";
            const string twilioPhoneNumber = "+13305371181"; 

            TwilioClient.Init(accountSid, authToken);

            var message = MessageResource.Create(
                body: $"✅ Your order #{orderId} has been placed successfully! Thanks for shopping with us.",
                from: new PhoneNumber(twilioPhoneNumber),
                to: new PhoneNumber(toPhoneNumber)
            );
        }
    }

}
