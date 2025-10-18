using PingIdentityApp.Models;

namespace PingIdentityApp.Services.Enrollment;

public interface IEnrollmentService
{
    /// <summary>
    /// Begins the enrollment process for a user.
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="email"></param>
    /// <returns></returns>
    Task BeginEnrollmentAsync(string groupId, string email);

    /// <summary>
    /// Gets or sets the active email invitations.
    /// </summary>
    IList<EmailInvitation> ActiveEmailInvitations { get; set; }
}