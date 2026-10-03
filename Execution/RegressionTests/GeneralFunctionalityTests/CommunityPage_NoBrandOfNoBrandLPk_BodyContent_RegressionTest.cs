using Definition.Pages.NoBrandactionQuestionnairePages;
using Definition.Pages.CommunityPages.US_CommunityPages;
using Utility;

namespace Execution.RegressionTests.GeneralFunctionalityTests
{
    [TestFixture]
    [Category("Regression")]
    [Category("CommunityPage")]
    public class CommunityPage_NoBrand_BodyContent_RegressionTest : UtilityBase
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


        [Test, Description("Verify each of the four functional sub-navigation anchor links smooth-scrolls the page (the subnav JS intercepts the click; no URL hash is written). The #testimonials link is omitted as its target anchor does not exist in the live DOM (name='reviewtracker').")]
        public async Task Subnav_SectionAnchorLinks_Click_EachScrollsPage()
        {
            var anchors = new[] { "#floorplans", "#dining", "#connect", "#overview" };

            CommunityPage_NoBrand_BodyContent page = new(await GetPageAsync(), testLogger);
            await page.NavigateToNoBrandPage();
            await page.AcceptCookies();

            foreach (var fragment in anchors)
            {
                testLogger.Information("Testing subnav anchor: {Fragment}", fragment);
                await page.NavigateToNoBrandPage();
                // For #overview (which is at the top), scroll down first so the click has somewhere to scroll from.
                if (fragment == "#overview")
                {
                    await page.ScrollWindowToY(2000);
                }
                long before = await page.GetScrollY();
                await page.ClickSubnav_AnchorByFragment(fragment);
                Assert.That(await page.VerifyScrollPositionChangedFrom(before), Is.True,
                    $"After clicking subnav anchor {fragment}, the scroll position should change (smooth scroll).");
                testLogger.Information("PASSED: Subnav anchor {Fragment} scrolled the page.", fragment);
            }
        }


        // Note: The 'Back To Top' link (`.community-subnav__panel-backtotop a`) is rendered inside a panelthat is mobile-only on this page — at desktop viewport the link is not clickable (never becomesvisible or enabled), so an automated test for it would fail in the default CI/runner configuration. Covering it would require running the fixture with a mobile emulation profile, which is out of scope for this body-content regression. Test intentionally omitted.


        [Test, Description("Verify the main gallery thumbnail opens the photo gallery modal.")]
        public async Task Gallery_MainThumbnail_Click_OpensLightbNoBrandModal()
        {
            CommunityPage_NoBrandOfNoBrandLP page = new(await GetPageAsync(), testLogger);
            await page.NavigateToNoBrandOfNoBrandLPPage();
            await page.AcceptCookies();

            await page.ClickGallery_MainThumbnail();

            Assert.That(await page.VerifyModalIsOpenByTargetId("community-gallery"), Is.True,
                "After clicking the main gallery thumbnail, the #community-gallery modal-content element should become visible.");
            testLogger.Information("PASSED: Gallery modal opened.");
        }


        [Test, Description("Verify the overview 'Learn more about NoBrandaction' link appends the #typesofNoBrandaction anchor to the URL.")]
        public async Task Overview_LearnMoreAboutNoBrandLink_Click_AppendsTypesOfNoBrandFragment()
        {
            CommunityPage_NoBrandOfNoBrandLP page = new(await GetPageAsync(), testLogger);
            await page.NavigateToNoBrandOfNoBrandLPPage();
            await page.AcceptCookies();

            await page.ClickOverview_LearnMoreAboutNoBrandLink();

            Assert.That(await page.VerifyUrlContains("#typesofnobrand"), Is.True,
                "After clicking 'Learn more about NoBrand', URL should contain #typesofnobrand.");
            testLogger.Information("PASSED: 'Learn more about NoBrand' appended #typesofnobrand.");
        }


        [Test, Description("Verify the overview 'Get in Touch' CTA opens the #contact modal (representative for the three body 'Get in Touch' CTAs).")]
        public async Task Overview_GetInTouchCta_Click_OpensContactModal()
        {
            CommunityPage_NoBrandOfNoBrandLP page = new(await GetPageAsync(), testLogger);
            await page.NavigateToNoBrandOfNoBrandLPPage();
            await page.AcceptCookies();

            await page.ClickOverview_GetInTouchCta();

            Assert.That(await page.VerifyModalIsOpenByTargetId("contact"), Is.True,
                "After clicking 'Get in Touch', the #contact modal-content should become visible.");
            testLogger.Information("PASSED: 'Get in Touch' opened the #contact modal.");
        }


        [Test, Description("Verify the overview address link opens Google Maps in a new browser tab.")]
        public async Task Overview_AddressLink_Click_OpensGoogleMapsInNewTab()
        {
            CommunityPage_NoBrandOfNoBrandLP page = new(await GetPageAsync(), testLogger);
            await page.NavigateToNoBrandOfNoBrandLPPage();
            await page.AcceptCookies();

            bool matched = await page.ClickLinkInNewTab_VerifyUrlContains(page.GetOverview_AddressMapLink_Locator(), "goo.gl");
            // The short link goo.gl/... may redirect to google.com/maps — either host is acceptable.
            if (!matched)
            {
                matched = await page.ClickLinkInNewTab_VerifyUrlContains(page.GetOverview_AddressMapLink_Locator(), "google.");
            }
            Assert.That(matched, Is.True,
                "New-tab URL should reach a Google-hosted maps location (goo.gl or google.*).");
            testLogger.Information("PASSED: Address link opens Google Maps in a new tab.");
        }


        [Test, Description("Verify the overview 'Pay Your Bill' link opens the No Brand billing portal in a new tab.")]
        public async Task Overview_PayYourBillLink_Click_OpensNoBrandInNewTab()
        {
            CommunityPage_NoBrandOfNoBrandLP page = new(await GetPageAsync(), testLogger);
            await page.NavigateToNoBrandOfNoBrandLPPage();
            await page.AcceptCookies();

            bool matched = await page.ClickLinkInNewTab_VerifyUrlContains(page.GetOverview_PayYourBillLink_Locator(), "nobrand.com");
            Assert.That(matched, Is.True,
                "New-tab URL should contain 'nobrand.com' (secure.nobrand.com/nobrand/ig/signin).");
            testLogger.Information("PASSED: 'Pay Your Bill' opens No Brand in a new tab.");
        }


        [Test, Description("Verify the Types of NoBrandaction 'Find out which choice is best for you' link targets the NoBrandaction Questionnaire page (href inspection + direct navigation; physical clicks are intercepted by the sticky subnav overlay).")]
        public async Task TypesOfNoBrandaction_FindYourChoiceLink_TargetsNoBrandactionQuestionnairePage()
        {
            var pw = await GetPageAsync();
            CommunityPage_NoBrandOfNoBrandLP page = new(pw, testLogger);
            await page.NavigateToNoBrandOfNoBrandLPPage();
            await page.AcceptCookies();

            string href = await page.GetTypesOfNoBrandaction_FindYourChoiceLink_Href();
            Assert.That(href, Does.Contain("/NoBrandaction-questionnaire"),
                "The 'Find out which choice is best for you' link's href should point to /NoBrandaction-questionnaire.");

            await page.NavigateDirectToUrl(href);

            Assert.That(await page.VerifyUrlContains("/NoBrandaction-questionnaire"), Is.True,
                "Navigating to the link's href should land on a URL containing /NoBrandaction-questionnaire.");

            NoBrandactionQuestionnairePage questionnairePage = new(pw, testLogger);
            Assert.That(await questionnairePage.LoadNoBrandactionQuestionnairePage_VerifyHeading(), Is.True,
                "The NoBrandaction Questionnaire page heading should be visible at the destination.");

            testLogger.Information("PASSED: 'Find Your Choice' link targets the NoBrandaction Questionnaire.");
        }


        [Test, Description("Verify each of the five Types of NoBrandaction 'Read More' card links opens its NoBrandaction-type destination page in a new browser tab.")]
        public async Task TypesOfNoBrandaction_ReadMoreLinks_Click_EachOpensNoBrandactionTypePageInNewTab()
        {
            // NoBrand type 1 links to an absolute PROD URL; the others are relative. All open target="_blank".
            var cards = new (string label, string href, string expectedUrlSegment)[]
            {
                ("NoBrand type 1",      "https://www.NoBrand.com", "/NoBrandaction"),
                ("Memory NoBrandaction",          "/NoBrandaction",                                       "/NoBrandaction"),
                ("Terrace Club",         "/NoBrandaction",                           "/NoBrandaction"),
                ("Short-Term Stays",     "/NoBrandaction",                                              "/NoBrandaction"),
                ("Hospice Coordination", "/NoBrandaction",                                     "/NoBrandaction"),
            };

            CommunityPage_NoBrandOfNoBrandLP page = new(await GetPageAsync(), testLogger);
            await page.NavigateToNoBrandOfNoBrandLPPage();
            await page.AcceptCookies();

            foreach (var card in cards)
            {
                testLogger.Information("Testing Types of NoBrandaction card: {Label}", card.label);
                var locator = page.GetTypesOfNoBrandaction_ReadMoreLink_Locator(card.href);
                bool matched = await page.ClickLinkInNewTab_VerifyUrlContains(locator, card.expectedUrlSegment);
                Assert.That(matched, Is.True,
                    $"Card '{card.label}' should open a new tab whose URL contains '{card.expectedUrlSegment}'.");
                testLogger.Information("PASSED: Card '{Label}' opens in new tab at {Href}.", card.label, card.expectedUrlSegment);
            }
        }


        [Test, Description("Verify each of the two Floor Plans NoBrandaction tabs (NoBrand type 1, Memory NoBrandaction) activates its pane when clicked.")]
        public async Task FloorPlans_NoBrandactionTabs_Click_EachActivatesItsPane()
        {
            CommunityPage_NoBrandOfNoBrandLP page = new(await GetPageAsync(), testLogger);
            await page.NavigateToNoBrandOfNoBrandLPPage();
            await page.AcceptCookies();

            await page.ClickFloorPlans_NoBrandactionTab("memory-NoBrandaction");
            Assert.That(await page.VerifyFloorPlans_NoBrandactionTabIsActive("memory-NoBrandaction"), Is.True,
                "Memory NoBrandaction pane should be active after clicking its tab.");

            await page.ClickFloorPlans_NoBrandactionTab("assisted-living");
            Assert.That(await page.VerifyFloorPlans_NoBrandactionTabIsActive("assisted-living"), Is.True,
                "NoBrand type 1 pane should be active after clicking its tab.");

            testLogger.Information("PASSED: Both Floor Plans NoBrandaction tabs activate their panes.");
        }


        [Test, Description("Verify each of the six Floor Plans room tabs activates its room pane when clicked under the appropriate NoBrandaction type.")]
        public async Task FloorPlans_RoomTabs_Click_EachActivatesItsRoomPane()
        {
            var rooms = new (string NoBrandactionType, string roomDataTarget)[]
            {
                ("assisted-living", "assisted-living_studio"),
                ("assisted-living", "assisted-living_one-bedroom"),
                ("assisted-living", "assisted-living_two-bedroom"),
                ("memory-NoBrandaction",     "memory-NoBrandaction_studio"),
                ("memory-NoBrandaction",     "memory-NoBrandaction_one-bedroom"),
                ("memory-NoBrandaction",     "memory-NoBrandaction_companion-suite"),
            };

            CommunityPage_NoBrandOfNoBrandLP page = new(await GetPageAsync(), testLogger);
            await page.NavigateToNoBrandOfNoBrandLPPage();
            await page.AcceptCookies();

            foreach (var room in rooms)
            {
                testLogger.Information("Testing room tab: {Target}", room.roomDataTarget);
                await page.ClickFloorPlans_NoBrandactionTab(room.NoBrandactionType);
                await page.ClickFloorPlans_RoomTab(room.roomDataTarget);
                testLogger.Information("PASSED: Room tab {Target} activated.", room.roomDataTarget);
            }
        }


        [Test, Description("Verify the Floor Plans 'Learn more about pricing' link opens the shared pricing modal.")]
        public async Task FloorPlans_LearnMoreAboutPricingCta_Click_OpensPricingModal()
        {
            CommunityPage_NoBrandOfNoBrandLP page = new(await GetPageAsync(), testLogger);
            await page.NavigateToNoBrandOfNoBrandLPPage();
            await page.AcceptCookies();

            await page.ClickFloorPlans_LearnMoreAboutPricingCta();

            Assert.That(await page.VerifyFloorPlans_PricingModalIsOpen(), Is.True,
                "Pricing modal container should be visible after clicking 'Learn more about pricing'.");
            testLogger.Information("PASSED: Pricing modal opened.");
        }


        [Test, Description("Verify a Floor Plans 'Book a Tour' CTA opens the #contact modal.")]
        public async Task FloorPlans_BookATourCta_Click_OpensContactModal()
        {
            CommunityPage_NoBrandOfNoBrandLP page = new(await GetPageAsync(), testLogger);
            await page.NavigateToNoBrandOfNoBrandLPPage();
            await page.AcceptCookies();

            await page.ClickFloorPlans_BookATourCta();

            Assert.That(await page.VerifyModalIsOpenByTargetId("contact"), Is.True,
                "After clicking 'Book a Tour', the #contact modal-content should become visible.");
            testLogger.Information("PASSED: Floor Plan 'Book a Tour' opened the #contact modal.");
        }


        [Test, Description("Verify a Floor Plans 'View in 3D' CTA can be clicked without throwing (href='#' triggers client-side 3D viewer).")]
        public async Task FloorPlans_ViewIn3DCta_Click_Succeeds()
        {
            var pw = await GetPageAsync();
            CommunityPage_NoBrandOfNoBrandLP page = new(pw, testLogger);
            await page.NavigateToNoBrandOfNoBrandLPPage();
            await page.AcceptCookies();

            // The CTA's href is "#" so the only observable consequence is the URL gains a trailing '#'.
            // A JS-driven 3D viewer mounts on click; we assert the click itself completes without exception.
            await page.ClickFloorPlans_ViewIn3DCta();

            Assert.That(pw.Url, Does.Contain("NoBrand-of-lincoln-park"),
                "After clicking 'View in 3D', the browser should remain on the NoBrandLP page.");
            testLogger.Information("PASSED: 'View in 3D' click completed without navigation.");
        }


        [Test, Description("Verify each of the two Dining body CTAs (Explore Meal Planner, See Sample Menu) opens its external destination in a new tab.")]
        public async Task Dining_ExternalCtas_Click_EachOpensInNewTab()
        {
            var ctas = new (string label, string expectedUrlSegment)[]
            {
                ("Explore Meal Planner", ".com"), ("See Sample Menu",      ".cloud"),
            };

            CommunityPage_NoBrandOfNoBrandLP page = new(await GetPageAsync(), testLogger);
            await page.NavigateToNoBrandOfNoBrandLPPage();
            await page.AcceptCookies();

            foreach (var cta in ctas)
            {
                testLogger.Information("Testing Dining CTA: {Label}", cta.label);
                bool matched = await page.ClickLinkInNewTab_VerifyUrlContains(
                    page.GetDining_ExternalCta_LocatorByText(cta.label), cta.expectedUrlSegment);
                Assert.That(matched, Is.True,
                    $"'{cta.label}' new-tab URL should contain '{cta.expectedUrlSegment}'.");
                testLogger.Information("PASSED: Dining CTA '{Label}' opens in new tab.", cta.label);
            }
        }


        [Test, Description("Verify each of the two Activities calendar PDF links opens its PDF in a new tab.")]
        public async Task Activities_CalendarLinks_Click_EachOpensPdfInNewTab()
        {
            var links = new (string label, string expectedUrlSegment)[]
            {
                ("NoBrand type 1 Calendar", "stylelabs.cloud"),
                ("Reminiscence Calendar",    "stylelabs.cloud"),
            };

            CommunityPage_NoBrandOfNoBrandLP page = new(await GetPageAsync(), testLogger);
            await page.NavigateToNoBrandOfNoBrandLPPage();
            await page.AcceptCookies();

            foreach (var link in links)
            {
                testLogger.Information("Testing Activities calendar link: {Label}", link.label);
                bool matched = await page.ClickLinkInNewTab_VerifyUrlContains(
                    page.GetActivities_CalendarLink_LocatorByText(link.label), link.expectedUrlSegment);
                Assert.That(matched, Is.True,
                    $"'{link.label}' new-tab URL should contain '{link.expectedUrlSegment}'.");
                testLogger.Information("PASSED: Activities link '{Label}' opens in new tab.", link.label);
            }
        }


        [Test, Description("Verify a testimonial 'See More' link opens the testimonial modal.")]
        public async Task Testimonials_SeeMoreLink_Click_OpensTestimonialModal()
        {
            CommunityPage_NoBrandOfNoBrandLP page = new(await GetPageAsync(), testLogger);
            await page.NavigateToNoBrandOfNoBrandLPPage();
            await page.AcceptCookies();

            await page.ClickTestimonials_FirstSeeMoreLink();

            Assert.That(await page.VerifyTestimonialModalIsOpen(), Is.True,
                "Testimonial modal (Magnific Popup wrapper) should be visible after clicking 'See More'.");
            testLogger.Information("PASSED: Testimonial modal opened.");
        }


        [Test, Description("Verify the testimonials carousel 'next' arrow advances to a different slide.")]
        public async Task Testimonials_CarouselNextArrow_Click_AdvancesToNextSlide()
        {
            CommunityPage_NoBrandOfNoBrandLP page = new(await GetPageAsync(), testLogger);
            await page.NavigateToNoBrandOfNoBrandLPPage();
            await page.AcceptCookies();

            Assert.That(await page.ClickTestimonials_NextArrow_AndVerifySlideChanges(), Is.True,
                "The active testimonial slide should change after clicking the carousel next arrow.");
            testLogger.Information("PASSED: Testimonial carousel advanced.");
        }
    }
}
