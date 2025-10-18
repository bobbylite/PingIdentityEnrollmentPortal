using Flurl;
using Microsoft.Extensions.Options;
using PingIdentityApp.Configuration;
using PingIdentityApp.Models;
using PingIdentityApp.Services.Email;

namespace PingIdentityApp.Services.Enrollment;

public class EnrollmentService : IEnrollmentService
{
    private readonly ILogger<EnrollmentService> _logger;
    private readonly IEmailService _emailService;
    private readonly MailGunOptions _mailGunOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnrollmentService"/> class.
    /// </summary>
    /// <param name="logger"></param>
    public EnrollmentService(
        ILogger<EnrollmentService> logger,
        IEmailService emailService,
        IOptions<MailGunOptions> mailGunOptions
    )
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(emailService);
        ArgumentNullException.ThrowIfNull(mailGunOptions);
        
        _mailGunOptions = mailGunOptions.Value;
        _logger = logger;
        _emailService = emailService;

        ActiveEmailInvitations = new List<EmailInvitation>();
    }

    /// <inheritdoc/>
    public IList<EmailInvitation> ActiveEmailInvitations { get; set; }

    /// <inheritdoc/>
    public async Task BeginEnrollmentAsync(string groupId, string email)
    {
        ArgumentNullException.ThrowIfNull(groupId);
        ArgumentNullException.ThrowIfNull(email);

        var invitation = new EmailInvitation
        {
            InvitationId = Guid.NewGuid(),
            GroupId = groupId,
            Email = email,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };
        var magicLink = _mailGunOptions.MagicLinkBaseUrl
            .AppendPathSegment("CompleteEnrollment")
            .SetQueryParam("invitationId", invitation.InvitationId);
        var isSuccessful = await _emailService.SendEmailAsync(
            email,
            "Enrollment Invitation",
            _emailService.BuildHtmlBody(magicLink)
        );

        if (isSuccessful)
        {
            invitation.Status = "Sent";
            ActiveEmailInvitations.Add(invitation);
            _logger.LogInformation("Enrollment email sent to '{Email}' for invitation ID '{InvitationId}'", email, invitation.InvitationId);

            return;
        }
        else
        {
            invitation.Status = "Failed";
            _logger.LogError("Failed to send enrollment email to '{Email}' for invitation ID '{InvitationId}'", email, invitation.InvitationId);
            
            return;
        }        
    }
}