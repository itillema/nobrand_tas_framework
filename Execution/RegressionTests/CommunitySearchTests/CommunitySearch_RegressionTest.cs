using Definition.Pages.HomePages;
using Utility;

namespace Execution.RegressionTests.CommunitySearchTests
{
    [TestFixture]
    [Category("Regression")]
    [Category("CommunitySearch")]
    public class CommunitySearch_RegressionTest : UtilityBase
    {
        [SetUp]
        public void Setup()
        {
            // Pass which environment and locale you want to use for this test method, or leave the strings empty to test the with the Global environment config (Defined in UtilityBase class)
            GetEnvironmentConfig("uat", "us");

        }

        [Test, Order(1), Description("Verify users can search for communities through the community search form on the Home page.")]
        public async Task HomePage_SearchForCommunities()
        {

            // Navigate to Home page
            HomePage homePage = new(await GetPageAsync(), testLogger);

            // Verify cookie banner is visible and accept cookies
            await homePage.LoadHomePage_VerifyAndAcceptCookies();
            testLogger.Information("PASSED: Verified cookies banner is visible and cookies have been accepted.");

            // Perform search and get the results page object
            var searchResultsPage = await homePage.HomePage_FindANoBrandHome_US_SearchForCommunity();
            testLogger.Information("PASSED: User has searched for a community from the Home page.");

            // Verify results page loaded
            Assert.That(await searchResultsPage.VerifySearchResultsLoaded(), Is.True, "Community Search Results page did not load correctly or no results were found.");
            testLogger.Information("PASSED: Community Search Results page loaded and results are visible.");
        }

        [Test, Order(2), Description("Verify users can click on a search result and navigate to the respective community page.")]
        public async Task SearchForCommunity_VerifyNavigationToCommunityPage()
        {

            // Navigate to Home page
            HomePage homePage = new(await GetPageAsync(), testLogger);

            // Verify cookie banner is visible and accept cookies
            await homePage.LoadHomePage_VerifyAndAcceptCookies();
            testLogger.Information("PASSED: Verified cookies banner is visible and cookies have been accepted.");

            // Perform search and get the results page object
            var searchResultsPage = await homePage.HomePage_FindANoBrandHome_US_SearchForCommunity();
            testLogger.Information("PASSED: User has searched for a community from the Home page.");

            // Verify results page loaded and click on the first search result to navigate to the community page
            var communityPage = await searchResultsPage.SelectSearchResult_NavigateToCommunity();
            testLogger.Information("PASSED: Community Search Results page loaded and the first result has been selected.");

            // Verify the community page loaded correctly
            Assert.That(await communityPage.LoadNoBrandCommunityPage_VerifySubheading(), Is.True, "Community page did not load correctly or subheading is not visible.");
            testLogger.Information("PASSED: Navigated to the community page and verified the subheading is visible, confirming the page loaded correctly.");

        }


        [TearDown]
        public async Task TearDown()
        {
            await TestCleanupAsync();
        }

    }
}

