# Definition Layer - Architecture & Style Guide

## Overview
The **Definition Layer** contains the core modeling of the application under test. It primarily consists of **Page Objects** (representing UI pages) and **Test Data** (data models and static data). This layer abstracts the UI mechanics from the test logic.

## Directory Structure

| Directory | Purpose |
| :--- | :--- |
| **Modules** | Reusable UI components that appear across multiple pages (e.g., Headers, Footers, Search widgets). |
| **Pages** | Page Object classes, typically organized by functional area (e.g., `HomePages`, `CommunityPages`). |
| **TestData** | Data models (DTOs) and static test data files (JSON/XML) used to drive tests. |

## Page Object Style Guide

### 1. Class Definitions
*   **Namespace**: `Definition.Pages.[FunctionalArea]`.
*   **Inheritance**: Typically standalone, or inherits from a specific `BasePage` if shared logic exists.
*   **Constructor**: Use Primary Constructors to inject dependencies (`IPage`, `ILogger`). Page objects receive a Playwright `IPage` — never an `IWebDriver`. This suite was migrated from Selenium to Playwright; there is no WebDriver/ChromeDriver anywhere in the repo.

```csharp
public class HomePage(IPage page, ILogger logger)
{
    private const string PageUrl = "https://...";
    // ...
}
```

### 2. Locators (Properties)
*   **Visibility**: `private`. Page internals should not be exposed.
*   **Type**: `ILocator`.
*   **Naming Convention**: `[Description]_[ElementType]` (e.g., `Submit_Button`, `UserName_Input`, `GlobalNav_Link`).
*   **Syntax**: Use expression-bodied properties with `page.Locator(...)`. Prefer CSS / `page.GetByRole(...)` / `GetByLabel` / `GetByText`; use `page.Locator("xpath=...")` only for complex XPath. Append `.First` when a selector may match multiple elements (avoids Playwright strict-mode violations).
*   **Grouping**: Group locators by page section (e.g., `//-- Header Elements --//`).
*   **Consumption**: Methods consume the named `ILocator` property — they never inline a selector.

```csharp
//-- Login Section Elements --//
private ILocator UserName_Input => page.Locator("#username");
private ILocator Login_Button => page.GetByRole(AriaRole.Button, new() { Name = "Log In" });
```

### 3. Methods (Actions & Validations)
*   **Visibility**: `public`. These form the API used by the Execution layer.
*   **Naming**: `[Action]_[Goal/Verification]` (e.g., `LoadHomePage_VerifyLogoVisible`, `SubmitContactForm`).
*   **Return Types**:
    *   **Actions**: `async Task` (or `async Task<NextPage>` returning `new NextPage(page, logger)` when navigation occurs).
    *   **Verifications**: `async Task<bool>` for assertions.
*   **Logging**: Log significant steps using the injected `logger`.
*   **Waits**: Rely on Playwright **auto-wait** — actions (`ClickAsync`, `FillAsync`, etc.) wait for the element to be visible + stable + enabled. Navigations use `await page.GotoDomReadyAsync(url)` (extension in `Utility/PageNavigation.cs`, waits for DOMContentLoaded). Verify with web-first assertions via `using static Microsoft.Playwright.Assertions;`. There is **no** `WebDriverWait` / `ExpectedConditions` / `Thread.Sleep` / `IJavaScriptExecutor` / `scrollIntoView` / `document.readyState`.
*   **Text entry**: `await X.FillAsync(v)` clears + types; a keystroke typeahead (e.g. a ZIP lookup) needs `await X.PressSequentiallyAsync(v); await X.BlurAsync();`; a `<select>` uses `await X.SelectOptionAsync(new SelectOptionValue { Label = t });`.

```csharp
public async Task<bool> VerifyWelcomeMessage()
{
    logger.Information("Verifying welcome message...");
    try
    {
        await Expect(WelcomeMessage_Text).ToBeVisibleAsync();
        return true;
    }
    catch (PlaywrightException)
    {
        return false;
    }
}
```


