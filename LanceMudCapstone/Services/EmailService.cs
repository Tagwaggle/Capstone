using Azure;
using Azure.Communication.Email;

namespace LanceMudCapstone.Services;

public class EmailService
{
    private readonly EmailClient _client;
    private readonly string _senderAddress;

    public EmailService(IConfiguration config)
    {
        // Pull connection string from appsettings.json
        _client = new EmailClient(config["ACS:ConnectionString"]);

        // Your Azure-managed domain sender address
        _senderAddress = "donotreply@bcfdc616-edf8-4e7c-8e38-5298b289e9b2.azurecomm.net";
    }

    public async Task SendContactFormEmail(string name, string email, string message)
    {
        var content = new EmailContent($"New Contact Form Submission from {name}")
        {
            PlainText =
                $"Name: {name}\n" +
                $"Email: {email}\n\n" +
                $"Message:\n{message}"
        };

        var recipients = new EmailRecipients(
            new List<EmailAddress>
            {
            new EmailAddress("joseph.guynes@gmail.com")
            }
        );

        var emailMessage = new EmailMessage(
            senderAddress: _senderAddress,
            recipients: recipients,
            content: content
        );

        // Correct overload for ACS Email SDK 1.0+
        await _client.SendAsync(WaitUntil.Completed, emailMessage);
    }
}