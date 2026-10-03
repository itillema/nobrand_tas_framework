using Serilog;
using Utility;

namespace Execution
{
    [SetUpFixture]
    public class GlobalSetup
    {
        [OneTimeSetUp]
        public void RunBeforeAllTests()
        {
            ExtentManager.Init();
            Log.Information("ExtentReports initialized for this test run.");
        }

        [OneTimeTearDown]
        public void RunAfterAllTests()
        {
            ExtentManager.Flush();
            Log.Information("ExtentReports flushed. HTML report generated.");
        }
    }
}

