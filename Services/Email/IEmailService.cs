namespace PingIdentityApp.Services.Email;

public interface IEmailService
{
    /// <summary>
    /// Sends an email asynchronously.
    /// </summary>
    /// <param name="to"></param>
    /// <param name="subject"></param>
    /// <param name="body"></param>
    /// <returns></returns>
    Task<bool> SendEmailAsync(string to, string subject, string body);
}
