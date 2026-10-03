using Definition.Pages.CommunityPages.US_CommunityPages;
using Definition.TestData.FormData;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using NUnit.Framework;
using Utility;

namespace Execution.RegressionTests.LeadFormsTests
{
    [TestFixture]
    [Category("Regression")]
    [Category("LeadForm")]
    public class NoBrandCap_NoBrandlux_LeadFormA_RegressionTest : UtilityBase
    {
        [SetUp]
        public void Setup()
        {
            // Configure test environment for UAT, US locale
            GetEnvironmentConfig("uat", "us");
        }

        [Test, Description("Verify NoBrandCap NoBrandlux Lead Form A submits successfully and a Thank You confirmation replaces the form in-page.")]
        public async Task NoBrandCapLeadFormA_Submit_ReplacedWithThankYou()
        {
            // ARRANGE — create form data and page object (Playwright)
            var formData = new CP_TheNoBrandCap_FormData();
            CommunityPage_TheNoBrandCap NoBrandCapPage = new(await GetPageAsync(), testLogger);

            // ACT — navigate to community page
            await NoBrandCapPage.NavigateToNoBrandCapPage();
            testLogger.Information("STEP: Navigated to The NoBrandCap community page.");

            // Accept cookie consent banner
            await NoBrandCapPage.AcceptCookies();
            testLogger.Information("STEP: Accepted cookies.");

            // Scroll to lead form at bottom of page
            await NoBrandCapPage.ScrollToLeadForm();
            testLogger.Information("STEP: Scrolled to lead form.");

            // Fill all lead form fields
            await NoBrandCapPage.FillLeadForm(formData);
            testLogger.Information("STEP: Filled lead form with test data.");

            // Submit form
            await NoBrandCapPage.SubmitLeadForm();
            testLogger.Information("STEP: Submitted lead form.");

            // ASSERT — verify in-page thank-you confirmation is visible
            Assert.That(await NoBrandCapPage.VerifyThankYouConfirmation(), Is.True,
                "The in-page thank-you confirmation did not appear after form submission.");
            testLogger.Information("PASSED: In-page thank-you confirmation verified successfully.");
        }

        [TearDown]
        public async Task TearDown()
        {
            await TestCleanupAsync();
        }
    }
}

