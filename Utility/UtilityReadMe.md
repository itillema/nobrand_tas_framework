# Utility Layer - Architecture & Style Guide

## Overview
The **Utility Layer** provides cross-cutting concerns, shared infrastructure, and helper functions used across the entire framework. It handles the "plumbing" of the test solution, such as Playwright browser/page management, logging configuration, and reporting.

## Directory Structure

| Directory | Purpose |
| :--- | :--- |
| **Logging** | Configuration and wrappers for logging libraries (e.g., Serilog). |
| **Reporting** | Configuration for test reporting engines (e.g., ExtentReports). |
| **(Root)** | Contains the core `UtilityBase` class. |

## Base Class Patterns (`UtilityBase`)

Most test classes will inherit from `UtilityBase`. This class centralizes the complexity of managing the test lifecycle.

### Key Responsibilities
1.  **Playwright Browser/Page Management**:
    *   `GetPageAsync()`: Lazily launches the Playwright-managed Chromium (headless when `CI=true`) and a 1920x1080 browser context on first use, navigates to `BaseUrl`, and returns the `IPage`. Page objects receive this `IPage`.
    *   `GetEnvironmentConfig(env, locale)`: Synchronous — resolves `BaseUrl` from the 6 env x locale combinations in `EnvironmentUrls`. It does **not** launch the browser; the browser launches lazily on the first `GetPageAsync()` call.
2.  **Logging Initialization**:
    *   Static Constructor: Initializes the global `Log.Logger` once per test run.
    *   `GetEnvironmentConfig`: Initializes a context-specific `testLogger` for the current test method.
3.  **Test Lifecycle**:
    *   `TestCleanupAsync()`: The single async teardown — records the outcome into ExtentReports, captures a failure screenshot via `page.ScreenshotAsync`, disposes the Playwright browser/context, and logs the test finish event.

### Extension Guidelines
When adding new utilities:
*   **Stateless Helpers**: If a helper doesn't need to hold state, make it a `static` class/method in the root or a dedicated folder.
*   **Stateful Helpers**: If it requires `IPage` access, consider making it an extension method on `IPage` or a component in the `Adaptation` layer.
*   **Do not pollute Global Namespace**: Ensure all utilities are properly namespaced (e.g., `Utility.Helpers`).

### Usage Example
```csharp
// In a Test Class
public class MyTest : UtilityBase
{
    [SetUp]
    public void Init()
    {
        // Resolves BaseUrl & initializes Logger via UtilityBase (synchronous).
        GetEnvironmentConfig("uat", "us");
    }

    [Test]
    public async Task SomeScenario()
    {
        // GetPageAsync() lazily launches Chromium and returns the IPage.
        SomePage page = new(await GetPageAsync(), testLogger);
        // ...
    }

    [TearDown]
    public async Task Cleanup()
    {
        // Reporting + Playwright browser/context disposal.
        await TestCleanupAsync();
    }
}
```

