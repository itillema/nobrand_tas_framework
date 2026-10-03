using AventStack.ExtentReports;
using Microsoft.Playwright;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using Serilog;
using Serilog.Events;
using System.Reflection;

namespace Utility
{
    public class UtilityBase
    {
        // Playwright stack. The browser/context/page are launched lazily on the first GetPageAsync() call and disposed in TestCleanupAsync(). Page objects take the IPage; tests do:
        //   var page = await GetPageAsync();  SomePage x = new(page, testLogger);
        private IPlaywright? _playwright;
        private IBrowser? _browser;
        private IBrowserContext? _context;
        private IPage? _page;

        // Toggle to run tests headless locally from Test Explorer. CI always runs headless.
        private const bool LocalHeadless = true;

        private readonly bool headless = LocalHeadless || Environment.GetEnvironmentVariable("CI") == "true";
        private ExtentTest? extentTest;
        public string BaseUrl { get; private set; }
        public ILogger testLogger { get; private set; }

        // DO NOT ATTEMPT TO ACCESS THESE DOMAINS, THEY ARE FAKE.
        private static readonly Dictionary<(string env, string locale), string> EnvironmentUrls = new()
        {
            { ("dev",  "us"), "https://dev.nobrand.com/"    },
            { ("uat",  "us"), "https://uat.nobrand.com/"    },
            { ("prod", "us"), "https://www.nobrand.com/"    },
            { ("dev",  "ca"), "https://dev.ca.nobrand.com/" },
            { ("uat",  "ca"), "https://uat.ca.nobrand.com/" },
            { ("prod", "ca"), "https://www.nobrand.ca/"     },
        };

        // Static constructor for ONE-TIME logger configuration
        static UtilityBase()
        {
            var logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
            var logFilePath = Path.Combine(logDirectory, $"TestRun_{DateTime.Now:yyyyMMdd_HHmmss}.log");

            // Ensure the directory exists
            Directory.CreateDirectory(logDirectory);

            // Configure the static Serilog.Log instance
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
                .WriteTo.File(logFilePath,
                              outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            Log.Information("==================================================");
            Log.Information("Global Logger Initialized. Starting Test Run...");
            Log.Information("==================================================");

            // Hook to flush logs when the test run exits
            AppDomain.CurrentDomain.ProcessExit += (s, e) =>
            {
                Log.Information("Test Run Finished. Flushing and Closing Logger...");
                Log.CloseAndFlush();
            };
        }

        /// <summary>
        ///     Returns the Playwright page, launching the browser/context lazily on first use and navigating to <see cref="BaseUrl"/>.
        /// </summary>
        public async Task<IPage> GetPageAsync()
        {
            if (_page is null)
            {
                _playwright = await Playwright.CreateAsync();
                _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
                    Headless = headless,
                    Args = headless
                        ? new[] { "--no-sandbNoBrand", "--disable-dev-shm-usage", "--disable-gpu" }
                        : null
                });
                _context = await _browser.NewContextAsync(new BrowserNewContextOptions
                {
                    ViewportSize = new ViewportSize { Width = 1920, Height = 1080 },
                    // Lets page objects navigate with relative paths (e.g. GotoDomReadyAsync("/NoBrandaction-questionnaire")), mirroring how the suite builds URLs from BaseUrl + a relative resource path.
                    BaseURL = BaseUrl
                });
                _page = await _context.NewPageAsync();
                await _page.GotoDomReadyAsync(BaseUrl);
                testLogger.Information("Playwright page configured. Navigating to: {Url}", BaseUrl);
            }
            return _page;
        }

        public void GetEnvironmentConfig(string testEnv, string locale)
        {
            // Initialize test-specific logging instance and log test start
            testLogger = Log.ForContext(this.GetType());
            testLogger.Information("--- Test Started: {TestName} ---", TestContext.CurrentContext.Test.Name);

            // Create ExtentTest node so even setup failures are captured in the report
            var testName = TestContext.CurrentContext.Test.Name;
            var description = TestContext.CurrentContext.Test.Properties.Get("Description")?.ToString()
                ?? testName;
            try
            {
                extentTest = ExtentManager.Extent.CreateTest(testName, description);
                testLogger.Debug("ExtentTest node created for: {TestName}", testName);
            }
            catch (InvalidOperationException ex)
            {
                testLogger.Warning(ex, "ExtentReports not initialized; skipping report node creation.");
            }

            AssignReportCategories(extentTest, testEnv, locale);

            // Set the environment you want to test here: dev, uat, or prod
            // If you want to use the same environment for all test cases, leave the variable string empty in test methods and choose the environment you want to test with below
            string testEnvironment;
            string testStore;

            if (testEnv != string.Empty)
            {
                // Chosen test environment for specific test methods
                testEnvironment = testEnv;
            }
            else
            {
                // Global environment config
                testEnvironment = "uat";
            }

            if (locale != string.Empty)
            {
                // Chosen test locale for specific test methods
                testStore = locale;
            }
            else
            {
                // Global locale config
                testStore = "us";
            }

            testLogger.Information("Test environment set to: {Env}, Locale: {Locale}, Headless: {Headless}", testEnvironment, testStore, headless);

            // Resolve the base URL only — the browser is launched lazily by GetPageAsync().
            ResolveBaseUrl(testEnvironment, testStore);
        }

        private void ResolveBaseUrl(string testEnvironment, string testLocale)
        {
            if (!EnvironmentUrls.TryGetValue((testEnvironment, testLocale), out string? url))
            {
                testLogger.Warning("Environment '{Env}'/Locale '{Locale}' not recognized. Defaulting to US UAT.", testEnvironment, testLocale);
                url = EnvironmentUrls[("uat", "us")];
            }

            BaseUrl = url;
            testLogger.Information("Base URL resolved: {Url}", BaseUrl);
        }

        /// <summary>
        ///     Per-test teardown: records the outcome (ExtentReports + JSON run summary), captures a failure screenshot, and disposes the Playwright browser/context. Async because Playwright teardown is async (NUnit supports an async [TearDown]).
        /// </summary>
        public async Task TestCleanupAsync()
        {
            string? base64 = null;
            string? screenshotPath = null;
            if (IsFailed() && _page is not null)
                (base64, screenshotPath) = await CapturePlaywrightScreenshotAsync(_page);

            FinalizeReport(base64, screenshotPath);

            if (_context is not null) await _context.CloseAsync();
            if (_browser is not null) await _browser.CloseAsync();
            _playwright?.Dispose();
            _page = null;
            _context = null;
            _browser = null;
            _playwright = null;

            testLogger.Information("--- Test Finished: {TestName} ---", TestContext.CurrentContext.Test.Name);
        }

        /// <summary>
        ///     Captures the just-completed test's outcome into the ExtentReports node and the
        ///     structured JSON run summary.
        /// </summary>
        private void FinalizeReport(string? base64Screenshot, string? screenshotPath)
        {
            if (extentTest is null) return;

            var outcome = TestContext.CurrentContext.Result.Outcome.Status;
            var message = TestContext.CurrentContext.Result.Message;

            switch (outcome)
            {
                case TestStatus.Passed:
                    extentTest.Pass("Test passed.");
                    break;

                case TestStatus.Failed:
                    extentTest.Fail($"Test failed: {message}");
                    if (base64Screenshot is not null)
                    {
                        var testName = TestContext.CurrentContext.Test.Name;
                        extentTest.AddScreenCaptureFromBase64String(base64Screenshot, $"Failure screenshot: {testName}");
                    }
                    break;

                case TestStatus.Skipped:
                    extentTest.Skip($"Test skipped: {message}");
                    break;

                case TestStatus.Inconclusive:
                    extentTest.Warning($"Test inconclusive: {message}");
                    break;

                default:
                    extentTest.Info($"Test outcome: {outcome}");
                    break;
            }
        }

        private static bool IsFailed() =>
            TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed;

        private void AssignReportCategories(ExtentTest? test, string testEnv, string locale)
        {
            if (test is null) return;

            var env = string.IsNullOrEmpty(testEnv) ? "uat" : testEnv;
            var loc = string.IsNullOrEmpty(locale) ? "us" : locale;

            var assigned = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            void AddCategory(string? name)
            {
                if (!string.IsNullOrWhiteSpace(name) && assigned.Add(name))
                {
                    test.AssignCategory(name);
                }
            }

            AddCategory(env.ToUpperInvariant());
            AddCategory(loc.ToUpperInvariant());

            // Class-level [Category] attributes aren't in TestContext.Test.Properties — NUnit attaches them to the parent TestFixture's bag, not the leaf test's. Read them directly from the runtime test class via reflection.
            foreach (var attr in this.GetType().GetCustomAttributes<CategoryAttribute>(inherit: true))
            {
                AddCategory(attr.Name);
            }

            foreach (var category in TestContext.CurrentContext.Test.Properties["Category"])
            {
                AddCategory(category?.ToString());
            }
        }

        /// <summary>Builds the on-disk + Reports-relative screenshot path for the current test.</summary>
        private static (string filePath, string relativePath) BuildScreenshotPaths()
        {
            var screenshotDirectory = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory, "Reports", "Screenshots");
            Directory.CreateDirectory(screenshotDirectory);

            var testName = TestContext.CurrentContext.Test.Name;
            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var fileName = $"{testName}_{timestamp}.png";

            return (Path.Combine(screenshotDirectory, fileName), $"Screenshots/{fileName}");
        }

        private async Task<(string? base64, string? path)> CapturePlaywrightScreenshotAsync(IPage page)
        {
            try
            {
                var (filePath, relativePath) = BuildScreenshotPaths();
                byte[] bytes = await page.ScreenshotAsync(new PageScreenshotOptions
                {
                    Path = filePath,
                    FullPage = true
                });
                testLogger.Information("Screenshot saved: {Path}", filePath);
                return (Convert.ToBase64String(bytes), relativePath);
            }
            catch (Exception ex)
            {
                testLogger.Warning(ex, "Failed to capture Playwright screenshot for report.");
                return (null, null);
            }
        }
    }
}

