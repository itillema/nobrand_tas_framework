using Definition.Pages.CommunityPages.US_CommunityPages;
using Definition.TestData.FormData;
using Utility;

namespace Execution.RegressionTests.LeadFormsTests
{
    [TestFixture]
    [Category("Regression")]
    [Category("LeadForm")]
    public class NoBrandAp_NoBrandlux_LeadFormA_RegressionTest : UtilityBase
    {
        [SetUp]
        public void Setup()
        {
            // Configure test environment for UAT, US locale
            GetEnvironmentConfig("uat", "us");
        }

        [Test, Description("Verify the inline NoBrandAp NoBrandlux community page 'Get in Touch' lead form submits successfully and a Thank You confirmation replaces the form in-page.")]
        public async Task NoBrandApNoBrandluxLeadFormA_Submit_ReplacedWithThankYou()
        {
            // Create form data and page object
            var formData = new CP_TheNoBrandAp_FormData();
            CommunityPage_TheNoBrandAp NoBrandApPage = new(await GetPageAsync(), testLogger);

            // Navigate to community page
            await NoBrandApPage.NavigateToNoBrandApPage();
            testLogger.Information("STEP: Navigated to The NoBrandAp community page.");

            // Accept cookie consent banner
            await NoBrandApPage.AcceptCookies();
            testLogger.Information("STEP: Accepted cookies.");

            // Scroll to inline 'GET IN TOUCH WITH THE NoBrandAp' lead form
            await NoBrandApPage.ScrollToLeadForm();
            testLogger.Information("STEP: Scrolled to the 'Get in Touch with The NoBrandAp' lead form.");

            // Fill all lead form fields and check both checkbNoBrandes (Founders Club + marketing consent)
            await NoBrandApPage.FillLeadForm(formData);
            testLogger.Information("STEP: Filled lead form with test data.");

            // Submit form (60s wait for in-page confirmation)
            await NoBrandApPage.SubmitLeadForm();
            testLogger.Information("STEP: Submitted lead form.");

            // Verify in-page thank-you confirmation is visible
            Assert.That(await NoBrandApPage.VerifyThankYouConfirmation(), Is.True,
                "The in-page 'Thank You!' confirmation did not appear after form submission.");
            testLogger.Information("PASSED: In-page thank-you confirmation verified successfully.");
        }

        [TearDown]
        public async Task TearDown()
        {
            await TestCleanupAsync();
        }
    }
}

