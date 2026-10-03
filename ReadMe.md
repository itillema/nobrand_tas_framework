# No-brand architecture and framework - Test automation solution
This de-branded regression test automation solution was built for a past client project.  All client name and branding references have been removed.  I own all of the code in this repository.

The solution is a Playwright-based regression test suite for a public marketing, directory, and operations sites, built on .NET 8 with NUnit, Microsoft.Playwright, Serilog, and ExtentReports, organized around a gTAA 4-layer architecture. **This solution is not buildable, runnable, or testable as-is, because it contains no client-specific URLs, credentials, or other sensitive data.  It is provided as a reference for architecture and framework design.**

---

## Table of Contents

- [Overview](#overview)
- [Tech Stack](#tech-stack)
- [Architecture](#architecture-gtaa--4-layers)
- [Repository Structure](#repository-structure)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [Running Tests Locally](#running-tests-locally)
- [Test Inventory](#test-inventory)
- [CI/CD](#cicd)
- [Reporting & Logs](#reporting--logs)
- [Conventions](#conventions)
- [Adding New Tests](#adding-new-tests)
- [Sensitive Data](#sensitive-data)
- [Troubleshooting](#troubleshooting)

---

## Overview

This suite covers two locales (US + CA) and three environments (development, staging, production). Coverage focuses on the high-value lead-generation funnel and navigation surfaces.

**What it tests:**
- Home page and landing pages (body content, navigation, CTAs)
- Header and footer navigation (incl. booking modals)
- Locale pages (US + CA) — content, links, lead forms
- Lead form submissions across landing pages and individual locale pages
- Site-wide search, locale page search, and resource search
- Page-load smoke test checks for every US and CA locale page

**What this version does not test:**
- Backend APIs / CRM integrations
- Native mobile apps
- Internal admin or NoBrandaction-management portals

These scopes were built in my maintained version of the application, but are not included in this de-branded repository, to protect client intellectual property and privacy.

**Coverage at a glance:** 23 regression test fixtures across 9 functional areas. See [Test Inventory](#test-inventory) for the complete list.

---

## Tech Stack

| Concern | Choice | Version |
|---|---|---|
| Language | C# / .NET | 8.0 |
| Test framework | NUnit | 4.5.1 |
| Test adapter | NUnit3TestAdapter | 6.2.0 |
| Test SDK | Microsoft.NET.Test.Sdk | 18.5.0 |
| Analyzers | NUnit.Analyzers | 4.12.0 |
| Browser automation | Microsoft.Playwright | 1.60.0 |
| NUnit integration | Microsoft.Playwright.NUnit | 1.60.0 |
| Logging | Serilog (Console + File sinks) | 4.3.1 |
| HTML reporting | ExtentReports | 5.0.4 |
| Imaging (screenshots) | System.Drawing.Common | 10.0.7 |
| Code coverage | coverlet.collector + ReportGenerator | 10.0.0 |
| CI/CD | GitHub Actions (windows-latest runner) | — |

> **Browser:** Playwright drives a Playwright-managed **Chromium** — installed once via the CLI (see [Getting Started](#getting-started)). No separate browser driver or ChromeDriver version-matching is required.

---

## Architecture (gTAA — 4 Layers)

The suite follows the **generic Test Automation Architecture (gTAA)**, separating tests, page objects, external integrations, and shared utilities.

```
┌─────────────────────────────────────────────────────────┐
│  Execution/                 ← Test fixtures (NUnit)     │
└──────────────────────┬──────────────────────────────────┘
                       ▼
┌─────────────────────────────────────────────────────────┐
│  Definition/                ← Page objects + TestData   │
└──────────────────────┬──────────────────────────────────┘
                       ▼
┌─────────────────────────────────────────────────────────┐
│  Adaptation/                ← External service wrappers │
└──────────────────────┬──────────────────────────────────┘
                       ▼
┌─────────────────────────────────────────────────────────┐
│  Utility/                   ← Base + constants + infra  │
└─────────────────────────────────────────────────────────┘

Dependency flow: Execution → Definition → Adaptation → (all use) Utility
```

| Layer | Responsibility |
|---|---|
| **Execution** | NUnit `[TestFixture]` classes that orchestrate test scenarios |
| **Definition** | Page Objects, reusable UI modules (drawers/modals), test data DTOs, and the TestScriptMatrix docs |
| **Adaptation** | Wrappers for external systems (DBs, APIs, integrations) — mostly placeholder stubs in this version of the repo |
| **Utility** | `UtilityBase` (Playwright browser/page lifecycle), `TestConstants` (timeouts), `PageNavigation` (navigation extensions), Serilog setup, ExtentReports manager |

For deeper architectural guidance, see [.agent/skills/gtaa-architecture/SKILL.md](.agent/skills/gtaa-architecture/SKILL.md).

---

## Repository Structure

```
nobrand_framework/
├── Execution/                       # NUnit test fixtures
│   ├── RegressionTests/             # 23 regression test classes
│   └── GlobalSetup.cs               # Assembly-level ExtentReports + logging init
├── Definition/                      # Page objects + test data
│   ├── Pages/                       # 16 page-category folders
│   │   ├── HomePages/
│   │   ├── LandingPages/
│   │   ├── CommunityPages/
│   │   │   ├── US_CommunityPages/
│   │   │   └── CA_CommunityPages/
│   │   ├── ContactPages/, AboutPages/, NoBrandactionAndLiving/, ...
│   ├── Modules/                     # Shared UI components (Drawers, Modals)
│   ├── TestData/                    # FormData, UserData, ProductData DTOs
│   └── TestScriptMatrix/            # Paired .md test-case docs (one per fixture)
├── Adaptation/                      # External service layer (placeholders)
│   ├── Databases/, Integrations/, Protocols/, Services/
├── Utility/                         # Base infrastructure
│   ├── UtilityBase.cs               # Playwright browser/page lifecycle, env config, logging
│   ├── PageNavigation.cs            # GotoDomReadyAsync navigation extension
│   ├── TestConstants.cs             # Timeout values
│   └── ExtentManager.cs             # HTML report singleton
├── .agent/skills/                   # Skill files for AI assistance
├── .github/workflows/
│   └── nobrand_test_run.yml             # CI/CD pipeline
├── CLAUDE.md                        # Convention reference (canonical)
└── nobrand_framework.sln                  # Solution file
```

---

## Prerequisites

- **.NET 8 SDK** 
- **Playwright-managed Chromium** — installed once with the Playwright CLI after the first build (see [Getting Started](#getting-started)). No system Chrome or ChromeDriver version-matching is needed.
- **Git**
- *(Optional)* Visual Studio 2022, JetBrains Rider, or VS Code with the C# Dev Kit extension
- **Environment variables** for sensitive data 

---


## Configuration

Tests support **6 environment combinations**: `(dev | uat | prod) × (us | ca)`, configured in each test's `[SetUp]`:

```csharp
GetEnvironmentConfig("uat", "us");   // US UAT — default for most tests
GetEnvironmentConfig("uat", "ca");   // CA UAT
GetEnvironmentConfig("prod", "us");  // US Production
```

- **UAT** is where the suite normally runs. Invisible **reCAPTCHA is active in UAT** (a `recaptcha/api2` exchange precedes every `POST /api/contact-form`). Page objects never solve or execute a captcha; lead-form submits use a trusted-click-then-retry pattern (e.g. `SubmitAndAwaitLeadPost`).
- **Headless mode** is controlled by the `private const bool LocalHeadless = true;` in `Utility/UtilityBase.cs`, and is forced on whenever the environment variable `CI == "true"` (GitHub Actions sets this). There is **no `HEADLESS` environment variable**. To run **headed** locally, set `LocalHeadless = false` in `UtilityBase.cs` (a source change) and rebuild.

Configuration is code-driven via `Utility/UtilityBase.cs` — there is no `appsettings.json`.

---

## Running Tests Locally


Output locations after a run:
- **NUnit results:** `TestResults/*.trx`
- **Serilog logs:** `Logs/*.log` (or environment-specific subfolders: `DevLogs/`, `QaLogs/`, `StageLogs/`, `ProdLogs/`)
- **ExtentReports HTML:** `Reports/TestExecutionReport_<timestamp>.html`
- **Coverage:** `**/coverage.cobertura.xml`

---

## Test Inventory

23 regression fixtures, all under `Execution/RegressionTests/`. Each fixture has a paired markdown spec in `Definition/TestScriptMatrix/`.

| # | Category | Test Class | What it Verifies |
|---|---|---|---|
| 1 | Home Page | `HomePage_RegressionTest` | Home page loads with logo, nav, footer |
| 2 | Home Page | `HomePage_BodyContent_RegressionTest` | Home page body content elements + interactions |
| 3 | Header | `HeaderNavigation_RegressionTest` | Header main nav and dropdown items |
| 4 | Header | `BookATour_SiteHeader_LeadFormAndInformation_RegressionTest` | Header Book-A-Tour modal (form + info) |
| 5 | Footer | `FooterNavigation_RegressionTest` | Footer navigation links |
| 6 | Page Loading | `PageLoadingChecks_US_RegressionTest` | All US community pages load correctly |
| 7 | Page Loading | `PageLoadingChecks_CA_RegressionTest` | All CA community pages load correctly |
| 8 | Search | `CommunitySearch_RegressionTest` | Community search end-to-end from home |
| 9 | Search | `SiteSearch_RegressionTest` | Site search results validation |
| 10 | Search | `ResourceSearch_RegressionTest` | Resource search results validation |
| 11 | Community Page | `CommunityPage_NoBrandOfNoBrandLP_BodyContent_RegressionTest` | NoBrandLP buttons & links |
| 12 | Lead Form | `Home_LP_LeadFormA_RegressionTest` | Home landing-page lead form |
| 13 | Lead Form | `Family_LP_LeadFormA_RegressionTest` | Family landing-page lead form |
| 14 | Lead Form | `NoBrandEveryDay_LP_LeadFormA_RegressionTest` | "NoBrand Every Day" landing-page lead form |
| 15 | Lead Form | `WhatIsNoBrand type 1_NoBrandactionLiving_LeadFormA_RegressionTest` | "What is NoBrand type 1" lead form |
| 16 | Lead Form | `NoBrandCap_NoBrandlux_LeadFormA_RegressionTest` | The NoBrandCap (NoBrandlux) lead form |
| 17 | Lead Form | `NoBrandAp_NoBrandlux_LeadFormA_RegressionTest` | The NoBrandAp (NoBrandlux) lead form — variant A |
| 18 | Lead Form | `NoBrandAp_NoBrandlux_LeadFormB_RegressionTest` | The NoBrandAp (NoBrandlux) lead form — variant B |
| 19 | Lead Form | `NoBrandEFS_NoBrandlux_LeadFormA_RegressionTest` | NoBrandEFS (NoBrandlux) lead form — variant A |
| 20 | Lead Form | `NoBrandEFS_NoBrandlux_LeadFormB_RegressionTest` | NoBrandEFS (NoBrandlux) lead form — variant B |
| 21 | Lead Form | `NoBrandBCCourt_NoBrandBC_LeadFormA_RegressionTest` | NoBrandBC (NoBrandBC) lead form |
| 22 | Lead Form | `NoBrandOfNoBrandIS_NoBrandMan_LeadFormA_RegressionTest` | NoBrand of NoBrandIS (NoBrandMan) lead form — variant A |
| 23 | Lead Form | `NoBrandOfNoBrandIS_NoBrandMan_LeadFormB_RegressionTest` | NoBrand of NoBrandIS (NoBrandMan) lead form — variant B |

---

## CI/CD

Workflow: [.github/workflows/nobrand_test_run.yml](.github/workflows/nobrand_test_run.yml)

**Triggers:**
- Pull requests targeting `main`
- Daily schedule: **12:30 UTC** (06:30 CST)

**Runner:** `windows-latest` (.NET 8 set up via `actions/setup-dotnet@v4`; Playwright installs its own Chromium)

**Pipeline steps:**
1. Checkout repo
2. Set up .NET 8
3. Cache NuGet packages
4. `dotnet restore` → `dotnet build` (Release)
5. Cache Playwright browsers (`~/AppData/Local/ms-playwright`)
6. Install the Playwright Chromium (`pwsh .../playwright.ps1 install chromium`)
7. `dotnet test` with TRX output and XPlat coverage
8. Parse TRX into a markdown **GitHub Actions Job Summary** (pass/fail counts + failed-test names)
9. Publish results via [`dorny/test-reporter`](https://github.com/dorny/test-reporter)
10. Generate HTML coverage report with `dotnet-reportgenerator-globaltool`
11. Append a code-coverage summary (line + branch %) to the Job Summary

**Artifacts (retained 30 days):**
- `test-results-<timestamp>` — TRX results
- `serilog-test-logs-<timestamp>` — structured test logs
- `extent-report-<timestamp>` — ExtentReports HTML
- `code-coverage-report` — Cobertura XML
- `coverage-report-html-<timestamp>` — ReportGenerator HTML

---

## Reporting & Logs

- **Serilog** writes structured logs per test to `Logs/` (or environment-specific folders: `DevLogs/`, `QaLogs/`, `StageLogs/`, `ProdLogs/`). Both console and file sinks are enabled.
- **ExtentReports** produces a single timestamped HTML report at `Reports/TestExecutionReport_<timestamp>.html`. Failed tests include embedded screenshots captured automatically during `UtilityBase.TestCleanupAsync()`.
- **Code coverage** is collected via coverlet (`--collect:"XPlat Code Coverage"`) and converted to HTML by ReportGenerator in CI.

---

## Conventions

- **Page objects:** use a primary constructor — `public class FooPage(IPage page, ILogger logger)`. They receive an `IPage`, never an `IWebDriver`.
- **Locators:** every locator is a `private ILocator X => page.Locator(...);` property at the top of the page object, grouped under `//-- Section --//` comment headers. Prefer CSS / `page.GetByRole(...)` / `GetByLabel` / `GetByText`; use `page.Locator("xpath=...")` only for complex XPath. Append `.First` when a selector may match multiple elements (avoids strict-mode violations). **Methods consume the named `ILocator` property and never inline a selector.**
- **Naming:** `[Description]_[ElementType]` for locators; `[Action]_[Goal]` for methods.
- **Method return types:** verifications return `async Task<bool>`, actions return `async Task`, navigations return `async Task<NextPage>` (returning `new NextPage(page, logger)`).
- **Waits:** Playwright **auto-waits** — `ClickAsync` / `FillAsync` and friends wait for visible + stable + enabled. Navigations use `await page.GotoDomReadyAsync(url)` (waits for DOMContentLoaded; extension in `Utility/PageNavigation.cs`). Verify with web-first assertions via `using static Microsoft.Playwright.Assertions;`: `await Expect(locator).ToBeVisibleAsync()`, `ToHaveTextAsync`, `ToHaveURLAsync`, `Not.ToHaveCountAsync(0)`, `ToHaveClassAsync(new Regex("active"))`. **No `WebDriverWait` / `ExpectedConditions` / `Thread.Sleep()` / `IJavaScriptExecutor` / `scrollIntoView` / `document.readyState`.**
- **Text entry:** `await X.FillAsync(v)` (clears + types); a keystroke typeahead (e.g. a ZIP lookup) needs `await X.PressSequentiallyAsync(v); await X.BlurAsync();`; a `<select>` uses `await X.SelectOptionAsync(new SelectOptionValue { Label = t });`.
- **Timeouts:** `TestConstants.DefaultWaitSeconds` (30s), `TestConstants.SubmitWaitSeconds` (60s), `TestConstants.ShortWaitSeconds` (10s) are **int seconds** — multiply by 1000 for Playwright millisecond options.
- **Tests:** inherit `UtilityBase`, call `GetEnvironmentConfig(env, locale)` in a synchronous `[SetUp]`, construct page objects via `new(await GetPageAsync(), testLogger)`, and `await TestCleanupAsync()` in an async `[TearDown]`.

---

## Sensitive Data

> 🔒 **Never hardcode credentials or real email addresses in source.**

- Test-user credentials are loaded from environment variables:
  - `TEST_USER1_EMAIL`
  - `TEST_USER1_PASSWORD`
- Form test data uses placeholder domains (e.g. `autotest@example.com`) — never real customer data.
- Invisible reCAPTCHA is **active** in UAT and production. Page objects never solve or execute the captcha; lead-form submits use the trusted-click-then-retry pattern (e.g. `SubmitAndAwaitLeadPost`) and wait on the resulting `POST /api/contact-form`.
- Configure your shell or IDE run-config to set the env vars locally; in CI they are provided via GitHub Actions secrets.

---

## Troubleshooting

| Symptom | Likely Cause / Fix |
|---|---|
| `Executable doesn't exist` / browser not installed | The Playwright-managed Chromium hasn't been installed. Run `pwsh nobrand_Execution/bin/Debug/net8.0/playwright.ps1 install chromium` (use `bin/Release/...` for Release builds). See [Getting Started](#getting-started). |
| Strict-mode violation: *"locator resolved to N elements"* | A selector matched more than one element. Append `.First` to the `ILocator` (or tighten the selector).  |
| Tests pass locally, fail in CI | Headless behavior differs. Reproduce locally by setting `LocalHeadless = true` (the CI value) in `Utility/UtilityBase.cs`. Check the `serilog-test-logs-<timestamp>` and `extent-report-<timestamp>` artifacts for failure context and screenshots. |
| Navigation hangs / `load` event never fires | Use `await page.GotoDomReadyAsync(url)` (waits for **DOMContentLoaded**, not `load`) and follow it with an auto-waiting action or `Expect` assertion. The site's third-party stack (consent manager, reCAPTCHA, embeds) can keep the `load` event from firing. |
| `NullReferenceException` from credential reads | `TEST_USER1_EMAIL` / `TEST_USER1_PASSWORD` env vars not set. See [Sensitive Data](#sensitive-data). |
| Tests time out on form submission | Increase to `TestConstants.SubmitWaitSeconds` (60s) and confirm **UAT** is targeted. Invisible reCAPTCHA is active in UAT, so submits use the trusted-click-then-retry pattern (e.g. `SubmitAndAwaitLeadPost`) that waits on `POST /api/contact-form`. |

