using Definition.Pages.AboutPages;
using Definition.Pages.AdviceAndPlanningPages;
using Definition.Pages.NoBrandactionersPages;
using Definition.Pages.NoBrandactionQuestionnairePages;
using Definition.Pages.CommunityPages.US_CommunityPages;
using Definition.Pages.ExperienceNoBrandPages;
using Definition.Pages.HomePages;
using Definition.Pages.LandingPages;
using Definition.Pages.LegalPages;
using Definition.Pages.ProfessionalsPages;
using Definition.Pages.ResourcePages;
using Definition.Pages.SiteSearchPages;
using Utility;

namespace Execution.RegressionTests.PageTypeChecksTests
{
    [TestFixture]
    [Category("Regression")]
    [Category("PageLoad")]
    public class PageLoadingChecks_US_RegressionTest : UtilityBase
    {

        [SetUp]
        public void Setup()
        {
            // Pass which environment and store you want to use for this test method, or leave the strings empty to test the with the Global environment config (Defined in UtilityBase class)
            GetEnvironmentConfig("uat", "us");

        }

        [Test, Order(1), Description("Verify Home page loads.")]
        public async Task NavigateToHomePage_CheckForMainElements()
        {
            HomePage homePage = new(await GetPageAsync(), testLogger);
            await homePage.LoadHomePage_VerifyAndAcceptCookies();
            testLogger.Information("PASSED: Verified cookies banner is visible and cookies have been accepted.");

            // Verify main logo is visible
            Assert.That(await homePage.LoadHomePage_VerifyLogoVisible(), Is.True);
            testLogger.Information("PASSED: Verified main NoBrand logo is visible.");
        }

        [Test, Order(2), Description("Verify a community details page loads.")]
        public async Task NavigateToCommunityDetailsPage_CheckForMainElements()
        {
            var page = await GetPageAsync();
            HomePage homePage = new(page, testLogger);
            await homePage.LoadHomePage_VerifyAndAcceptCookies();
            testLogger.Information("PASSED: Verified cookies banner is visible and cookies have been accepted.");

            // Define community details page path
            string communityPath = "/communities/il/NoBrand-of-lincoln-park/";

            // Navigate to a community details page (NoBrand of NoBrandLP)
            await page.GotoDomReadyAsync(BaseUrl + communityPath);
            CommunityPage_NoBrandOfNoBrandLP communityDetailsPage = new(page, testLogger);

            Assert.That(await communityDetailsPage.LoadNoBrandofNoBrandLPCommunityPage_VerifySubheading(), Is.True);
            testLogger.Information("PASSED: Verified community details page (NoBrand of NoBrandLP) loaded as expected.");
        }

        [Test, Order(3), Description("Verify a resource page loads.")]
        public async Task NavigateToResourceBlogPage_CheckForMainElements()
        {
            var page = await GetPageAsync();

            // Define resource page path
            string resourcePath = "/resources/NoBrandactiongivers-and-families/signs-its-time-for-assisted-living";

            // Navigate to a resource page (Signs it's time for NoBrand type 1)
            await page.GotoDomReadyAsync(BaseUrl + resourcePath);
            SignsItsTimeForNoBrand type 1_ResourcePage resourcePage = new(page, testLogger);

            Assert.That(await resourcePage.LoadSignsItsTimeForNoBrand_ResourcePage_VerifyTitle(), Is.True);
            testLogger.Information("PASSED: Verified resource page loaded as expected.");
        }

        [Test, Order(4), Description("Verify the NoBrandactioners page loads.")]
        public async Task NavigateToNoBrandactionersPage_CheckForMainElements()
        {
            var page = await GetPageAsync();

            string resourcePath = "/NoBrandactioners/";

            // Navigate to the NoBrandactioners page
            await page.GotoDomReadyAsync(BaseUrl + resourcePath);
            NoBrandactionersPage NoBrandactionersPage = new(page, testLogger);

            Assert.That(await NoBrandactionersPage.LoadNoBrandactionersPage_VerifyHeading(), Is.True);
            testLogger.Information("PASSED: Verified the NoBrandactioners page loads as expected.");
        }

        [Test, Order(5), Description("Verify the family landing page loads.")]
        public async Task NavigateToFamilyLandingPage_CheckForMainElements()
        {
            var page = await GetPageAsync();

            string resourcePath = "/lp/family/";

            // Navigate to the family landing page
            await page.GotoDomReadyAsync(BaseUrl + resourcePath);
            FamilyPage familyPage = new(page, testLogger);

            Assert.That(await familyPage.LoadFamilyLandingPage_VerifyHeading(), Is.True);
            testLogger.Information("PASSED: Verified the family landing page loaded as expected.");
        }

        [Test, Order(6), Description("Verify the privacy policy page loads.")]
        public async Task NavigateToPrivacyPolicyPage_CheckForMainElements()
        {
            var page = await GetPageAsync();

            string resourcePath = "/privacy-policy/";

            // Navigate to the privacy policy page
            await page.GotoDomReadyAsync(BaseUrl + resourcePath);
            PrivacyPolicyPage privacyPolicyPage = new(page, testLogger);

            Assert.That(await privacyPolicyPage.LoadPrivacyPolicyPage_VerifyHeading(), Is.True);
            testLogger.Information("PASSED: Verified the privacy policy page loaded as expected.");
        }

        [Test, Order(7), Description("Verify the For Professionals page loads.")]
        public async Task NavigateToForProfessionalsPage_CheckForMainElements()
        {
            var page = await GetPageAsync();

            string resourcePath = "/healthNoBrandaction-professionals/";

            // Navigate to the for professionals page
            await page.GotoDomReadyAsync(BaseUrl + resourcePath);
            ForProfessionalsPage professionalsPage = new(page, testLogger);

            Assert.That(await professionalsPage.LoadForProfessionalsPage_VerifyHeading(), Is.True);
            testLogger.Information("PASSED: Verified the for professionals page loaded as expected.");
        }

        [Test, Order(8), Description("Verify an About Us page (Vendor Partnerships page) loads.")]
        public async Task NavigateToVendorPartnershipsPage_CheckForMainElements()
        {
            var page = await GetPageAsync();

            string resourcePath = "/about/vendors/";

            // Navigate to the About / Vendor Partnerships page
            await page.GotoDomReadyAsync(BaseUrl + resourcePath);
            VenderPartnerships venderPartnershipsPage = new(page, testLogger);

            Assert.That(await venderPartnershipsPage.LoadVenderPartnershipsPage_VerifyHeading(), Is.True);
            testLogger.Information("PASSED: Verified the About / Vendor Partnerships page loaded as expected.");
        }

        [Test, Order(9), Description("Verify the Advice and Planning page loads.")]
        public async Task NavigateToAdvicePlanningPage_CheckForMainElements()
        {
            var page = await GetPageAsync();

            string resourcePath = "/advice-and-planning/";

            // Navigate to the Advice and Planning page
            await page.GotoDomReadyAsync(BaseUrl + resourcePath);
            AdviceAndPlanningPage advicePlanningPage = new(page, testLogger);

            Assert.That(await advicePlanningPage.LoadAdviceAndPlanningPage_VerifyHeading(), Is.True);
            testLogger.Information("PASSED: Verified the Advice and Planning page loaded as expected.");
        }

        [Test, Order(10), Description("Verify the Experience NoBrand page loads.")]
        public async Task NavigateToExperienceNoBrandPage_CheckForMainElements()
        {
            var page = await GetPageAsync();

            string resourcePath = "/experience-NoBrand/";

            // Navigate to the Experience NoBrand page
            await page.GotoDomReadyAsync(BaseUrl + resourcePath);
            ExperienceNoBrandPage experienceNoBrandPage = new(page, testLogger);

            Assert.That(await experienceNoBrandPage.LoadExperienceNoBrandPage_VerifyHeading(), Is.True);
            testLogger.Information("PASSED: Verified the Experience NoBrand page loaded as expected.");
        }

        [Test, Order(11), Description("Verify the About Leadership page loads.")]
        public async Task NavigateToAboutLeadershipPage_CheckForMainElements()
        {
            var page = await GetPageAsync();

            string resourcePath = "/about/leadership/";

            // Navigate to the About Leadership page
            await page.GotoDomReadyAsync(BaseUrl + resourcePath);
            LeadershipPage leadershipPage = new(page, testLogger);

            Assert.That(await leadershipPage.LoadLeadershipPage_VerifyHeading(), Is.True);
            testLogger.Information("PASSED: Verified the Leadership page loaded as expected.");
        }

        [Test, Order(12), Description("Verify a Confirmation 'Thank you' page loads.")]
        public async Task NavigateToThankYouPage_CheckForMainElements()
        {
            var page = await GetPageAsync();

            string resourcePath = "/thank-you-schedule-a-tour/";

            // Navigate to the Thank You page
            await page.GotoDomReadyAsync(BaseUrl + resourcePath);
            ThankYouPage thankYouPage = new(page, testLogger);

            Assert.That(await thankYouPage.LoadThankYouPage_VerifyHeading(), Is.True);
            testLogger.Information("PASSED: Verified the Thank You page loaded as expected.");
        }

        [Test, Order(13), Description("Verify the NoBrandaction Questionnaire page loads.")]
        public async Task NavigateToNoBrandactionQuestionnairePage_CheckForMainElements()
        {
            var page = await GetPageAsync();

            string resourcePath = "/NoBrandaction-questionnaire/";

            // Navigate to the NoBrandaction Questionnaire page
            await page.GotoDomReadyAsync(BaseUrl + resourcePath);
            NoBrandactionQuestionnairePage NoBrandactionQuestionnairePage = new(page, testLogger);

            Assert.That(await NoBrandactionQuestionnairePage.LoadNoBrandactionQuestionnairePage_VerifyHeading(), Is.True);
            testLogger.Information("PASSED: Verified the NoBrandaction Questionnaire page loaded as expected.");
        }

        [Test, Order(14), Description("Verify a Resource Category page loads.")]
        public async Task NavigateToHealthAndWellnessResourceCategoryPage_CheckForMainElements()
        {
            var page = await GetPageAsync();

            string resourcePath = "/resources/health-and-wellness/";

            // Navigate to the Health and Wellness resource category page
            await page.GotoDomReadyAsync(BaseUrl + resourcePath);
            HealthAndWellness_CategoryPage healthAndWellnessPage = new(page, testLogger);

            Assert.That(await healthAndWellnessPage.LoadHealthAndWellnessPage_VerifyHeading(), Is.True);
            testLogger.Information("PASSED: Verified the Resource Category (Health and Wellness) page loaded as expected.");
        }

        [Test, Order(15), Description("Verify the Frequently Asked Questions page loads.")]
        public async Task NavigateToFrequentlyAskedQuestionsPage_CheckForMainElements()
        {
            var page = await GetPageAsync();

            string resourcePath = "/advice-and-planning/frequently-asked-questions/";

            // Navigate to the Frequently Asked Questions page
            await page.GotoDomReadyAsync(BaseUrl + resourcePath);
            FrequentlyAskedQuestionsPage frequentlyAskedQuestionsPage = new(page, testLogger);

            Assert.That(await frequentlyAskedQuestionsPage.LoadFrequentlyAskedQuestionsPage_VerifyHeading(), Is.True);
            testLogger.Information("PASSED: Verified the Frequently Asked Questions page loaded as expected.");
        }

        [Test, Order(16), Description("Verify the NoBrandaction Questionnaire Results page loads.")]
        public async Task NavigateToNoBrandactionQuestionnaireResultsPage_CheckForMainElements()
        {
            var page = await GetPageAsync();

            string resourcePath = "/NoBrandaction-questionnaire/result-assisted-living/";

            // Navigate to the NoBrandaction Questionnaire Results page
            await page.GotoDomReadyAsync(BaseUrl + resourcePath);
            NoBrandactionQuestionnaireResultsPage NoBrandactionQuestionnaireResultsPage = new(page, testLogger);

            Assert.That(await NoBrandactionQuestionnaireResultsPage.LoadNoBrandactionQuestionnaireResultsPage_VerifyHeading(), Is.True);
            testLogger.Information("PASSED: Verified the NoBrandaction Questionnaire Results page loaded as expected.");
        }

        [Test, Order(17), Description("Verify the Site Search Results page loads.")]
        public async Task NavigateToSiteSearchResultsPage_CheckForMainElements()
        {
            var page = await GetPageAsync();

            string resourcePath = "/search#q=NoBrand";

            // Navigate to the Site Search Results page
            await page.GotoDomReadyAsync(BaseUrl + resourcePath);
            SiteSearchResultsPage siteSearchResultsPage = new(page, testLogger);

            Assert.That(await siteSearchResultsPage.LoadSiteSearchResultsPage_VerifyHeading(), Is.True);
            Assert.That(await siteSearchResultsPage.LoadSiteSearchResultsPage_VerifySearchTerm(), Is.True);
            testLogger.Information("PASSED: Verified the Site Search Results page loaded as expected.");
        }

        [Test, Order(18), Description("Verify the Community State Listing page loads.")]
        public async Task NavigateToCommunityStateListingPage_CheckForMainElements()
        {
            var page = await GetPageAsync();

            string resourcePath = "/community-state-listing";

            // Navigate to the Community State Listing page
            await page.GotoDomReadyAsync(BaseUrl + resourcePath);
            US_CommunityStateListingPage stateListingPage = new(page, testLogger);

            Assert.That(await stateListingPage.LoadCommunityStateListingPage_VerifyHeading(), Is.True);
            testLogger.Information("PASSED: Verified the Community State Listing page loaded as expected.");
        }

        [Test, Order(19), Description("Verify the Community Search State Listing page loads.")]
        public async Task NavigateToCommunitySearchStateListingPage_CheckForMainElements()
        {
            var page = await GetPageAsync();

            string resourcePath = "/communities/il";

            // Navigate to the Community Search State Listing page
            await page.GotoDomReadyAsync(BaseUrl + resourcePath);
            US_CommunitySearch_StateListingPage communitySearchStateListingPage = new(page, testLogger);

            Assert.That(await communitySearchStateListingPage.LoadCommunitySearchStateListingPage_VerifyHeading(), Is.True);
            Assert.That(await communitySearchStateListingPage.LoadCommunitySearchStateListingPage_VerifySearchResults(), Is.True);
            testLogger.Information("PASSED: Verified the Community Search State Listing page loaded as expected.");
        }

        [Test, Order(20), Description("Verify the Community Search page loads.")]
        public async Task NavigateToCommunitySearchPage_CheckForMainElements()
        {
            var page = await GetPageAsync();

            string resourcePath = "/communities";

            // Navigate to the Community Search page
            await page.GotoDomReadyAsync(BaseUrl + resourcePath);
            US_CommunitySearchResultsPage communitySearchResultsPage = new(page, testLogger);

            Assert.That(await communitySearchResultsPage.LoadCommunitySearchPage_VerifyMap(), Is.True);
            testLogger.Information("PASSED: Verified the Community Search page loaded as expected.");
        }

        [Test, Order(21), Description("Verify the NoBrandAp page loads.")]
        public async Task NavigateToNoBrandApPage_CheckForMainElements()
        {
            var page = await GetPageAsync();

            string resourcePath = "/communities/ny/the-NoBrandAp";

            // Navigate to the NoBrandAp page
            await page.GotoDomReadyAsync(BaseUrl + resourcePath);
            CommunityPage_TheNoBrandAp theNoBrandApPage = new(page, testLogger);

            Assert.That(await theNoBrandApPage.LoadNoBrandApPage_VerifyHeading(), Is.True);
            testLogger.Information("PASSED: Verified the NoBrandAp page loaded as expected.");
        }

        [Test, Order(22), Description("Verify the NoBrandEFS page loads.")]
        public async Task NavigateToNoBrandEFSPage_CheckForMainElements()
        {
            var page = await GetPageAsync();

            string resourcePath = "/communities/ny/NoBrand-at-east-56";

            // Navigate to the NoBrandEFS page
            await page.GotoDomReadyAsync(BaseUrl + resourcePath);
            CommunityPage_NoBrandEFS NoBrandEFSPage = new(page, testLogger);

            Assert.That(await NoBrandEFSPage.LoadNoBrandEFSPage_VerifyHeading(), Is.True);
            testLogger.Information("PASSED: Verified the NoBrandEFS page loaded as expected.");
        }

        [Test, Order(23), Description("Verify the NoBrandCap page loads.")]
        public async Task NavigateToNoBrandCapPage_CheckForMainElements()
        {
            var page = await GetPageAsync();

            string resourcePath = "/communities/ny/the-NoBrandCap";

            // Navigate to the NoBrandCap page
            await page.GotoDomReadyAsync(BaseUrl + resourcePath);
            CommunityPage_TheNoBrandCap theNoBrandCapPage = new(page, testLogger);

            Assert.That(await theNoBrandCapPage.LoadNoBrandCapPage_VerifyHeading(), Is.True);
            testLogger.Information("PASSED: Verified the NoBrandCap page loaded as expected.");

        }

        [Test, Order(24), Description("Verify the NoBrandBC page loads.")]
        public async Task NavigateToNoBrandBCCourtPage_CheckForMainElements()
        {
            var page = await GetPageAsync();

            string resourcePath = "/communities/md/NoBrandBC-court";

            // Navigate to the NoBrandBC page
            await page.GotoDomReadyAsync(BaseUrl + resourcePath);
            CommunityPage_NoBrandBCCourt NoBrandBCCourtPage = new(page, testLogger);

            Assert.That(await NoBrandBCCourtPage.LoadNoBrandBCCourtPage_VerifyHeading(), Is.True);
            testLogger.Information("PASSED: Verified the NoBrandBC page loaded as expected.");
        }

        [TearDown]
        public async Task TearDown()
        {
            await TestCleanupAsync();
        }

    }
}

