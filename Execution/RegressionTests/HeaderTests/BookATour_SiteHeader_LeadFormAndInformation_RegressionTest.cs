using Definition.Pages.CommunityPages.US_CommunityPages;
using Definition.Pages.HomePages;
using Definition.TestData.FormData;
using Utility;

namespace Execution.RegressionTests.HeaderTests
{
    [TestFixture]
    [Category("Regression")]
    [Category("LeadForm")]
    public class BookATour_SiteHeader_LeadFormAndInformation_RegressionTest : UtilityBase
    {
        [SetUp]
        public void Setup()
        {
            // Configure test environment for UAT, US locale
            GetEnvironmentConfig("uat", "us");
        }


        [Test, Description("Verify the site header 'Book a Tour' trigger is clickable and expands the dropdown panel below the header.")]
        public async Task BookATour_DropdownTrigger_Click_ExpandsDropdown()
        {
            var pw = await GetPageAsync();
            CommunityPage_NoBrandOfNoBrandLP communityPage = new(pw, testLogger);
            BookATour_DropdownMenu bookATour = new(pw, testLogger);

            await communityPage.NavigateToNoBrandOfNoBrandLPPage();
            testLogger.Information("STEP: Navigated to NoBrand of NoBrandLP community page.");

            await communityPage.AcceptCookies();
            testLogger.Information("STEP: Accepted cookies.");

            await bookATour.OpenBookATourDropdown();
            testLogger.Information("STEP: Clicked the 'Book a Tour' header trigger.");

            Assert.That(await bookATour.VerifyDropdownIsExpanded(), Is.True,
                "The Book a Tour dropdown panel did not become visible after clicking the header trigger.");
            testLogger.Information("PASSED: Book a Tour dropdown panel expanded below the header.");
        }


        [Test, Description("Verify the 'Book a Tour' lead form is visible inside the dropdown panel.")]
        public async Task BookATour_Dropdown_LeadFormIsVisible()
        {
            var pw = await GetPageAsync();
            CommunityPage_NoBrandOfNoBrandLP communityPage = new(pw, testLogger);
            BookATour_DropdownMenu bookATour = new(pw, testLogger);

            await communityPage.NavigateToNoBrandOfNoBrandLPPage();
            testLogger.Information("STEP: Navigated to NoBrand of NoBrandLP community page.");

            await communityPage.AcceptCookies();
            testLogger.Information("STEP: Accepted cookies.");

            await bookATour.OpenBookATourDropdown();
            testLogger.Information("STEP: Opened the 'Book a Tour' dropdown.");

            Assert.That(await bookATour.VerifyLeadFormIsVisible(), Is.True,
                "The Book a Tour lead form did not display inside the dropdown panel.");
            testLogger.Information("PASSED: Book a Tour lead form is visible inside the dropdown.");
        }


        [Test, Description("Verify the 'Now Viewing' community link/name and 'Call' phone-number links are visible to the right of the Book a Tour lead form.")]
        public async Task BookATour_Dropdown_CommunityContactInfoIsVisible()
        {
            var pw = await GetPageAsync();
            CommunityPage_NoBrandOfNoBrandLP communityPage = new(pw, testLogger);
            BookATour_DropdownMenu bookATour = new(pw, testLogger);

            await communityPage.NavigateToNoBrandOfNoBrandLPPage();
            testLogger.Information("STEP: Navigated to NoBrand of NoBrandLP community page.");

            await communityPage.AcceptCookies();
            testLogger.Information("STEP: Accepted cookies.");

            await bookATour.OpenBookATourDropdown();
            testLogger.Information("STEP: Opened the 'Book a Tour' dropdown.");

            await Assert.MultipleAsync(async () =>
            {
                Assert.That(await bookATour.VerifyNowViewingSection_NoBrandOfNoBrandLP(), Is.True,
                    "Now Viewing section did not display the correct NoBrand of NoBrandLP link and name.");
                Assert.That(await bookATour.VerifyCallSection(), Is.True,
                    "Call section did not display both phone-number links.");
            });
            testLogger.Information("PASSED: Now Viewing and Call sections verified.");
        }


        [Test, Description("Verify the site header 'Book a Tour' lead form submits successfully and a Thank You confirmation replaces the form in-place.")]
        public async Task BookATourLeadForm_Submit_ReplacedWithThankYou()
        {
            var formData = new BookATour_FormData();
            var pw = await GetPageAsync();
            CommunityPage_NoBrandOfNoBrandLP communityPage = new(pw, testLogger);
            BookATour_DropdownMenu bookATour = new(pw, testLogger);

            await communityPage.NavigateToNoBrandOfNoBrandLPPage();
            testLogger.Information("STEP: Navigated to NoBrand of NoBrandLP community page.");

            await communityPage.AcceptCookies();
            testLogger.Information("STEP: Accepted cookies.");

            await bookATour.OpenBookATourDropdown();
            testLogger.Information("STEP: Opened the 'Book a Tour' dropdown.");

            await bookATour.FillBookATourLeadForm(formData);
            testLogger.Information("STEP: Filled Book a Tour lead form with test data.");

            await bookATour.SubmitBookATourLeadForm();
            testLogger.Information("STEP: Submitted Book a Tour lead form.");

            Assert.That(await bookATour.VerifyThankYouConfirmation(), Is.True,
                "The in-place Thank-You confirmation did not replace the Book a Tour form after submission.");
            testLogger.Information("PASSED: In-place thank-you confirmation verified.");
        }


        [TearDown]
        public async Task TearDown()
        {
            await TestCleanupAsync();
        }
    }
}

