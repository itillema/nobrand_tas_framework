using Definition.Pages.HomePages;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace Execution.RegressionTests.GeneralFunctionalityTests
{

    [TestFixture]
    [Category("Regression")]
    [Category("HomePage")]
    public class HomePage_RegressionTest : UtilityBase
    {
        [SetUp]
        public void Setup()
        {
            // Pass which environment and store you want to use for this test method, or leave the strings empty to test the with the Global environment config (Defined in UtilityBase class)
            GetEnvironmentConfig("uat", "us");

        }

        [Test, Description("Verify Home page loads and all main elements are present.")]
        public async Task NavigateToHomePage_CheckForMainElements()
        {

            // Navigate to Home page
            HomePage homePage = new(await GetPageAsync(), testLogger);
            testLogger.Information("Navigated to Home page.");

            // Verify cookie banner is visible and accept cookies
            await homePage.LoadHomePage_VerifyAndAcceptCookies();
            testLogger.Information("PASSED: Verified cookies banner is visible and cookies have been accepted.");

            // Verify main logo is visible
            Assert.That(await homePage.LoadHomePage_VerifyLogoVisible(), Is.True);
            testLogger.Information("PASSED: Verified main NoBrand logo is visible.");

            // Verify main Nav is visible
            Assert.That(await homePage.LoadHomePage_VerifyNavMenu(), Is.True);
            testLogger.Information("PASSED: Verified main header navigation is visible.");

            // Verify dropdown Nav is visible
            Assert.That(await homePage.LoadHomePage_VerifyDropdownNavMenu(), Is.True);
            testLogger.Information("PASSED: Verified header dropdown navigation is visible.");

            // Verify footer Nav is visible
            Assert.That(await homePage.LoadHomePage_VerifyFooterNavMenu(), Is.True);
            testLogger.Information("PASSED: Verified main footer navigation is visible.");

            // Verify user can navigate away and back to Home page
            Assert.That(await homePage.LoadHomePage_VerifyNavigateAwayAndBack(), Is.True);
            testLogger.Information("PASSED: Verified user can navigate away and back to Home page.");


        }

        [TearDown]
        public async Task TearDown()
        {
            await TestCleanupAsync();
        }

    }
}

