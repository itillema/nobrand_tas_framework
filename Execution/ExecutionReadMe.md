# Execution Layer - Architecture & Style Guide

## Overview
The **Execution Layer** contains the executable test classes. It orchestrates the tests by calling actions from the **Definition Layer** (Page Objects), utilizing helpers from the **Utility Layer**, and asserting expected outcomes.

## Directory Structure

| Directory | Purpose |
| :--- | :--- |
| **SmokeTests** | High-level critical path tests to verify core functionality. |
| **RegressionTests** | Comprehensive test suite for verifiable features. |
| **FunctionalTests** | Targeted tests for specific functional areas. |
| **EndToEndTests** | Integration scenarios covering complete user journeys. |
| **POCTests** | Experimenting with new testing procedures |

## Test Class Style Guide

### 1. Class Setup
*   **Inheritance**: MUST inherit from `UtilityBase` to gain access to Playwright page management (`GetPageAsync()`) and standard setup/teardown.
*   **Attributes**: Decorate with `[TestFixture]`.
*   **Naming**: `[Feature]_[TestType]` (e.g., `CommunitySearch_SmokeTest`).

```csharp
namespace Execution.SmokeTests
{
    [TestFixture]
    public class CommunitySearch_SmokeTest : UtilityBase
    {
        // ...
    }
}
```

### 2. Setup & Teardown
*   **Setup**: Use a synchronous `[SetUp]` method to initialize the environment.
    *   Call `GetEnvironmentConfig(env, locale)` to set the test context. This only resolves `BaseUrl`; the Playwright browser launches lazily on the first `GetPageAsync()` call.
*   **Teardown**: Use an async `[TearDown]` method to ensure clean exit.
    *   `await TestCleanupAsync()` (from `UtilityBase`) — records the outcome, captures a failure screenshot, and disposes the Playwright browser/context.

```csharp
[SetUp]
public void Setup()
{
    // Leave arguments empty for global default (UAT/US)
    GetEnvironmentConfig("uat", "us");
}

[TearDown]
public async Task TearDown()
{
    await TestCleanupAsync();
}
```

### 3. Test Methods
*   **Signature**: `[Test] public async Task ...` — test bodies are async and `await` every page-object call.
*   **Attributes**: `[Test]`, `[Description("...")]`.
*   **Naming**: `[Page/Feature]_[Scenario]`.
*   **Structure**:
    1.  **Arrange**: Initialize Page Objects via `new(await GetPageAsync(), testLogger)`.
    2.  **Act**: `await` Page Object methods to perform actions.
    3.  **Assert**: use `Assert.That(await ...)` to verify results. For multiple awaited asserts use `await Assert.MultipleAsync(async () => { Assert.That(await ...); ... });`.
    4.  **Log**: Use `testLogger.Information("PASSED: ...")` for clear reporting.

```csharp
[Test, Description("Verify search functionality")]
public async Task HomePage_SearchForCommunity()
{
    // Arrange — GetPageAsync() lazily launches Chromium and returns the IPage
    HomePage homePage = new(await GetPageAsync(), testLogger);

    // Act
    await homePage.LoadHomePage();
    bool isLoaded = await homePage.VerifyLogoVisible();

    // Assert
    Assert.That(isLoaded, Is.True, "Home page logo was not visible.");
    testLogger.Information("PASSED: Home page loaded successfully.");
}
```


