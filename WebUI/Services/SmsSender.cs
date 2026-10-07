using System.Net.Http.Headers;
using System.Text;
using Core.Domain.Entities;

namespace WebUI.Services;

public sealed class SmsSender(IHttpClientFactory httpClientFactory, IConfiguration configuration)
{
    public const string NotConfiguredMessage = "Twilio SMS is not configured.";

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(configuration["Sms:Twilio:AccountSid"]) &&
        !string.IsNullOrWhiteSpace(configuration["Sms:Twilio:AuthToken"]) &&
        !string.IsNullOrWhiteSpace(configuration["Sms:Twilio:FromNumber"]);

    public async Task<bool> SendMessageAsync(string recipient, string message)
    {
        var accountSid = configuration["Sms:Twilio:AccountSid"];
        var authToken = configuration["Sms:Twilio:AuthToken"];
        var fromNumber = configuration["Sms:Twilio:FromNumber"];

        if (!IsConfigured)
        {
            return false;
        }

        if (!IsValidPhoneNumber(recipient))
        {
            throw new InvalidOperationException("Enter valid phone numbers in international format, for example +2507XXXXXXXX.");
        }

        var client = httpClientFactory.CreateClient(nameof(SmsSender));
        var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{accountSid}:{authToken}"));
        using var request = new HttpRequestMessage(
            HttpMethod.Post,
            $"https://api.twilio.com/2010-04-01/Accounts/{Uri.EscapeDataString(accountSid!)}/Messages.json");
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        request.Content = new FormUrlEncodedContent(
        [
            new KeyValuePair<string, string>("To", recipient),
            new KeyValuePair<string, string>("From", fromNumber!),
            new KeyValuePair<string, string>("Body", message)
        ]);

        using var response = await client.SendAsync(request);
        if (!response.IsSuccessStatusCode)
        {
            throw new InvalidOperationException($"SMS delivery failed with status {(int)response.StatusCode}.");
        }

        return true;
    }

    private static bool IsValidPhoneNumber(string phoneNumber)
    {
        var digits = phoneNumber.StartsWith('+') ? phoneNumber[1..] : string.Empty;
        return digits.Length is >= 8 and <= 15 && digits.All(char.IsDigit);
    }
}