using Definition.Pages.HomePages;
using Definition.TestData.FormData;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using NUnit.Framework;
using System.Threading.Tasks;
using Utility;

namespace Execution.RegressionTests.LeadFormsTests
{
    [TestFixture]
    [Category("Regression")]
    [Category("LeadForm")]
    public class Home_LP_LeadFormA_RegressionTest : UtilityBase
    {
        [SetUp]
        public void Setup()
        {
            // Configure test environment for UAT, US locale
            GetEnvironmentConfig("uat", "us");
        }


        [Test, Description("Verify the lead form on the Home page can be submitted successfully and the browser is redirected to the Thank You page.")]
        public async Task SubmitHomeLeadForm_RedirectsToThankYou()
        {
            var formData = new Home_LeadFormA_FormData();
            HomePage homePage = new(await GetPageAsync(), testLogger);

            await homePage.NavigateToHome();
            testLogger.Information("STEP: Navigated to the Home page.");

            await homePage.LoadHomePage_VerifyAndAcceptCookies();
            testLogger.Information("STEP: Accepted cookies.");

            await homePage.ScrollToLeadForm();
            testLogger.Information("STEP: Scrolled to the 'Get in Touch to Learn More' lead form.");

            await homePage.FillLeadForm(formData);
            testLogger.Information("STEP: Filled the lead form with test data.");

            await homePage.SubmitLeadForm();
            testLogger.Information("STEP: Submitted the lead form.");

            Assert.That(await homePage.VerifyThankYouConfirmation(), Is.True,
                "The Thank You page heading was not visible after submitting the Home page lead form.");
            testLogger.Information("PASSED: Redirect to Thank You page verified after Home page lead form submission.");
        }

        [TearDown]
        public async Task TearDown()
        {
            await TestCleanupAsync();
        }

    }
}

