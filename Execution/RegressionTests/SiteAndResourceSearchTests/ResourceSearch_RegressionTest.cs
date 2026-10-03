using Definition.Pages.HomePages;

using Utility;

namespace Execution.RegressionTests.SiteAndResourceSearchTests
{
    [TestFixture]
    [Category("Regression")]
    [Category("SiteSearch")]
    public class ResourceSearch_RegressionTest : UtilityBase
    {
        [SetUp]
        public void Setup()
        {
            // Pass which environment and store you want to use for this test method, or leave the strings empty to test the with the Global environment config (Defined in UtilityBase class)
            GetEnvironmentConfig("uat", "us");

        }

        [Test, Order(1), Description("Verify users can search the resources page and return results for a search term.")]
        public async Task SearchResourcesForTerm_NavigateToResourceSearchResultsPage()
        {

            // Navigate to Home page
            HomePage homePage = new(await GetPageAsync(), testLogger);
            testLogger.Information("Navigated to Home page.");

            // Verify cookie banner is visible and accept cookies
            await homePage.LoadHomePage_VerifyAndAcceptCookies();
            testLogger.Information("PASSED: Verified cookies banner is visible and cookies have been accepted.");

            // Verify users can search resources from the Resources page and return results for "test" search term
            var searchPage = await homePage.HeaderNavigation_NavigateToResourcesPage();
            var resultsPage = await searchPage.SearchForResource_NavigateToSearchResultsPage();
            Assert.That(await resultsPage.ResourceResults_VerifySearchResultsAreDisplayed(), Is.True, "FAILED: Resource search did not return results for 'test' search term.");

            testLogger.Information("PASSED: Verified resource search returns results for 'test' search term.");

        }

        [TearDown]
        public async Task TearDown()
        {
            await TestCleanupAsync();
        }

    }
}

