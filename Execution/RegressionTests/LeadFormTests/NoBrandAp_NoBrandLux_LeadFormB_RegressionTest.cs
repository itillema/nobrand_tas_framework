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
    public class NoBrandAp_NoBrandlux_LeadFormB_RegressionTest : UtilityBase
    {
        [SetUp]
        public void Setup()
        {
            // Configure test environment for UAT, US locale
            GetEnvironmentConfig("uat", "us");
        }

        [Test, Description("Verify the NoBrandAp Inquire NoBrandlux community page lead form submits successfully and a Thank You confirmation replaces the form in-page.")]
        public async Task NoBrandApNoBrandluxLeadFormB_Submit_ReplacedWithThankYou()
        {
            // Create form data and page object
            var formData = new CP_TheNoBrandAp_FormData();
            CommunityPage_TheNoBrandAp NoBrandApPage = new(await GetPageAsync(), testLogger);

            // Navigate to the NoBrandAp Inquire sub-page
            await NoBrandApPage.NavigateToNoBrandApInquirePage();
            testLogger.Information("STEP: Navigated to The NoBrandAp Inquire page.");

            // Accept cookie consent banner
            await NoBrandApPage.AcceptCookies();
            testLogger.Information("STEP: Accepted cookies.");

            // Scroll to the 'Discover The NoBrandAp' lead form near the bottom of the page
            await NoBrandApPage.ScrollToInquireLeadForm();
            testLogger.Information("STEP: Scrolled to the 'Discover The NoBrandAp' lead form.");

            // Fill all lead form fields and check both checkbNoBrandes (Founders Club + marketing consent)
            await NoBrandApPage.FillInquireLeadForm(formData);
            testLogger.Information("STEP: Filled Inquire lead form with test data.");

            // Submit form
            await NoBrandApPage.SubmitInquireLeadForm();
            testLogger.Information("STEP: Submitted Inquire lead form.");

            // Verify in-page thank-you confirmation is visible
            Assert.That(await NoBrandApPage.VerifyInquireThankYouConfirmation(), Is.True,
                "The in-page 'Thank You!' confirmation did not appear after Form B submission.");
            testLogger.Information("PASSED: In-page thank-you confirmation verified successfully.");
        }

        [TearDown]
        public async Task TearDown()
        {
            await TestCleanupAsync();
        }
    }
}

