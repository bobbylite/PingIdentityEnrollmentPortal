namespace PingIdentityApp.Models;

public class EmailInvitation
{
    /// <summary>
    /// Gets or sets the unique identifier for the email invitation.
    /// </summary>
    public Guid InvitationId { get; set; }

    /// <summary>
    /// Gets or sets the user ID of the invitee.
    /// </summary>
    public string? UserId { get; set; }

    /// <summary>
    /// Gets or sets the email address of the invitee.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the group ID associated with the invitation.
    /// </summary>
    public string? GroupId { get; set; }

    /// <summary>
    /// Gets or sets the status of the invitation.
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the invitation was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}