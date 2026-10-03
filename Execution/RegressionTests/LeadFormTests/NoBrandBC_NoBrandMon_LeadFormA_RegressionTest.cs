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
    public class NoBrandBCCourt_NoBrandBC_LeadFormA_RegressionTest : UtilityBase
    {
        [SetUp]
        public void Setup()
        {
            // Configure test environment for UAT, US locale
            GetEnvironmentConfig("uat", "us");
        }

        [Test, Description("Verify the NoBrandBC NoBrandBC community page lead form submits successfully and the user is navigated to a Thank You confirmation page.")]
        public async Task NoBrandBCCourtNoBrandBCLeadFormA_Submit_NavigatedToThankYouPage()
        {
            // Create form data and page object
            var formData = new CP_NoBrandBCCourt_FormData();
            CommunityPage_NoBrandBCCourt NoBrandBCPage = new(await GetPageAsync(), testLogger);

            // Navigate to community page
            await NoBrandBCPage.NavigateToNoBrandBCCourtPage();
            testLogger.Information("STEP: Navigated to NoBrandBC community page.");

            // Accept cookie consent banner
            await NoBrandBCPage.AcceptCookies();
            testLogger.Information("STEP: Accepted cookies.");

            // Scroll to the "Want to know more about NoBrandBC?" lead form near the bottom of the page
            await NoBrandBCPage.ScrollToLeadForm();
            testLogger.Information("STEP: Scrolled to 'Want to know more about NoBrandBC?' lead form.");

            // Fill all lead form fields and check the marketing agreement checkbNoBrand
            await NoBrandBCPage.FillLeadForm(formData);
            testLogger.Information("STEP: Filled lead form with test data.");

            // Submit form (redirects to the Thank You page)
            await NoBrandBCPage.SubmitLeadForm();
            testLogger.Information("STEP: Submitted lead form.");

            // Verify the user was navigated to the NoBrandBC Thank You confirmation page
            Assert.That(await NoBrandBCPage.VerifyThankYouPageNavigation(), Is.True,
                "The user was not navigated to the 'Thank you for your interest in NoBrandBC' confirmation page after form submission.");
            testLogger.Information("PASSED: Thank-you confirmation page verified.");
        }

        [TearDown]
        public async Task TearDown()
        {
            await TestCleanupAsync();
        }
    }
}


