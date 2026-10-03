using Definition.Pages.LandingPages;
using Definition.TestData.FormData;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace Execution.RegressionTests.LeadFormsTests
{
    [TestFixture]
    [Category("Regression")]
    [Category("LeadForm")]
    public class Family_LP_LeadFormA_RegressionTest : UtilityBase
    {
        [SetUp]
        public void Setup()
        {
            // Configure test environment for UAT, US locale
            GetEnvironmentConfig("uat", "us");
        }

        [Test, Description("Verify Lead Form A on Family landing page submits successfully and redirects to Thank You page.")]
        public async Task LeadFormA_Submit_RedirectsToThankYou()
        {
            // Create test data and page object (Playwright)
            var formData = new LP_LeadFormA_FormData();
            FamilyPage landingPage = new(await GetPageAsync(), testLogger);

            // Navigate to landing page
            await landingPage.NavigateToLandingPage();
            testLogger.Information("PASSED: Navigated to Family landing page.");

            // Accept cookies
            await landingPage.AcceptCookies();
            testLogger.Information("PASSED: Accepted cookies.");

            // Scroll to lead form
            await landingPage.ScrollToLeadForm();
            testLogger.Information("PASSED: Scrolled to lead form.");

            // Fill lead form with test data
            await landingPage.FillLeadForm(formData);
            testLogger.Information("PASSED: Filled lead form with test data.");

            // Submit form
            await landingPage.SubmitLeadForm();
            testLogger.Information("PASSED: Submitted lead form.");

            // Verify Thank You page
            Assert.That(await landingPage.VerifyThankYouPage(), Is.True,
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

