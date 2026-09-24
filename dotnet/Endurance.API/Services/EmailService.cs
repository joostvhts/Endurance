using System.Net;
using System.Net.Mail;
using System.Text.Json;
using Endurance.API.Interfaces;
using Endurance.API.Models;
using Endurance.API.Models.EmailTemplate;
using Endurance.API.Models.Settings;

namespace Endurance.API.Services;

public class EmailService : IEmailService
{
    private readonly SmtpSettings _smtpSettings;
    private readonly Settings _settings;
    private readonly ILogger<EmailService> _logger;
    
    public EmailService(SmtpSettings smtpSettings, Settings settings, ILogger<EmailService> logger)
    {
        _smtpSettings = smtpSettings;
        _settings = settings;
        _logger = logger;
    }
    
    public async Task SendEmail(WatchedClassModel watchedClassModel, ClassInfoModel classInfoModel)
    {
        EmailNotifierSettings? emailNotifierSettings =
            JsonSerializer.Deserialize<EmailNotifierSettings>(watchedClassModel.NotifierSettings.Settings);
        
        using (var client = CreateSmtpClient())
        {
            DateTime gmtTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(classInfoModel.StartDateTime, "W. Europe Standard Time");

            var body = BasicEmailTemplate.Body;
            body = body.Replace("{ActivityName}", classInfoModel.Name)
                .Replace("{Day}", gmtTime.ToShortDateString())
                .Replace("{Time}", gmtTime.ToShortTimeString())
                .Replace("{Venue}", classInfoModel.VenueName)
                .Replace("{UnsubscribeUrl}",
                    WebUtility.HtmlEncode(
                        $"{_settings.FrontendBaseUrl.TrimEnd('/')}/#action=unsubscribe&token={Uri.EscapeDataString(watchedClassModel.UnsubscribeToken!)}"));

            using var mailMessage = new MailMessage
            {
                From = new MailAddress(_smtpSettings.From),
                Subject = "There's a spot available!",
                Body = body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(emailNotifierSettings!.Email);

            await client.SendMailAsync(mailMessage);
        }
    }

    public async Task SendVerificationEmail(string emailAddress, string verificationUrl)
    {
        using var client = CreateSmtpClient();
        using var mailMessage = new MailMessage
        {
            From = new MailAddress(_smtpSettings.From),
            Subject = "Verify your Endurance email address",
            Body = VerificationEmailTemplate.Body.Replace("{VerificationUrl}",
                WebUtility.HtmlEncode(verificationUrl)),
            IsBodyHtml = true
        };
        mailMessage.To.Add(emailAddress);

        try
        {
            await client.SendMailAsync(mailMessage);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send verification email");
            throw;
        }
    }

    private SmtpClient CreateSmtpClient()
    {
        var client = new SmtpClient(_smtpSettings.Server, _smtpSettings.Port)
        {
            EnableSsl = _smtpSettings.EnableSsl
        };

        if (!string.IsNullOrWhiteSpace(_smtpSettings.Username))
        {
            client.Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password);
        }

        return client;
    }
}
