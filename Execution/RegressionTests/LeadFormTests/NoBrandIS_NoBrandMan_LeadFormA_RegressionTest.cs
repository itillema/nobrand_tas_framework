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
    public class NoBrandOfNoBrandIS_NoBrandMan_LeadFormA_RegressionTest : UtilityBase
    {
        [SetUp]
        public void Setup()
        {
            // Configure test environment for UAT, US locale
            GetEnvironmentConfig("uat", "us");
        }

        [Test, Description("Verify the inline NoBrand of NoBrandIS community page lead form submits successfully and a Thank You confirmation replaces the form in-page.")]
        public async Task NoBrandOfNoBrandISLeadFormA_Submit_ReplacedWithThankYou()
        {
            // ARRANGE — create form data and page object (Playwright)
            var formData = new CP_NoBrandOfNoBrandIS_FormData();
            CommunityPage_NoBrandOfNoBrandIS NoBrandISPage = new(await GetPageAsync(), testLogger);

            // ACT — navigate to community page
            await NoBrandISPage.NavigateToNoBrandISPage();
            testLogger.Information("STEP: Navigated to the NoBrand of NoBrandIS community page.");

            // Accept cookie consent banner
            await NoBrandISPage.AcceptCookies();
            testLogger.Information("STEP: Accepted cookies.");

            // Scroll to inline 'Get in Touch' lead form
            await NoBrandISPage.ScrollToLeadForm();
            testLogger.Information("STEP: Scrolled to inline 'Get in Touch' lead form.");

            // Fill all lead form fields
            await NoBrandISPage.FillLeadForm(formData);
            testLogger.Information("STEP: Filled lead form with test data.");

            // Submit form (60s wait for in-page confirmation)
            await NoBrandISPage.SubmitLeadForm();
            testLogger.Information("STEP: Submitted lead form.");

            // ASSERT — verify in-page thank-you confirmation is visible
            Assert.That(await NoBrandISPage.VerifyThankYouConfirmation(), Is.True,
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

