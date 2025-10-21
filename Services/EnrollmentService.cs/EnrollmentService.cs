using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Flurl;
using Microsoft.Extensions.Options;
using PingIdentityApp.Configuration;
using PingIdentityApp.Constants;
using PingIdentityApp.Data;
using PingIdentityApp.Models;
using PingIdentityApp.Services.Email;
using PingIdentityApp.Services.PingOne;

namespace PingIdentityApp.Services.Enrollment;

public class EnrollmentService : IEnrollmentService
{
    private readonly ILogger<EnrollmentService> _logger;
    private readonly IEmailService _emailService;
    private readonly MailGunOptions _mailGunOptions;
    private readonly PingOneAuthenticationOptions _pingOneOptions;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IPingOneManagementService _pingOneManagementService;
    private readonly GetChattyDataContext _dataContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="EnrollmentService"/> class.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="emailService"></param>
    /// <param name="mailGunOptions"></param>
    /// <param name="pingOneOptions"></param>
    /// <param name="httpClientFactory"></param>
    /// <param name="pingOneManagementService"></param>
    public EnrollmentService(
        ILogger<EnrollmentService> logger,
        IEmailService emailService,
        IOptions<MailGunOptions> mailGunOptions,
        IOptions<PingOneAuthenticationOptions> pingOneOptions,
        IHttpClientFactory httpClientFactory,
        IPingOneManagementService pingOneManagementService,
        GetChattyDataContext dataContext
    )
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(emailService);
        ArgumentNullException.ThrowIfNull(mailGunOptions);
        ArgumentNullException.ThrowIfNull(pingOneOptions);
        ArgumentNullException.ThrowIfNull(httpClientFactory);
        ArgumentNullException.ThrowIfNull(pingOneManagementService);
        ArgumentNullException.ThrowIfNull(dataContext);

        _dataContext = dataContext;
        _httpClientFactory = httpClientFactory;
        _mailGunOptions = mailGunOptions.Value;
        _logger = logger;
        _pingOneOptions = pingOneOptions.Value;
        _pingOneManagementService = pingOneManagementService;
        _emailService = emailService;

        ActiveEmailInvitations = new List<EmailInvitation>();
        EnrolledIdentities = new List<EnrolledIdentity>();
    }

    /// <inheritdoc/>
    public IList<EmailInvitation> ActiveEmailInvitations { get; set; }

    /// <inheritdoc/>
    public IList<EnrolledIdentity> EnrolledIdentities { get; set; }

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

    /// <inheritdoc/>
    public async Task CompleteEnrollmentAsync(EnrollmentIdentity enrollmentIdentity)
    {
        ArgumentNullException.ThrowIfNull(enrollmentIdentity);

        var httpClient = _httpClientFactory.CreateClient(HttpClientNames.PingOneApi);
        var serializedEnrollmentIdentity = JsonSerializer.Serialize(enrollmentIdentity);
        _logger.LogInformation("Completing enrollment with data: '{SerializedIdentity}'", serializedEnrollmentIdentity);
        var content = new StringContent(serializedEnrollmentIdentity, Encoding.UTF8, new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.pingidentity.user.import+json"));
        var enrollmentResponse = await httpClient.PostAsync($"environments/{_pingOneOptions.EnvironmentId}/users", content);

        enrollmentResponse.EnsureSuccessStatusCode();

        var deserializedResponse = await enrollmentResponse.Content.ReadFromJsonAsync<PingOneUser>();

        if (deserializedResponse == null)
        {
            _logger.LogError("Failed to deserialize enrollment response for email '{Email}'", enrollmentIdentity.Email);
            throw new NullReferenceException("Failed to deserialize enrollment response.");
        }

        EnrolledIdentities.Add(new EnrolledIdentity
        {
            User = deserializedResponse,
            InvitationId = ActiveEmailInvitations.First(i => i.InvitationId.ToString() == enrollmentIdentity.InvitationId).InvitationId.ToString()
        });
    }

    /// <inheritdoc/>
    public async Task VerifyEnrollmentIdentityAsync(VerifyEnrollmentIdentity verifyEnrollmentIdentity)
    {
        ArgumentNullException.ThrowIfNull(verifyEnrollmentIdentity);

        var groupId = ActiveEmailInvitations.Where(e => e.InvitationId.ToString() == verifyEnrollmentIdentity.InvitationId)
            .Select(e => e.GroupId)
            .FirstOrDefault();
        var userId = EnrolledIdentities.Where(e => e.InvitationId == verifyEnrollmentIdentity.InvitationId)
            .Select(e => e.User?.Id)
            .FirstOrDefault();
        var verificationCode = verifyEnrollmentIdentity.VerificationCode;

        var user = await _pingOneManagementService.GetUserByIdAsync(userId!);
        ArgumentNullException.ThrowIfNull(user);
        var group = await _pingOneManagementService.GetGroupByIdAsync(groupId!);
        ArgumentNullException.ThrowIfNull(group);
        
        var httpClient = _httpClientFactory.CreateClient(HttpClientNames.PingOneApi);
        var serializedVerificationEnrollmentIdentity = JsonSerializer.Serialize(verifyEnrollmentIdentity);
        _logger.LogInformation("Completing enrollment with data: '{SerializedIdentity}'", serializedVerificationEnrollmentIdentity);
        var content = new StringContent(serializedVerificationEnrollmentIdentity, Encoding.UTF8, new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.pingidentity.user.verify+json"));
        var verificationResponse = await httpClient.PostAsync($"environments/{_pingOneOptions.EnvironmentId}/users/{userId}", content);

        verificationResponse.EnsureSuccessStatusCode();

        await _pingOneManagementService.ProvisionGroupMembershipAsync(userId!, _pingOneOptions.BirthRightGroupId);
        
        var AccessRequest = new Data.Entities.AccessRequest
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            GroupId = groupId,
            RequestedAt = DateTime.UtcNow,
            Status = AccessRequestStatus.Pending,
            FirstName = user?.Name?.Given,
            LastName = user?.Name?.Family,
            GroupName = group?.Name
        };
        _dataContext.AccessRequests.Add(AccessRequest);
        await  _dataContext.SaveChangesAsync();
    }
}