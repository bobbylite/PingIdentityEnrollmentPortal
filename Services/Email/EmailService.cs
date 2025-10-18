using Flurl;
using Microsoft.Extensions.Options;
using PingIdentityApp.Configuration;
using PingIdentityApp.Constants;

namespace PingIdentityApp.Services.Email;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    private readonly IOptions<MailGunOptions> _emailOptions;
    private readonly IHttpClientFactory _httpClientFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmailService"/> class.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="emailSettings"></param>
    /// <param name="enrollmentService"></param>
    public EmailService(
        ILogger<EmailService> logger,
        IOptions<MailGunOptions> emailOptions,
        IHttpClientFactory httpClientFactory
    )
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(emailOptions);
        ArgumentNullException.ThrowIfNull(httpClientFactory);

        _logger = logger;
        _emailOptions = emailOptions;
        _httpClientFactory = httpClientFactory;
    }

    /// <inheritdoc/>
    public async Task<bool> SendEmailAsync(string to, string subject, string body)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(to);
        ArgumentNullException.ThrowIfNullOrEmpty(subject);
        ArgumentNullException.ThrowIfNullOrEmpty(body);

        _logger.LogInformation("Sending email to '{To}' with subject '{Subject}'", to, subject);
        var httpClient = _httpClientFactory.CreateClient(HttpClientNames.MailGunApi);
        var mailGunEndpoint = $"{_emailOptions.Value.ApiBaseUrl}{_emailOptions.Value.SandboxDomain}/messages";
        
        var url = mailGunEndpoint
            .SetQueryParam("from", _emailOptions.Value.FromEmail)
            .SetQueryParam("to", to)
            .SetQueryParam("subject", subject)
            .SetQueryParam("text", body);
        var mailGunResponse = await httpClient.PostAsync(url, null);

        if (mailGunResponse.IsSuccessStatusCode)
        {
            _logger.LogInformation("Email sent successfully to '{To}'", to);
            return true;
        }
        else
        {
            _logger.LogError("Failed to send email to '{To}'. Status Code: {StatusCode}", to, mailGunResponse.StatusCode);
            return false;
        }
    }
}