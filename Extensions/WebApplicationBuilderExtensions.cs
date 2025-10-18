using PingIdentityApp.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Options;
using PingIdentityApp.Http;
using PingIdentityApp.Services.Token;
using PingIdentityApp.Constants;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using PingIdentityApp.Services.PingOne;
using PingIdentityApp.Services.Certification;
using PingIdentityApp.Services.Enrollment;
using PingIdentityApp.Services.Email;

namespace PingIdentityApp.Extensions;

public static class WebApplicationBuilderExtensions
{   
    /// <summary>
    /// Add services to the application.
    /// </summary>
    /// <param name="webApplicationBuilder">A builder for web applications and services.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <remarks>
    /// This method adds services to the application.
    /// </remarks>
    public static WebApplicationBuilder AddServices(this WebApplicationBuilder webApplicationBuilder)
    { 
        ArgumentNullException.ThrowIfNull(webApplicationBuilder);

        webApplicationBuilder.Services.AddServiceDiscovery();

        webApplicationBuilder.Services.ConfigureHttpClientDefaults(http =>
        {
            // Turn on service discovery by default
            http.AddServiceDiscovery();
        });

        // Add a singleton service of type ITokenService. The same instance of TokenService will be used every time ITokenService is requested.
        webApplicationBuilder.Services.AddSingleton<ITokenService, TokenService>();
        webApplicationBuilder.Services.AddSingleton<ICertificationService, CertificationService>();
        webApplicationBuilder.Services.AddSingleton<IEnrollmentService, EnrollmentService>();

        webApplicationBuilder.Services.AddTransient<IPingOneManagementService, PingOneManagementService>();
        webApplicationBuilder.Services.AddTransient<IEmailService, EmailService>();
        webApplicationBuilder.Services.AddHttpContextAccessor();
        webApplicationBuilder.Services.AddControllersWithViews();

        return webApplicationBuilder;
    }

    /// <summary>
    /// Add an HttpClient for the PingIdentityApp REST API. Includes handling authentication for the API by passing the access
    /// token for the currently logged in user as a JWT bearer token to the API.
    /// </summary>
    /// <param name="webApplicationBuilder">A builder for web applications and services.</param>
    /// <returns>A reference to this instance after the operation has completed of type <see cref="WebApplicationBuilder"/></returns>
    /// <remarks>
    /// This method adds an HttpClient for the PingIdentityApp REST API.
    /// </remarks>
    public static WebApplicationBuilder AddPingIdentityAppApiClients(this WebApplicationBuilder webApplicationBuilder)
    {
        ArgumentNullException.ThrowIfNull(webApplicationBuilder);

        webApplicationBuilder.Services.AddOptionsWithValidateOnStart<PingOneAuthenticationOptions>()
            .BindConfiguration(PingOneAuthenticationOptions.SectionKey);
        webApplicationBuilder.Services.AddOptionsWithValidateOnStart<MailGunOptions>()
            .BindConfiguration(MailGunOptions.SectionKey);

        webApplicationBuilder.Services.AddTransient<AuthenticationHandler>()
        .AddHttpClient(HttpClientNames.PingOneApi, (provider, client) =>
        {
            var apiOptionsSnapshot = provider.GetRequiredService<IOptionsMonitor<PingOneAuthenticationOptions>>();
            var apiOptions = apiOptionsSnapshot.CurrentValue;
            client.BaseAddress = new Uri($"{apiOptions.BaseUrl}");
        })
        .AddHttpMessageHandler<AuthenticationHandler>();
        
        webApplicationBuilder.Services.AddTransient<MailGunAuthenticationHandler>()
        .AddHttpClient(HttpClientNames.MailGunApi, (provider, client) =>
        {
            var apiOptionsSnapshot = provider.GetRequiredService<IOptionsMonitor<MailGunOptions>>();
            var apiOptions = apiOptionsSnapshot.CurrentValue;
            client.BaseAddress = new Uri($"{apiOptions.ApiBaseUrl}");
        })
        .AddHttpMessageHandler<MailGunAuthenticationHandler>();

        return webApplicationBuilder;
    }

    /// <summary>
    /// Add support for refreshing access tokens using refresh tokens.
    /// With .NET 8, the configuration for this can be controled completely from 'appsettings.json'.
    /// </summary>
    /// <param name="webApplicationBuilder">A builder for web applications and services.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    /// <remarks>
    /// This method adds support for refreshing access tokens using refresh tokens.
    /// </remarks>
    public static WebApplicationBuilder AddAccessControl(this WebApplicationBuilder webApplicationBuilder)
    {
        ArgumentNullException.ThrowIfNull(webApplicationBuilder);

        webApplicationBuilder.Services.AddAuthentication();
        webApplicationBuilder.Services.AddAuthorization();
        
        return webApplicationBuilder;
    }
}
