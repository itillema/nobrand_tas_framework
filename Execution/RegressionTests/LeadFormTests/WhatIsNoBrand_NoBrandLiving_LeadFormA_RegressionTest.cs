using Utility;

namespace Execution.RegressionTests.LeadFormsTests
{
    [TestFixture]
    [Category("Regression")]
    [Category("LeadForm")]
    public class WhatIsNoBrand_NoBrandactionLiving_LeadFormA_RegressionTest : UtilityBase
    {
        [SetUp]
        public void Setup()
        {
            // Configure test environment for UAT, US locale
            GetEnvironmentConfig("uat", "us");
        }

        [Test, Description("Verify users can navigate to the What Is NoBrand type 1 page and submit Lead Form A successfully.")]
        public async Task LeadFormA_Submit_RedirectsToThankYou()
        {
            // Create test data and page object
            var formData = new LP_LeadFormA_FormData();
            WhatIsNoBrandPage page = new(await GetPageAsync(), testLogger);

            // Navigate to landing page
            await page.NavigateToLandingPage();
            testLogger.Information("PASSED: Navigated to What Is NoBrand type 1 page.");

            // Accept cookies
            await page.AcceptCookies();
            testLogger.Information("PASSED: Handled cookie banner.");

            // Scroll to lead form
            await page.ScrollToLeadForm();
            testLogger.Information("PASSED: Scrolled to lead form.");

            // Fill lead form with test data
            await page.FillLeadForm(formData);
            testLogger.Information("PASSED: Filled lead form with test data.");

            // Submit form
            await page.SubmitLeadForm();
            testLogger.Information("PASSED: Submitted lead form.");

            // Verify Thank You page
            Assert.That(await page.VerifyThankYouPage(), Is.True,
                "Thank You page did not load or heading is not visible.");
            testLogger.Information("PASSED: Thank You page verified successfully.");
        }

        [TearDown]
        public async Task TearDown()
        {
            await TestCleanupAsync();
        }
    }
}

