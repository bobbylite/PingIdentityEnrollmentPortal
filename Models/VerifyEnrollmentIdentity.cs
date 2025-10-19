using System.Text.Json.Serialization;

public class VerifyEnrollmentIdentity
{
    /// <summary>
    /// The verification code for the user.
    /// </summary>
    [JsonPropertyName("verificationCode")]
    public string? VerificationCode { get; set; }

    /// <summary>
    /// The invitation ID associated with the user.
    /// </summary>
    [JsonIgnore]
    public string? InvitationId { get; set; }
}
