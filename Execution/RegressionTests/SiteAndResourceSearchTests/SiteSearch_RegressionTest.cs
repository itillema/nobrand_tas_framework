using Definition.Pages.HomePages;

using Utility;

namespace Execution.RegressionTests.SiteAndResourceSearchTests
{
    [TestFixture]
    [Category("Regression")]
    [Category("SiteSearch")]
    public class SiteSearch_RegressionTest : UtilityBase
    {
        [SetUp]
        public void Setup()
        {
            // Pass which environment and store you want to use for this test method, or leave the strings empty to test the with the Global environment config (Defined in UtilityBase class)
            GetEnvironmentConfig("uat", "us");

        }


        [Test, Order(1), Description("Verify users can search the site using the header site search and return results.")]
        public async Task SearchSiteForTerm_NavigateToSiteSearchResultsPage()
        {

            // Navigate to Home page
            HomePage homePage = new(await GetPageAsync(), testLogger);
            testLogger.Information("Navigated to Home page.");

            // Verify cookie banner is visible and accept cookies
            await homePage.LoadHomePage_VerifyAndAcceptCookies();
            testLogger.Information("PASSED: Verified cookies banner is visible and cookies have been accepted.");

            // Verify users can search site via site search from Home page and return results for "test" search term
            var searchResultsPage = await homePage.SiteSearch_SearchForTerm();
            Assert.That(await searchResultsPage.LoadSiteSearchResultsPage_VerifySearchResults(), Is.True);
            testLogger.Information("PASSED: Verified site search returns results for 'test' search term.");

        }

        [Test, Order(2), Description("Verify users can filter and sort site search results.")]
        public async Task SearchSiteForTerm_FilterAndSortSiteSearchResultsPage()
        {

            // Navigate to Home page
            HomePage homePage = new(await GetPageAsync(), testLogger);
            testLogger.Information("Navigated to Home page.");

            // Verify cookie banner is visible and accept cookies
            await homePage.LoadHomePage_VerifyAndAcceptCookies();
            testLogger.Information("PASSED: Verified cookies banner is visible and cookies have been accepted.");

            // Verify users can search site via site search from Home page and return results for "test" search term
            var searchResultsPage = await homePage.SiteSearch_SearchForTerm();
            Assert.That(await searchResultsPage.LoadSiteSearchResultsPage_FilterAndSortSearchResults(), Is.True);
            testLogger.Information("PASSED: Verified users can filter and sort site search results for 'test' search term.");

        }

        [Test, Order(3), Description("Verify users can navigate to a blog page from the site search results list.")]
        public async Task SearchSiteForTerm_NavigateToBlogPageFromResultsList()
        {

            // Navigate to Home page
            HomePage homePage = new(await GetPageAsync(), testLogger);
            testLogger.Information("Navigated to Home page.");

            // Verify cookie banner is visible and accept cookies
            await homePage.LoadHomePage_VerifyAndAcceptCookies();
            testLogger.Information("PASSED: Verified cookies banner is visible and cookies have been accepted.");

            // Verify users can search site via site search from Home page, return results, and navigate to a blog page from the results list
            var searchResultsPage = await homePage.SiteSearch_SearchForTerm();
            var blogPage = await searchResultsPage.LoadSiteSearchResultsPage_ClickSearchResultAndNavigateToBlogPage();
            Assert.That(await blogPage.LoadHowToOrganizeANoBrandusersMedicalInformation_ResourcePage_VerifyTitle(), Is.True);
            testLogger.Information("PASSED: Verified users can navigate to a blog page from site search results.");

        }


        [TearDown]
        public async Task TearDown()
        {
            await TestCleanupAsync();
        }

    }
}

