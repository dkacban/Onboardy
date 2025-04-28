using Microsoft.SemanticKernel;
using System.ComponentModel;
using Twilio.Types;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace OnboardyAgent.Plugins;

[Description("This is SMS sending plugin")]
public class SmsPlugin
{
    public SmsPlugin()
    {
        TwilioClient.Init("AC14dfcfa2c6443f846de467d5b475e64c", "9d7d7a8991277d6011a2da57444dc539");
    }

    [KernelFunction]
    [Description("Send SMS message with given text as content")]
    public async Task<string> SendSMSMessage([Description("text of SMS")] string text, [Description("phone number")] string phone)
    {
        phone = $"+48{phone}";

        var message = MessageResource.Create(
            new PhoneNumber(phone),
            from: new PhoneNumber("+19544510632"),
            body: text
        );

        return "wiadomość została wysłana";
    }
}