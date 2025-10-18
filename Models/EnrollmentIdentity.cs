using System.Text.Json.Serialization;

public class EnrollmentIdentity
{
    /// <summary>
    /// The invitation ID.
    /// </summary>
    public string? InvitationId { get; set; }

    /// <summary>
    /// The email address of the user.
    /// </summary>
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    /// <summary>
    /// The name of the user.
    /// </summary>
    [JsonPropertyName("name")]
    public EnrollmentIdentityName? Name { get; set; }

    /// <summary>
    /// The population of the user.
    /// </summary>
    [JsonPropertyName("population")]
    public EnrollmentPopulation? Population { get; set; }

    /// <summary>
    /// The lifecycle of the user.
    /// </summary>
    [JsonPropertyName("lifecycle")]
    public EnrollmentLifecycle? Lifecycle { get; set; }

    /// <summary>
    /// The username of the user.
    /// </summary>
    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonPropertyName("password")]
    public EnrollmentPassword? Password { get; set; }
}

public class EnrollmentPassword
{
    /// <summary>
    /// The password value.
    /// </summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }

    /// <summary>
    /// Indicates whether the user must change the password.
    /// </summary>
    [JsonPropertyName("forceChange")]
    public string? ForceChange { get; set; }
}

public class EnrollmentLifecycle
{
    /// <summary>
    /// The status of the lifecycle.
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    /// <summary>
    /// Indicates whether to suppress the verification code.
    /// </summary>
    [JsonPropertyName("suppressVerificationCode")]
    public string? SuppressVerificationCode { get; set; }
}

public class EnrollmentPopulation
{
    /// <summary>
    /// The ID of the population.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }
}

public class EnrollmentIdentityName
{
    /// <summary>
    /// The given name of the user.
    /// </summary>
    [JsonPropertyName("given")]
    public string? Given { get; set; }

    /// <summary>
    /// The family name of the user.
    /// </summary>
    [JsonPropertyName("family")]
    public string? Family { get; set; }
}