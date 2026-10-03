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
    public class NoBrandOfNoBrandIS_NoBrandMan_LeadFormB_RegressionTest : UtilityBase
    {
        [SetUp]
        public void Setup()
        {
            // Configure test environment for UAT, US locale
            GetEnvironmentConfig("uat", "us");
        }

        [Test, Description("Verify the NoBrand of NoBrandIS community page pop-up modal lead form submits successfully and a Thank You confirmation replaces the form in-modal.")]
        public async Task NoBrandOfNoBrandISLeadFormB_Submit_ReplacedWithThankYou()
        {
            // Reuse the shared NoBrand of NoBrandIS form data
            var formData = new CP_NoBrandOfNoBrandIS_FormData();
            CommunityPage_NoBrandOfNoBrandIS NoBrandISPage = new(await GetPageAsync(), testLogger);

            // Navigate to community page
            await NoBrandISPage.NavigateToNoBrandISPage();
            testLogger.Information("STEP: Navigated to the NoBrand of NoBrandIS community page.");

            await NoBrandISPage.AcceptCookies();
            testLogger.Information("STEP: Accepted cookies.");

            // Scroll to 'Limited Availability' container section (pricing overview card)
            await NoBrandISPage.ScrollToLimitedAvailability();
            testLogger.Information("STEP: Scrolled to the 'Limited Availability' container section.");

            // Click 'GET IN TOUCH' to open the lead form modal
            await NoBrandISPage.OpenLeadFormModal();
            testLogger.Information("STEP: Opened the pop-up lead form modal.");

            // Fill modal form fields
            await NoBrandISPage.FillLeadFormModal(formData);
            testLogger.Information("STEP: Filled modal lead form with test data.");

            // Submit modal form
            await NoBrandISPage.SubmitLeadFormModal();
            testLogger.Information("STEP: Submitted modal lead form.");

            // Verify the in-modal thank-you confirmation
            Assert.That(await NoBrandISPage.VerifyThankYouConfirmationInModal(), Is.True,
                "The in-modal thank-you confirmation did not appear after modal form submission.");
            testLogger.Information("PASSED: In-modal thank-you confirmation verified successfully.");
        }

        [TearDown]
        public async Task TearDown()
        {
            await TestCleanupAsync();
        }
    }
}

