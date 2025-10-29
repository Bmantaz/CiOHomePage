namespace CiOHomePage.Server.Services;

public interface IEmailSender
{
 Task SendAsync(string toEmail, string subject, string plainTextBody);
}
