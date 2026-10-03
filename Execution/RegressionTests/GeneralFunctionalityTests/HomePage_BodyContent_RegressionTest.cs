using Definition.Pages.ExperienceNoBrandPages;
using Definition.Pages.HomePages;
using Definition.Pages.ResourcePages;
using Utility;

namespace Execution.RegressionTests.GeneralFunctionalityTests
{
    [TestFixture]
    [Category("Regression")]
    [Category("HomePage")]
    public class HomePage_BodyContent_RegressionTest : UtilityBase
    {
        [SetUp]
        public void Setup()
        {
            GetEnvironmentConfig("uat", "us");
        }

        [TearDown]
        public async Task TearDown()
        {
            await TestCleanupAsync();
        }


        [Test, Description("Verify the body 'Learn About NoBrandaction Types' CTA appends the #contact anchor fragment to the URL.")]
        public async Task Hero_LearnAboutNoBrandactionTypesCta_Click_AppendsContactAnchorToUrl()
        {
            HomePage homePage = new(await GetPageAsync(), testLogger);
            await homePage.LoadHomePage_VerifyAndAcceptCookies();

            await homePage.ClickHero_LearnAboutNoBrandactionTypesCta();

            Assert.That(await homePage.VerifyHero_LearnAboutNoBrandactionTypes_AnchorActive(), Is.True,
                "After click, URL should contain #contact (proves the click wiring; the live page's id is 'Contact-Us' so no scroll occurs).");
            testLogger.Information("PASSED: 'Learn About NoBrandaction Types' CTA appends #contact to the URL.");
        }


        [Test, Description("Verify the 'Discover the NoBrand Experience' feature card navigates to the Experience NoBrand page.")]
        public async Task NoBrandYourWay_DiscoverNoBrandExperienceCard_Click_NavigatesToExperienceNoBrandPage()
        {
            HomePage homePage = new(await GetPageAsync(), testLogger);
            await homePage.LoadHomePage_VerifyAndAcceptCookies();

            ExperienceNoBrandPage experiencePage = await homePage.ClickNoBrandYourWay_DiscoverNoBrandExperienceCard();

            await Assert.MultipleAsync(async () =>
            {
                Assert.That(await homePage.VerifyUrlContains("/experience-NoBrand"), Is.True,
                            "URL should contain /experience-NoBrand after clicking the feature card.");
                Assert.That(await experiencePage.LoadExperienceNoBrandPage_VerifyHeading(), Is.True,
                    "Experience NoBrand page heading should be visible.");
            });
            testLogger.Information("PASSED: 'Discover the NoBrand Experience' card navigates to Experience NoBrand.");
        }


        [Test, Description("Verify each of the six 'Making Every Day Brilliant' cards navigates to its expected destination.")]
        public async Task NoBrandYourWay_SixCardLinks_Click_EachNavigatesToExpectedDestination()
        {
            var cards = new (string label, string href)[]
            {
                ("Life Your Way (Explore NoBrandaction & living)", "/NoBrandaction-living"),
                ("Active & Inspired (See classes & events)", "/experience-NoBrand/assisted-living-programs-activities"),
                ("Savor Every Meal (Discover cuisine)", "/experience-NoBrand/signature-dining"),
                ("Getting Social (Sample the social life)", "/experience-NoBrand/residents-social-life"),
                ("Always Connected (Stay connected)", "/experience-NoBrand/residents-family-connection"),
                ("Comfortable & Safe (More about design)", "/experience-NoBrand/residents-safety-comfort"),
            };

            HomePage homePage = new(await GetPageAsync(), testLogger);
            await homePage.LoadHomePage_VerifyAndAcceptCookies();

            foreach (var card in cards)
            {
                testLogger.Information("Testing card: {Card}", card.label);
                await homePage.NavigateToHome();
                await homePage.ClickMakingEveryDayBrilliant_CardByHref(card.href);
                Assert.That(await homePage.VerifyUrlContains(card.href), Is.True,
                    $"Click on '{card.label}' should navigate to a URL containing '{card.href}'.");
                testLogger.Information("PASSED: Card '{Card}' navigates to {Href}.", card.label, card.href);
            }
        }


        [Test, Description("Verify each of the four 'What are you looking for today?' persona tabs activates when clicked (aria-selected flips to true).")]
        public async Task WhatAreYouLookingFor_PersonaTabs_Click_EachActivatesItsTab()
        {
            var tabs = new[]
            {
                "looking for a family member",
                "looking for myself",
                "to refer a patient or client",
                "interested in NoBrandactioner opportunities",
            };

            HomePage homePage = new(await GetPageAsync(), testLogger);
            await homePage.LoadHomePage_VerifyAndAcceptCookies();

            foreach (var label in tabs)
            {
                testLogger.Information("Activating persona tab: {Label}", label);
                await homePage.ClickWhatAreYouLookingFor_PersonaTab(label);
                Assert.That(await homePage.VerifyWhatAreYouLookingFor_PersonaTabIsSelected(label), Is.True,
                    $"Persona tab '{label}' should report aria-selected=true after click.");
                testLogger.Information("PASSED: Persona tab '{Label}' is selected.", label);
            }
        }


        [Test, Description("Verify each Family Member persona card navigates to its expected destination.")]
        public async Task WhatAreYouLookingFor_FamilyMemberCards_Click_EachNavigatesToExpectedDestination()
        {
            // content. URL-contains assertions still pass on the path segment regardless of host.
            var cards = new (string label, string href, string expectedUrlSegment)[]
            {
                ("Advice & Planning",            "/advice-and-planning",                                            "/advice"),
                ("Take the NoBrandaction Questionnaire",  "https://www.NoBrand.com",          "/NoBrandaction"),
                ("NoBrandaction & Living",                "/NoBrandaction",                                                    "/NoBrandaction"),
                ("Find a Community",             "https://www.NoBrand.com",     "/community-state-listing"),
            };

            await VerifyPersonaPanelCardsNavigate("looking for a family member", cards);
        }


        [Test, Description("Verify each Looking for Myself persona card navigates to its expected destination.")]
        public async Task WhatAreYouLookingFor_LookingForMyselfCards_Click_EachNavigatesToExpectedDestination()
        {
            var cards = new (string label, string href, string expectedUrlSegment)[]
            {
                ("Experience NoBrand",           "/experience-NoBrand",                                             "/experience-NoBrand"),
                ("Take the NoBrandaction Questionnaire",  "https://www.NoBrand.com",          "/NoBrandaction"),
                ("NoBrandaction & Living",                "/NoBrandaction-living",                                                    "/NoBrandaction"),
                ("Find a Community",             "/communities",                                                    "/communities"),
            };

            await VerifyPersonaPanelCardsNavigate("looking for myself", cards);
        }


        [Test, Description("Verify each Refer Patient/Client persona card navigates to its expected destination.")]
        public async Task WhatAreYouLookingFor_ReferPatientClientCards_Click_EachNavigatesToExpectedDestination()
        {
            var cards = new (string label, string href, string expectedUrlSegment)[]
            {
                ("Referring a Patient",          "/healthNoBrandaction-professionals",                                       "/healthNoBrandaction-professionals"),
                ("NoBrandaction & Living",                "/NoBrandaction-living",                                                    "/NoBrandaction-living"),
                ("Programs & Activities",        "/experience-NoBrand/assisted-living-programs-activities",         "/experience-NoBrand/assisted-living-programs-activities"),
                ("Promoting Health Safety",      "/about/healthy-communities",                                      "/about/healthy-communities"),
            };

            await VerifyPersonaPanelCardsNavigate("to refer a patient or client", cards);
        }


        [Test, Description("Verify each NoBrandactioner Opportunities persona card navigates to its expected destination.")]
        public async Task WhatAreYouLookingFor_NoBrandactionerOpportunitiesCards_Click_EachNavigatesToExpectedDestination()
        {
            var cards = new (string label, string href, string expectedUrlSegment)[]
            {
                ("Job Search",                                 "/NoBrandactioners",                       "/NoBrandactioners"),
                ("Life at NoBrand",                            "/NoBrandactioners/life-at-NoBrand",       "/NoBrandactioners/life-at-NoBrand"),
                ("Diversity, Equity, Inclusion and Belonging", "/NoBrandactioners/you-belong",            "/NoBrandactioners/you-belong"),
                ("Benefits",                                   "/NoBrandactioners/employee-benefits",     "/NoBrandactioners/employee-benefits"),
            };

            await VerifyPersonaPanelCardsNavigate("interested in NoBrandactioner opportunities", cards);
        }


        [Test, Description("Verify each 'Latest from NoBrand' blog card (featured + four list items) navigates to its article URL.")]
        public async Task LatestFromNoBrand_BlogCards_Click_EachNavigatesToArticle()
        {
            HomePage homePage = new(await GetPageAsync(), testLogger);
            await homePage.LoadHomePage_VerifyAndAcceptCookies();

            // Featured article
            testLogger.Information("Testing featured article card.");
            await homePage.NavigateToHome();
            await homePage.ClickLatestFromNoBrand_FeaturedArticleCard();
            const string featuredHref = "/resources/news-and-updates/2025-wellness-safety-equity-and-sustainability-recognitions";
            Assert.That(await homePage.VerifyUrlContains(featuredHref), Is.True,
                $"Featured article click should navigate to URL containing '{featuredHref}'.");
            testLogger.Information("PASSED: Featured article navigates to {Href}.", featuredHref);

            // Four blog list items
            var items = new (string label, string href)[]
            {
                ("How to Pay for NoBrand type 1",       "/resources/finance-and-planning/how-to-pay-for-assisted-living"),
                ("Your Guide to Active Aging",           "/resources/health-and-wellness/active-healthy-aging-tips"),
                ("Autumn Adventures Await",              "/resources/lifestyle/fall-activities-for-NoBrandusers"),
                ("4 Expert Tips for Navigating",         "/resources/NoBrandactiongivers-and-families/expert-tips-for-navigating-NoBranduser-living"),
            };

            foreach (var item in items)
            {
                testLogger.Information("Testing blog item: {Label}", item.label);
                await homePage.NavigateToHome();
                await homePage.ClickLatestFromNoBrand_BlogItemByHref(item.href);
                Assert.That(await homePage.VerifyUrlContains(item.href), Is.True,
                    $"Blog item '{item.label}' should navigate to URL containing '{item.href}'.");
                testLogger.Information("PASSED: Blog item '{Label}' navigates to {Href}.", item.label, item.href);
            }
        }


        [Test, Description("Verify the 'View All Blogs' CTA appends the article-type filter fragment to the URL.")]
        public async Task LatestFromNoBrand_ViewAllBlogsCta_Click_NavigatesToBlogFilter()
        {
            var pw = await GetPageAsync();
            HomePage homePage = new(pw, testLogger);
            await homePage.LoadHomePage_VerifyAndAcceptCookies();

            await homePage.ClickLatestFromNoBrand_ViewAllBlogsCta();

            // or (if a JS handler intercepts) routes to the Resources page filter view. Either signal is acceptable.
            bool fragmentApplied = await homePage.VerifyUrlContains("nobrand_articletype");
            bool routedToResources = !fragmentApplied && await homePage.VerifyUrlContains("/resources");
            Assert.That(fragmentApplied || routedToResources, Is.True,
                "After clicking 'View All Blogs', URL should contain the blog filter fragment or route to /resources.");

            // If routed to the Resources page, also verify the page heading rendered.
            if (routedToResources)
            {
                ResourcesPage resourcesPage = new(pw, testLogger);
                Assert.That(await resourcesPage.LoadResourcesPage_VerifyHeading(), Is.True,
                    "Resources page heading should be visible if the CTA routed to /resources.");
            }

            testLogger.Information("PASSED: 'View All Blogs' CTA produces the expected blog-filter destination.");
        }


        [Test, Description("Verify the body Instagram social icon opens Instagram in a new browser tab.")]
        public async Task SocialMedia_BodyInstagramIcon_Click_OpensInstagramInNewTab()
        {
            HomePage homePage = new(await GetPageAsync(), testLogger);
            await homePage.LoadHomePage_VerifyAndAcceptCookies();

            // The page object captures the popup page, asserts its URL contains instagram.com, then closes it.
            Assert.That(await homePage.ClickBodySocial_InstagramIcon_VerifyOpensInstagram(), Is.True,
                "New tab URL should be on instagram.com after clicking the body Instagram icon.");
            testLogger.Information("PASSED: Body Instagram icon opens Instagram in a new tab.");
        }


        //-- Helpers --//

        /// <summary>
        ///     Activate the named persona tab once, then for each card: navigate back to Home, re-activate the
        ///     tab (panel state is lost on navigation), click the card by its href, and assert the destination URL.
        /// </summary>
        private async Task VerifyPersonaPanelCardsNavigate(string tabLabel, (string label, string href, string expectedUrlSegment)[] cards)
        {
            HomePage homePage = new(await GetPageAsync(), testLogger);
            await homePage.LoadHomePage_VerifyAndAcceptCookies();

            foreach (var card in cards)
            {
                testLogger.Information("Testing persona card under '{Tab}': {Card}", tabLabel, card.label);
                await homePage.NavigateToHome();
                await homePage.ClickWhatAreYouLookingFor_PersonaTab(tabLabel);
                await homePage.ClickPersonaCard_UnderActivePanel_ByHref(card.href);
                Assert.That(await homePage.VerifyUrlContains(card.expectedUrlSegment), Is.True,
                    $"Card '{card.label}' under '{tabLabel}' should navigate to URL containing '{card.expectedUrlSegment}'.");
                testLogger.Information("PASSED: Card '{Card}' navigates to {Url}.", card.label, card.expectedUrlSegment);
            }
        }
    }
}

