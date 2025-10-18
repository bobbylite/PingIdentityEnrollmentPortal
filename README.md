# 🧭 GetChatty Ltd. — Onboarding Enrollment Portal

The **GetChatty Onboarding Enrollment Portal** is a **secure, invitation-based registration experience** built to demonstrate how **PingIdentity’s PingOne** platform enables **frictionless employee onboarding** and **identity-driven enrollment workflows**.

Built with **.NET 8 Blazor Server**, **Bootstrap 5**, and **PingOne APIs**, this portal simulates how enterprises can manage employee access and provisioning securely through **invitation-only enrollment** tied to PingOne groups.

---

## 🚀 Overview

This portal complements the **GetChatty Kiosk Employee Portal** by focusing on the **initial onboarding process** — before a user has an account.

It demonstrates:

* **Secure invitation-based onboarding**
* **Dynamic group selection** from PingOne
* **PingOne integration** for user enrollment and provisioning
* **Modern Bootstrap UI/UX** for a professional onboarding experience

---

## 🧠 Key Features

* ✉️ **Invitation-Based Enrollment** — Ensures only invited users can register.
* 🔐 **PingOne Integration** — Uses PingOne’s Management API for group retrieval and onboarding flows.
* 🧾 **Dynamic Group List** — Department/group options come directly from PingOne’s directory.
* 💎 **Modern, Responsive UI** — Styled entirely with Bootstrap 5 and soft gradient backgrounds.
* ⚙️ **Extensible Architecture** — Easily connects to existing HR or IAM systems.
* 💬 **Enrollment Confirmation Page** — Displays enrollment success with clean visual feedback.

---

## 🧩 Architecture

```
+------------------------------+
|   Blazor Server Frontend     |
|   (.NET 8 Razor Views)       |
|          ↓                   |
|  EnrollmentService (C#)      |
|          ↓                   |
|  PingOne Management API      |
+-----------↑------------------+
            |
+-----------+------------------+
|   PingOne Platform           |
|   - Group Directory          |
|   - User Provisioning APIs   |
|   - Access Management        |
+------------------------------+
```

* **Frontend:** Blazor Server w/ Razor Views
* **Identity Platform:** PingOne (via API + SDK)
* **Auth Flow:** Invitation-based (no login required yet)
* **Integration Layer:** `IPingOneManagementService` and `IEnrollmentService`

---

## ⚙️ Configuration

### 1. PingOne Setup

1. Log in to your **PingOne Admin Console**.
2. Create or locate your **environment**.
3. Obtain the following from your PingOne app:

   * **Environment ID**
   * **Client ID**
   * **Client Secret** (if needed for Management API)
   * **Region Base URL** (e.g., `https://api.pingone.com/v1/environments/{envId}`)
4. Create a **Personal Access Token (PAT)** or API credential with:

   * `read:groups`
   * `write:users`
   * `invite:users`

---

### 2. App Settings

In your app configuration (e.g., `appsettings.json`):

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "PingOneAuthentication": {
    "Issuer": "https://api.pingone.com",
    "ClientId": "{ClientId}",
    "ClientSecret": "{ClientSecret}"
  },
  "MailGun": {
    "ApiBaseUrl": "https://api.mailgun.net/v3/",
    "ApiKey": "...",
    "SandboxDomain": "sandbox{123}.mailgun.org",
    "FromEmail": "GetChatty Ltd. <postmaster@sandbox{123}.mailgun.org>"
  },
  "AllowedHosts": "*"
}

```

---

## 🧪 Running Locally

### Prerequisites

* [.NET 8 SDK](https://dotnet.microsoft.com/download)
* PingOne Developer Account
* Visual Studio 2022 or VS Code

### Steps

```bash
# 1. Clone the repo
git clone https://github.com/getchatty-ltd/onboarding-portal.git
cd onboarding-portal

# 2. Restore dependencies
dotnet restore

# 3. Run the project
dotnet run

# 4. Open in your browser
https://localhost:5001
```

---

## 💡 Demo Flow

1. **User receives an invitation** (email or HR-triggered).
2. They visit the **enrollment portal** using a secure invitation link.
3. They enter their **work email** and select a **PingOne group** (e.g., Engineering, HR, Marketing).
4. The portal triggers `BeginEnrollment` — calling `EnrollmentService.BeginEnrollmentAsync()` to register the user.
5. A confirmation page (`EnrollmentBeginConfirmation.cshtml`) displays a success message and onboarding instructions.

---

## 📄 Example Views

### `BeginEnrollment.cshtml`

* Displays the onboarding form
* Dynamically loads PingOne groups via `IPingOneManagementService.GetGroupsAsync()`
* Posts `email` and `groupId` to the controller

### `EnrollmentBeginConfirmation.cshtml`

* Reads `TempData["SuccessMessage"]`
* Shows confirmation UI with a “Return to Home” button

---

## 📦 Technologies Used

| Category | Technology                   |
| -------- | ---------------------------- |
| Frontend | Blazor Server (.NET 8)       |
| Identity | PingOne Management API       |
| Styling  | Bootstrap 5                  |
| Language | C#                           |
| Hosting  | ASP.NET Core                 |
| IDE      | Visual Studio 2022 / VS Code |

---

## 🔒 Security Highlights

* Only invited users can enroll.
* Form posts use **server-side validation** for `email` and `groupId`.
* Enrollment calls are wrapped in a service layer (`IEnrollmentService`) to isolate PingOne credentials.
* Group selection list fetched securely from PingOne — no hardcoded group names.

---

## 🧰 Future Enhancements

* ✉️ Email-based invitation tokens (JWT or GUID verification)
* 🧩 Integration with PingOne DaVinci for adaptive onboarding workflows
* 🧾 Automatic role assignment via PingOne APIs
* 📊 Administrative dashboard for managing pending enrollments

---

## 👨‍💻 Author

**Developed by:** GetChatty Ltd.
**Engineer:** Bobby L.
**Stack:** .NET 8 • Blazor Server • PingOne • Bootstrap 5
