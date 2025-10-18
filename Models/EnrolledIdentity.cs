using PingIdentityApp.Models;

public class EnrolledIdentity
{
    /// <summary>
    /// The enrolled user details.
    /// </summary>
    public PingOneUser? User { get; set; }

    /// <summary>
    /// The invitation ID associated with the enrolled identity.
    /// </summary>
    public string? InvitationId { get; set; }
}