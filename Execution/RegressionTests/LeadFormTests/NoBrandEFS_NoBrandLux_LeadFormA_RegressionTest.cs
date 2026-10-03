using Definition.Pages.CommunityPages.US_CommunityPages;
using Definition.TestData.FormData;
using Utility;

namespace Execution.RegressionTests.LeadFormsTests
{
    [TestFixture]
    [Category("Regression")]
    [Category("LeadForm")]
    public class NoBrandEFS_NoBrandlux_LeadFormA_RegressionTest : UtilityBase
    {
        [SetUp]
        public void Setup()
        {
            // Configure test environment for UAT, US locale
            GetEnvironmentConfig("uat", "us");
        }

        [Test, Description("Verify the NoBrandEFS NoBrandlux community page lead form submits successfully and a Thank You confirmation replaces the form in-page.")]
        public async Task NoBrandEFSNoBrandluxLeadFormA_Submit_ReplacedWithThankYou()
        {
            // Create form data and page object
            var formData = new CP_NoBrandEFS_FormData();
            CommunityPage_NoBrandEFS NoBrandEFSPage = new(await GetPageAsync(), testLogger);

            // Navigate to community page
            await NoBrandEFSPage.NavigateToNoBrandEFSPage();
            testLogger.Information("STEP: Navigated to the NoBrand at NoBrandEFSth community page.");

            // Accept cookie consent banner
            await NoBrandEFSPage.AcceptCookies();
            testLogger.Information("STEP: Accepted cookies.");

            // Scroll to inline 'Get In Touch with NoBrand at NoBrandEFSth' lead form
            await NoBrandEFSPage.ScrollToLeadForm();
            testLogger.Information("STEP: Scrolled to the 'Get In Touch with NoBrand at NoBrandEFSth' lead form.");

            // Fill all lead form fields and check both checkbNoBrandes (deluxe suites + marketing consent)
            await NoBrandEFSPage.FillLeadForm(formData);
            testLogger.Information("STEP: Filled lead form with test data.");

            // Submit form (60s wait for in-page confirmation)
            await NoBrandEFSPage.SubmitLeadForm();
            testLogger.Information("STEP: Submitted lead form.");

            // Verify in-page thank-you confirmation is visible
            Assert.That(await NoBrandEFSPage.VerifyThankYouConfirmation(), Is.True,
                "The in-page 'Thank you!' confirmation did not appear after form submission.");
            testLogger.Information("PASSED: In-page thank-you confirmation verified successfully.");
        }

        [TearDown]
        public async Task TearDown()
        {
            await TestCleanupAsync();
        }
    }
}

