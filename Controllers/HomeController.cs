using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PingIdentityApp.Models;
using PingIdentityApp.Services.Enrollment;

namespace PingIdentityApp.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IEnrollmentService _enrollmentService;

    /// <summary>
    /// Initializes a new instance of the <see cref="HomeController"/> class.
    /// </summary>
    /// <param name="logger"></param>
    public HomeController(
        ILogger<HomeController> logger,
        IEnrollmentService enrollmentService)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(enrollmentService);

        _logger = logger;
        _enrollmentService = enrollmentService;
    }

    /// <summary>
    /// Index action to serve the home page.
    /// </summary>
    /// <returns></returns>
    public IActionResult Index()
    {
        return View();
    }

    /// <summary>
    /// Handles the Begin Enrollment form submission.
    /// </summary>
    /// <param name="email">The user's work email address.</param>
    /// <param name="groupId">The selected PingOne group ID.</param>
    /// <returns>Redirects to the next step in onboarding.</returns>
    [HttpPost]
    [Route("BeginEnrollment")]
    public async Task<IActionResult> BeginEnrollment(string email, string groupId)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(email);
        ArgumentNullException.ThrowIfNullOrEmpty(groupId);

        TempData["SuccessMessage"] = $"Enrollment initiated for {email} in group {groupId}.";

        await _enrollmentService.BeginEnrollmentAsync(groupId, email);

        return RedirectToAction("EnrollmentBeginConfirmation");
    }

    /// <summary>
    /// EnrollmentBeginConfirmation action to serve the enrollment confirmation page.
    /// </summary>
    /// <returns></returns>
    public IActionResult EnrollmentBeginConfirmation()
    {
        return View();
    }

    /// <summary>
    /// Certifications action to serve the certifications page.
    /// </summary>
    /// <returns></returns>
    public IActionResult Certifications()
    {
        ViewData["Message"] = "Your certification campaigns page.";
        return View();
    }

    /// <summary>
    /// Access action to serve the access requests page.
    /// </summary>
    /// <returns></returns>
    public IActionResult Access()
    {
        ViewData["Message"] = "Your access requests page.";
        return View();
    }

    /// <summary>
    /// EmployeeResources action to serve the employee resources page.
    /// </summary>
    /// <returns></returns>
    public IActionResult EmployeeResources()
    {
        ViewData["Message"] = "Your employee resources page.";
        return View();
    }

    /// <summary>
    /// VendorResources action to serve the vendor resources page.
    /// </summary>
    /// <returns></returns>
    public IActionResult VendorResources()
    {
        ViewData["Message"] = "Your vendor resources page.";
        return View();
    }

    /// <summary>
    /// CertificationsControl action to serve the certifications control page.
    /// </summary>
    /// <returns></returns>
    public IActionResult CertificationsControl()
    {
        ViewData["Message"] = "Your certification campaigns page.";
        return View();
    }

    /// <summary>
    /// Error action to handle errors.
    /// </summary>
    /// <returns></returns>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
