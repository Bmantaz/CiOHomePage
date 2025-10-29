using System.Net;
using System.Net.Mail;

namespace CiOHomePage.Server.Services;

public class SmtpEmailSender(IConfiguration config, ILogger<SmtpEmailSender> logger) : IEmailSender
{
 private readonly IConfiguration _config = config;
 private readonly ILogger<SmtpEmailSender> _logger = logger;
 public async Task SendAsync(string toEmail, string subject, string plainTextBody)
 {
 var host = _config["Smtp:Host"];
 if (string.IsNullOrWhiteSpace(host)) { _logger.LogWarning("SMTP host not configured; skipping email to {to}", toEmail); return; }
 var port = int.TryParse(_config["Smtp:Port"], out var p) ? p :25;
 var user = _config["Smtp:User"];
 var pass = _config["Smtp:Pass"];
 var from = _config["Smtp:From"] ?? user ?? "noreply@example.com";
 using var client = new SmtpClient(host, port)
 {
 EnableSsl = true,
 Credentials = string.IsNullOrEmpty(user) ? CredentialCache.DefaultNetworkCredentials : new NetworkCredential(user, pass)
 };
 using var msg = new MailMessage(from, toEmail, subject, plainTextBody);
 try
 {
 await client.SendMailAsync(msg);
 }
 catch (Exception ex)
 {
 _logger.LogError(ex, "Failed to send email to {to}", toEmail);
 }
 }
}
