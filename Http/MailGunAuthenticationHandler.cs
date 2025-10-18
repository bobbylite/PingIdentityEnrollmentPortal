using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using Microsoft.Extensions.Options;
using PingIdentityApp.Configuration;

namespace PingIdentityApp.Http;

/// <summary>
/// Represents a handler responsible for authenticating HTTP requests.
/// </summary>
/// <remarks>
/// This class is used to authenticate HTTP requests.
/// </remarks>
/// <seealso cref="DelegatingHandler" />
/// <seealso cref="MailGunAuthenticationHandler" />
public class MailGunAuthenticationHandler : DelegatingHandler
{
    private readonly ILogger<MailGunAuthenticationHandler> _logger;
    private readonly MailGunOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="MailGunAuthenticationHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="options">The MailGun options.</param>
    /// <remarks>
    /// This constructor initializes a new instance of the <see cref="MailGunAuthenticationHandler"/> class.
    /// </remarks>
    /// <seealso cref="MailGunAuthenticationHandler" />
    public MailGunAuthenticationHandler(
        ILogger<MailGunAuthenticationHandler> logger,
        IOptionsMonitor<MailGunOptions> options)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(options);

        _options = options.CurrentValue;
        _logger = logger;
    }
        
    /// <summary>
    /// Sends an HTTP request.
    /// </summary>
    /// <param name="request">The HTTP request message.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <remarks>
    /// This method sends an HTTP request.
    /// </remarks>
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Sending HTTP request");
        
        HttpResponseMessage response = await PerformRequest(request, cancellationToken);
        
        _logger.LogDebug("The status code for the request response is '{StatusCode}'", response.StatusCode);

        if (response.StatusCode is HttpStatusCode.Unauthorized or HttpStatusCode.Forbidden or HttpStatusCode.BadRequest)
        {
            _logger.LogInformation("The HTTP response contains an invalid token, performing authorization");
            _logger.LogInformation("Retrying the request after performing authorization");
            response = await PerformRequest(request, cancellationToken);
        }
        
        _logger.LogTrace("Completed sending the request");

        return response;
    }

    private async Task<HttpResponseMessage> PerformRequest(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        _logger.LogTrace("Performing HTTP request");

        _logger.LogDebug("Setting the 'Authorization' header before performing the request");
        var basicAuthHeader = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"api:{_options.ApiKey}"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", basicAuthHeader);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
        request.Headers.Host = "api.mailgun.net";

        _logger.LogDebug("Continuing the request");
        HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
        
        _logger.LogDebug("Completed performing the request");
        
        return response;
    }
}
