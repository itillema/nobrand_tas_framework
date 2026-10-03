using System.Diagnostics;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;

namespace Utility
{
    /// <summary>
    ///     Manages the singleton ExtentReports instance for HTML test reporting.
    ///     Initialized once per test run from the assembly-level [SetUpFixture] in the Execution project.
    /// </summary>
    public static class ExtentManager
    {
        private static ExtentReports? _extent;
        private static readonly object _lock = new();

        /// <summary>The shared ExtentReports instance. Null until Init() is called.</summary>
        public static ExtentReports Extent
        {
            get
            {
                if (_extent is null)
                    throw new InvalidOperationException(
                        "ExtentManager has not been initialized. Call ExtentManager.Init() from a [SetUpFixture].");
                return _extent;
            }
        }

        /// <summary>
        ///     Initializes the ExtentReports instance and attaches the Spark HTML reporter.
        ///     Call once from the assembly-level [OneTimeSetUp].
        /// </summary>
        public static void Init()
        {
            lock (_lock)
            {
                if (_extent is not null) return;

                var reportDirectory = Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory, "Reports");
                Directory.CreateDirectory(reportDirectory);

                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                var reportPath = Path.Combine(reportDirectory,
                    $"TestExecutionReport_{timestamp}.html");

                var htmlReporter = new ExtentSparkReporter(reportPath);
                htmlReporter.Config.DocumentTitle = "NoBrand - Test Report";
                htmlReporter.Config.ReportName = "Automated Test Execution Report";

                _extent = new ExtentReports();
                _extent.AttachReporter(htmlReporter);

                AddRunMetadata(_extent);
            }
        }

        /// <summary>
        ///     Flushes all logs to the HTML report file.
        ///     Call once from the assembly-level [OneTimeTearDown].
        /// </summary>
        public static void Flush()
        {
            _extent?.Flush();
        }

        private static void AddRunMetadata(ExtentReports extent)
        {
            extent.AddSystemInfo("Browser", "Chrome");
            extent.AddSystemInfo("Chrome Version", GetChromeVersion());
            extent.AddSystemInfo("OS", Environment.OSVersion.ToString());
            extent.AddSystemInfo("CI", Environment.GetEnvironmentVariable("CI") ?? "false");
            extent.AddSystemInfo("Test Environment",
                Environment.GetEnvironmentVariable("TEST_ENV") ?? "uat");

            var serverUrl = Environment.GetEnvironmentVariable("GITHUB_SERVER_URL");
            var repository = Environment.GetEnvironmentVariable("GITHUB_REPOSITORY");
            var runId = Environment.GetEnvironmentVariable("GITHUB_RUN_ID");
            if (!string.IsNullOrEmpty(serverUrl) &&
                !string.IsNullOrEmpty(repository) &&
                !string.IsNullOrEmpty(runId))
            {
                extent.AddSystemInfo("GitHub Run",
                    $"{serverUrl}/{repository}/actions/runs/{runId}");
            }

            var sha = Environment.GetEnvironmentVariable("GITHUB_SHA");
            if (!string.IsNullOrEmpty(sha))
            {
                extent.AddSystemInfo("Commit SHA", sha.Length >= 7 ? sha[..7] : sha);
            }

            var refName = Environment.GetEnvironmentVariable("GITHUB_REF_NAME");
            if (!string.IsNullOrEmpty(refName))
            {
                extent.AddSystemInfo("Branch", refName);
            }
        }

        private static string GetChromeVersion()
        {
            string[] candidatePaths =
            {
                @"C:\Program Files\Google\Chrome\Application\chrome.exe",
                @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe"
            };

            foreach (var path in candidatePaths)
            {
                if (File.Exists(path))
                {
                    var version = FileVersionInfo.GetVersionInfo(path).FileVersion;
                    if (!string.IsNullOrEmpty(version)) return version;
                }
            }

            return "Unknown";
        }
    }
}

