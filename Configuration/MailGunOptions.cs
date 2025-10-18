namespace PingIdentityApp.Configuration;

/// <summary>
/// Represents the configuration options for the PingOne API.
/// </summary>
/// <remarks>
/// This class represents the configuration options for the MailGun API.
/// </remarks>
public class MailGunOptions
{
    /// <summary>
    /// The configuration section key for the MailGun API.
    /// </summary>
    /// <value>The configuration section key for the MailGun API.</value>
    /// <remarks>
    /// This value is used to identify the configuration section for the MailGun API.
    /// </remarks>
    public const string SectionKey = "MailGun";

    /// <summary>
    /// Gets or sets the base URL for magic links.
    /// </summary>
    public string? MagicLinkBaseUrl { get; set; }

    /// <summary>
    /// Gets or sets the base URL for the MailGun API.
    /// </summary>
    public string? ApiBaseUrl { get; set; }

    /// <summary>
    /// Gets or sets the sandbox domain for the MailGun API.
    /// </summary>
    public string? SandboxDomain { get; set; }

    /// <summary>
    /// Gets or sets the API key for the MailGun API.
    /// </summary>
    public string? ApiKey { get; set; }

    /// <summary>
    /// Gets or sets the from email address for the MailGun API.
    /// </summary>
    public string? FromEmail { get; set; }
}