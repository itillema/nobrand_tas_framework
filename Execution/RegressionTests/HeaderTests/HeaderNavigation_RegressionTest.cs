using Definition.Pages.HomePages;
using Utility;

namespace Execution.RegressionTests.HeaderTests
{
    [TestFixture]
    [Category("Regression")]
    [Category("Navigation")]
    public class HeaderNavigation_RegressionTest : UtilityBase
    {
        [SetUp]
        public void Setup()
        {
            // Pass which environment and store you want to use for this test method, or leave the strings empty to test the with the Global environment config (Defined in UtilityBase class)
            GetEnvironmentConfig("uat", "us");

        }


        [Test, Order(1), Description("Verify all header main navigation elements are present on the Home page.")]
        public async Task NavigateToHomePage_CheckForHeaderMainNavigationElements()
        {
            // Navigate to Home page (Playwright)
            HomePage homePage = new(await GetPageAsync(), testLogger);
            testLogger.Information("Navigated to Home page.");

            // Verify cookie banner is visible and accept cookies
            await homePage.LoadHomePage_VerifyAndAcceptCookies();
            testLogger.Information("PASSED: Verified cookies banner is visible and cookies have been accepted.");

            // Verify Experience NoBrand navigation menu item
            Assert.That(await homePage.HeaderNavigation_VerifyExperienceNoBrandNavMenuItem(), Is.True);
            testLogger.Information("PASSED: Verified Experience NoBrand navigation menu item is visible.");

            // Verify NoBrandaction & Living navigation menu item
            Assert.That(await homePage.HeaderNavigation_VerifyNoBrandactionAndLivingNavMenuItem(), Is.True);
            testLogger.Information("PASSED: Verified NoBrandaction & Living navigation menu item is visible.");

            // Verify Advice & Planning navigation menu item
            Assert.That(await homePage.HeaderNavigation_VerifyAdviceAndPlanningNavMenuItem(), Is.True);
            testLogger.Information("PASSED: Verified Advice & Planning navigation menu item is visible.");
        }


        [Test, Order(2), Description("Verify all header dropdown navigation elements are present on the Home page.")]
        public async Task NavigateToHomePage_CheckForHeaderDropdownNavigationElements()
        {
            // Navigate to Home page (Playwright)
            HomePage homePage = new(await GetPageAsync(), testLogger);
            testLogger.Information("Navigated to Home page.");

            // Verify cookie banner is visible and accept cookies
            await homePage.LoadHomePage_VerifyAndAcceptCookies();
            testLogger.Information("PASSED: Verified cookies banner is visible and cookies have been accepted.");

            // Open dropdown navigation menu
            await homePage.OpenHeaderDropdownMenu();
            testLogger.Information("Opened header dropdown menu.");

            // Verify For Professionals navigation menu item
            Assert.That(await homePage.HeaderNavigation_VerifyForProfessionalsNavMenu(), Is.True);
            testLogger.Information("PASSED: Verified For Professionals navigation menu item is visible.");

            // Verify NoBrandactioners at NoBrand navigation menu item
            Assert.That(await homePage.HeaderNavigation_VerifyNoBrandactionersAtNoBrandNavMenu(), Is.True);
            testLogger.Information("PASSED: Verified NoBrandactioners at NoBrand navigation menu item is visible.");

            // Verify About Us navigation menu item
            Assert.That(await homePage.HeaderNavigation_VerifyAboutUsNavMenuItem(), Is.True);
            testLogger.Information("PASSED: Verified About Us navigation menu item is visible.");

            // Close dropdown navigation menu
            await homePage.CloseHeaderDropdownMenu();
            testLogger.Information("Closed header dropdown menu.");
        }

        [TearDown]
        public async Task TearDown()
        {
            await TestCleanupAsync();
        }

    }
}

