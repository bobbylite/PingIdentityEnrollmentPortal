using PingIdentityApp.Models;
using PingIdentityApp.Services.Email;

namespace PingIdentityApp.Services.Enrollment;

public class EnrollmentService : IEnrollmentService
{
    private readonly ILogger<EnrollmentService> _logger;
    private readonly IEmailService _emailService;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnrollmentService"/> class.
    /// </summary>
    /// <param name="logger"></param>
    public EnrollmentService(
        ILogger<EnrollmentService> logger,
        IEmailService emailService
    )
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(emailService);

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

        var isSuccessful = await _emailService.SendEmailAsync(
            email,
            "Enrollment Invitation",
            $"You are invited to enroll. Your invitation ID is {invitation.InvitationId}."
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