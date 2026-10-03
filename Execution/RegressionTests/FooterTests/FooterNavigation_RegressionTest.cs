using Definition.Pages.HomePages;

using Utility;

namespace Execution.RegressionTests.FooterTests
{
    [TestFixture]
    [Category("Regression")]
    [Category("Navigation")]
    public class FooterNavigation_RegressionTest : UtilityBase
    {
        [SetUp]
        public void Setup()
        {
            // Pass which environment and store you want to use for this test method, or leave the strings empty to test the with the Global environment config (Defined in UtilityBase class)
            GetEnvironmentConfig("uat", "us");

        }

        [Test, Description("Verify all footer navigation elements are present on the Home page.")]
        public async Task NavigateToHomePage_CheckForFooterNavigationElements()
        {
            // Navigate to Home page (Playwright)
            HomePage homePage = new(await GetPageAsync(), testLogger);
            testLogger.Information("Navigated to Home page.");

            // Scroll down to footer and verify nav is visible
            Assert.That(await homePage.LoadHomePage_VerifyFooterNavMenu(), Is.True);
            testLogger.Information("PASSED: Verified main footer navigation is visible.");

            // Verify About Us navigation menu item link is visible
            Assert.That(await homePage.FooterNavigation_VerifyAboutUsNavMenuItem(), Is.True);
            testLogger.Information("PASSED: Verified About Us footer link is visible.");

            // Verify NoBrandactioners at No Brand navigation menu item link is visible
            Assert.That(await homePage.FooterNavigation_VerifyNoBrandactionersAtNoBrandNavMenuItem(), Is.True);
            testLogger.Information("PASSED: Verified NoBrandactioners at No Brand footer link is visible.");

            // Verify NoBrand Blog navigation menu item link is visible
            Assert.That(await homePage.FooterNavigation_VerifyNoBrandBlogNavMenuItem(), Is.True);
            testLogger.Information("PASSED: Verified NoBrand Blog footer link is visible.");

            // Verify For Professionals navigation menu item link is visible
            Assert.That(await homePage.FooterNavigation_VerifyForProfessionalsNavMenuItem(), Is.True);
            testLogger.Information("PASSED: Verified For Professionals footer link is visible.");
        }

        [TearDown]
        public async Task TearDown()
        {
            await TestCleanupAsync();
        }


    }
}


