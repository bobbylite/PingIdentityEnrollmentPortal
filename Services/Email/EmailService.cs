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
            .SetQueryParam("html", body);
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
    
    /// <inheritdoc/>
    public string BuildHtmlBody(string magicLink)
    {
        return $@"
            <!DOCTYPE html>
            <html lang=""en"">
            <head>
            <meta charset=""UTF-8"">
            <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
            <style>
                body {{
                font-family: 'Segoe UI', sans-serif;
                background-color: #f8f9fa;
                color: #333;
                padding: 20px;
                }}
                .container {{
                background: #fff;
                border-radius: 10px;
                padding: 30px;
                box-shadow: 0 2px 8px rgba(0,0,0,0.1);
                max-width: 600px;
                margin: auto;
                }}
                .btn {{
                display: inline-block;
                padding: 12px 24px;
                margin-top: 20px;
                background-color: #dc3545;
                color: #fff !important;
                text-decoration: none;
                border-radius: 30px;
                font-weight: 600;
                }}
                .btn:hover {{
                background-color: #c82333;
                }}
            </style>
            </head>
            <body>
            <div class=""container"">
                <h2>Welcome to PingOne Enrollment</h2>
                <p>You're almost there! Click below to complete your enrollment.</p>
                <p style=""text-align:center;"">
                <a href=""{magicLink}"" class=""btn"">Complete Enrollment</a>
                </p>
                <p>If the button above doesn't work, copy and paste the link below into your browser:</p>
                <p><a href=""{magicLink}"">{magicLink}</a></p>
            </div>
            </body>
            </html>";
    }
}