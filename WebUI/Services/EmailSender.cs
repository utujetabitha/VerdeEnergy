using System.Net;
using System.Net.Mail;

namespace WebUI.Services;

public sealed class EmailSender
{
    private readonly IConfiguration _configuration;

    public EmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendContactMessageAsync(string senderName, string senderEmail, string interest, string message)
    {
        var smtpSettings = _configuration.GetSection("Smtp");
        var host = smtpSettings["Host"] ?? "smtp.gmail.com";
        var portText = smtpSettings["Port"] ?? "587";
        var enableSslText = smtpSettings["EnableSsl"] ?? "true";
        var username = smtpSettings["UserName"] ?? "utujetabitha@gmail.com";
        var password = smtpSettings["Password"];

        if (string.IsNullOrWhiteSpace(password) || password == "YOUR_GMAIL_APP_PASSWORD")
        {
            throw new InvalidOperationException("Email is not configured yet. Set a valid Gmail app password in appsettings.json under Smtp:Password.");
        }

        var fromAddress = smtpSettings["FromAddress"] ?? username;
        var toAddress = smtpSettings["ToAddress"] ?? "utujetabitha@gmail.com";

        var port = int.TryParse(portText, out var parsedPort) ? parsedPort : 587;
        var enableSsl = bool.TryParse(enableSslText, out var parsedSsl) && parsedSsl;

        using var client = new SmtpClient(host, port)
        {
            Credentials = new NetworkCredential(username, password),
            EnableSsl = enableSsl,
            UseDefaultCredentials = false
        };

        using var mail = new MailMessage
        {
            From = new MailAddress(fromAddress, "Verde Energy Contact"),
            Subject = $"New contact message: {interest}",
            Body = $"Name: {senderName}\nEmail: {senderEmail}\nInterest: {interest}\n\nMessage:\n{message}",
            IsBodyHtml = false
        };

        mail.To.Add(toAddress);
        await client.SendMailAsync(mail);
    }
}
