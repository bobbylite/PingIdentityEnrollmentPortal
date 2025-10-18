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
    /// Completes the enrollment process for a user.
    /// </summary>
    /// <param name="firstName"></param>
    /// <param name="lastName"></param>
    /// <param name="password"></param>
    /// <param name="populationid"></param>
    /// <returns></returns>
    Task CompleteEnrollmentAsync(EnrollmentIdentity enrollmentIdentity);

    /// <summary>
    /// Verifies the enrollment identity for a user.
    /// </summary>
    /// <param name="verifyEnrollmentIdentity"></param>
    /// <returns></returns>
    Task VerifyEnrollmentIdentityAsync(VerifyEnrollmentIdentity verifyEnrollmentIdentity);

    /// <summary>
    /// Gets or sets the active email invitations.
    /// </summary>
    IList<EmailInvitation> ActiveEmailInvitations { get; set; }

    /// <summary>
    /// Gets or sets the enrolled identities.
    /// </summary>
    IList<EnrolledIdentity> EnrolledIdentities { get; set; }
}