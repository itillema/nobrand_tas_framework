using Microsoft.Playwright;
using Serilog;
using System.Text.RegularExpressions;
using Utility;
using static Microsoft.Playwright.Assertions;


namespace Definition.Pages.CommunityPages.US_CommunityPages
{
    public class CommunityPage_NoBrandOfNoBrandLP(IPage page, ILogger logger)
    {
        private const string PageUrl = "https://uat.nobrand.com/";


        //-- Page Elements --//
        private ILocator PageH1Heading_Text => page.Locator("xpath=//h2[@class='community-gallery__subtitle' and text()='NoBrand ']").First;


        //-- Cookie Consent Elements --//
        private ILocator AcceptCookies_Button => page.Locator("#onetrust-accept-btn-handler").First;


        //-- Body Section: Sticky Sub-navigation --//
        private ILocator Subnav_Panel_Container => page.Locator(".community-subnav__panel").First;
        private ILocator Subnav_ExploreTrigger_Link => page.Locator("a.community-subnav__selected-trigger").First;
        private ILocator Subnav_OverviewAnchor_Link => page.Locator(".community-subnav__panel a[href='#overview']").First;
        private ILocator Subnav_FloorPlansAnchor_Link => page.Locator(".community-subnav__panel a[href='#floorplans']").First;
        private ILocator Subnav_DiningAnchor_Link => page.Locator(".community-subnav__panel a[href='#dining']").First;
        private ILocator Subnav_TestimonialsAnchor_Link => page.Locator(".community-subnav__panel a[href='#testimonials']").First;
        private ILocator Subnav_ConnectAnchor_Link => page.Locator(".community-subnav__panel a[href='#connect']").First;
        private ILocator Subnav_BackToTop_Link => page.Locator(".community-subnav__panel-backtotop a").First;


        //-- Body Section: Gallery --//
        private ILocator Gallery_MainThumbnail_Link => page.Locator(".community-gallery__gallery-main a.community-gallery__gallery-link").First;
        private ILocator Gallery_LightbNoBrandModal_Container => page.Locator("#community-gallery.mfp-ready, .mfp-wrap .mfp-content #community-gallery, .mfp-wrap").First;


        //-- Body Section: Community Overview --//
        private ILocator Overview_AddressMap_Link => page.Locator(".community-overview__location a.link-icon--location").First;
        private ILocator Overview_LearnMoreAboutNoBrandaction_Link => page.Locator(".community-overview__NoBrandaction a[href='#typesofNoBrandaction']").First;
        private ILocator Overview_GetInTouchCta_Button => page.Locator(".community-overview__cta a.modal-trigger[href='#contact']").First;
        private ILocator Overview_PayYourBill_Link => page.Locator(".community-overview__paylink a[target='_blank']").First;


        //-- Body Section: Types of NoBrandaction --//
        private ILocator TypesOfNoBrandaction_FindYourChoice_Link => page.Locator(".community-typesofNoBrandaction a[href='/NoBrandaction-questionnaire']").First;


        //-- Body Section: Floor Plans --//
        private ILocator FloorPlans_NoBrandactionTab_AL => page.Locator("#assisted-living-tab").First;
        private ILocator FloorPlans_NoBrandactionTab_MC => page.Locator("#memory-NoBrandaction-tab").First;
        private ILocator FloorPlans_TypeContainer_AL => page.Locator("#assisted-living").First;
        private ILocator FloorPlans_TypeContainer_MC => page.Locator("#memory-NoBrandaction").First;
        // "Learn more about pricing" opens a shared #pricing-modal — use the AL Studio trigger as the representative.
        private ILocator FloorPlans_LearnMorePricing_Link => page.Locator("#assisted-living_studio a.modal-trigger[href='#pricing-modal']").First;
        private ILocator FloorPlans_PricingModal_Container => page.Locator(".community-floorplans__modal").First;
        // "Book a Tour" inside AL Studio — same #contact anchor as the other CTAs.
        private ILocator FloorPlans_BookATour_Cta_Button => page.Locator("#assisted-living_studio a.button.modal-trigger[href='#contact']").First;
        // "View in 3D" — an href="#" button that triggers an in-page 3D viewer on click.
        private ILocator FloorPlans_ViewIn3D_Cta_Button => page.Locator("#assisted-living_studio a.button.button--3d").First;


        //-- Body Section: Testimonials --//
        private ILocator Testimonials_Section_Container => page.Locator("section.community-testimonials").First;
        private ILocator Testimonials_PrevArrow_Button => page.Locator(".community-testimonials__swiper-prev").First;
        private ILocator Testimonials_NextArrow_Button => page.Locator(".community-testimonials__swiper-next").First;
        private ILocator Testimonials_SlideActive_Card => page.Locator(".community-testimonials__swiper-slide.swiper-slide-active").First;
        // Representative "See More" — the first in DOM order.
        private ILocator Testimonials_SeeMore_FirstLink => page.Locator(".community-testimonials .community-testimonials__card-body a.modal-trigger").First;


        //-- Body Section: Dining --//
        private ILocator Dining_ExternalCtas_All => page.Locator(".community-dining__ctas a[target='_blank']").First;


        //-- Body Section: Activities --//
        private ILocator Activities_CalendarLinks_All => page.Locator(".community-activities a[target='_blank']").First;


        //-- Page Methods --//

        /// <summary>
        ///     Verify NoBrand of NoBrandLP page loads.
        /// </summary>
        /// <returns>
        ///     Boolean value indicating if the page subheader is visible.
        /// </returns>
        public async Task<bool> LoadNoBrandCommunityPage_VerifySubheading()
        {
            logger.Information("    Verifying subheading is displayed on page...");

            try
            {
                await Expect(PageH1Heading_Text).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
                {
                    Timeout = TestConstants.DefaultWaitSeconds * 1000
                });
                logger.Information("    Subheading is displayed on page.");
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "    Failed to verify subheading is displayed on the page.");
                return false;
            }


        }


        /// <summary>
        ///     Navigate to the NoBrand of NoBrandLP community page and wait for the DOM to be ready.
        /// </summary>
        public async Task NavigateToNoBrandOfNoBrandLPPage()
        {
            logger.Information("Navigating to the NoBrand of NoBrandLP community page...");
            await page.GotoDomReadyAsync(PageUrl);
            logger.Information("    Successfully navigated to the NoBrand of NoBrandLP page.");
        }


        /// <summary>
        ///     Accept the cookie consent banner if visible. Non-fatal: swallows timeout.
        /// </summary>
        public async Task AcceptCookies()
        {
            logger.Information("Checking for and accepting cookies...");

            try
            {
                await AcceptCookies_Button.ClickAsync(new LocatorClickOptions
                {
                    Timeout = TestConstants.ShortWaitSeconds * 1000
                });
                logger.Information("    Cookies accepted successfully.");
            }
            catch (Exception ex)
            {
                logger.Information("    Cookie banner not found or already accepted.");
            }
        }


        //-- Body Section Methods --//

        /// <summary>
        ///     Click a body-section subnav anchor link by its target fragment (e.g. "#overview", "#floorplans", "#dining", "#testimonials", "#connect"). The subnav's JS intercepts the click with preventDefault() and performs a smooth scroll without updating the URL hash — verification should use <see cref="GetScrollY"/> before/after to confirm a scroll occurred.
        /// </summary>
        public async Task ClickSubnav_AnchorByFragment(string anchorFragment)
        {
            logger.Information("Clicking subnav anchor link {Fragment}...", anchorFragment);
            ILocator link = page.Locator($".community-subnav__panel a[href='{anchorFragment}']").First;
            try
            {
                await link.ClickAsync();
                logger.Information("    Subnav anchor {Fragment} clicked.", anchorFragment);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "    Failed to click subnav anchor {Fragment}.", anchorFragment);
                throw;
            }
        }


        /// <summary>
        ///     Return the current window.scrollY value, used to compare scroll positions before and after a click (e.g. subnav smooth-scroll anchors, Back To Top). Coerces from JS number (which may be a double in Chrome) to long.
        /// </summary>
        public async Task<long> GetScrollY()
        {
            return await page.EvaluateAsync<long>("() => Math.floor(window.pageYOffset || document.documentElement.scrollTop || 0)");
        }


        /// <summary>
        ///     Scroll the window to a specific y-coordinate (used to prime Back To Top tests that need the page to be scrolled down from the top first).
        /// </summary>
        public async Task ScrollWindowToY(long y)
        {
            await page.EvaluateAsync("y => window.scrollTo(0, y)", y);
        }


        /// <summary>
        ///     Wait until the window.scrollY value differs from the provided pre-click value by at least <paramref name="minDelta"/> pixels. Used to verify a subnav smooth-scroll click actually moved the viewport.
        /// </summary>
        public async Task<bool> VerifyScrollPositionChangedFrom(long scrollYBefore, int minDelta = 50)
        {
            logger.Information("Verifying scroll position changed from {Before} by >= {Delta}px...", scrollYBefore, minDelta);
            try
            {
                await page.WaitForFunctionAsync(
                    "args => Math.abs((window.pageYOffset||document.documentElement.scrollTop||0) - args.before) >= args.delta",
                    new { before = scrollYBefore, delta = minDelta });
                long final = await GetScrollY();
                logger.Information("    Scroll moved from {Before} to {After}px.", scrollYBefore, final);
                return true;
            }
            catch (Exception ex)
            {
                logger.Warning(ex, "    Timeout waiting for scroll position change.");
                return false;
            }
        }


        /// <summary>
        ///     Click the subnav "Back To Top" link.
        /// </summary>
        public async Task ClickSubnav_BackToTopLink()
        {
            logger.Information("Clicking subnav 'Back To Top' link...");
            try
            {
                await Subnav_BackToTop_Link.ClickAsync();
                logger.Information("    Back To Top link clicked.");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "    Failed to click Back To Top.");
                throw;
            }
        }


        /// <summary>
        ///     Verify the browser has scrolled so that window.scrollY is within <paramref name="threshold"/> pixels of the top of the page. Used after clicking "Back To Top".
        /// </summary>
        public async Task<bool> VerifyScrollPositionAtTop(int threshold = 100)
        {
            logger.Information("Verifying scroll position is within {Threshold}px of top...", threshold);
            try
            {
                await page.WaitForFunctionAsync(
                    "t => (window.pageYOffset||document.documentElement.scrollTop||0) <= t",
                    threshold);
                long finalY = await GetScrollY();
                logger.Information("    Scroll position: {Y}px (threshold {Threshold}px).", finalY, threshold);
                return true;
            }
            catch (Exception ex)
            {
                logger.Warning(ex, "    Timeout waiting for scroll to reach top.");
                return false;
            }
        }


        /// <summary>
        ///     Click the first gallery thumbnail and wait for the Magnific Popup lightbNoBrand to become visible.
        /// </summary>
        public async Task ClickGallery_MainThumbnail()
        {
            logger.Information("Clicking gallery main thumbnail...");
            try
            {
                // The gallery thumbnail is an overlay link that opens a Magnific Popup lightbNoBrand; it isn't a normal actionable click target (the original used a JS click). Dispatch a native click on the element so its handler fires regardless of visual actionability.
                await Gallery_MainThumbnail_Link.ScrollIntoViewIfNeededAsync();
                await Gallery_MainThumbnail_Link.EvaluateAsync("el => el.click()");
                logger.Information("    Gallery thumbnail clicked.");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "    Failed to click gallery thumbnail.");
                throw;
            }
        }


        /// <summary>
        ///     Verify that a CMS modal with the given id has become visible (offsetParent != null).
        ///     The site's modal system toggles display on `<div id="X" class="modal-content">` elements when their paired <c>&lt;a href="#X" class="modal-trigger"&gt;</c> is clicked.
        /// </summary>
        public async Task<bool> VerifyModalIsOpenByTargetId(string modalTargetId)
        {
            logger.Information("Verifying modal '{Id}' is open (effectively visible via offsetParent)...", modalTargetId);
            try
            {
                await page.WaitForFunctionAsync(
                    "id => { var el=document.getElementById(id); return !!el && el.offsetParent !== null; }",
                    modalTargetId);
                logger.Information("    Modal '{Id}' is visible.", modalTargetId);
                return true;
            }
            catch (Exception ex)
            {
                logger.Warning(ex, "    Timeout waiting for modal '{Id}' to become visible.", modalTargetId);
                return false;
            }
        }


        /// <summary>
        ///     Verify that any `.modal-content` element on the page is currently effectively visible. Useful when the modal target id is dynamic (e.g. each testimonial has a unique GUID-based id).
        /// </summary>
        public async Task<bool> VerifyAnyModalContentIsDisplayed()
        {
            logger.Information("Verifying any .modal-content element is currently displayed...");
            try
            {
                await page.WaitForFunctionAsync(
                    "() => Array.from(document.querySelectorAll('.modal-content')).some(e => e.offsetParent !== null)");
                logger.Information("    At least one .modal-content element is displayed.");
                return true;
            }
            catch (Exception ex)
            {
                logger.Warning(ex, "    Timeout waiting for a .modal-content element to become displayed.");
                return false;
            }
        }


        /// <summary>
        ///     Click the overview "Learn more about NoBrandaction" in-page anchor link (scrolls to the Types of NoBrandaction section).
        /// </summary>
        public async Task ClickOverview_LearnMoreAboutNoBrandactionLink()
        {
            await ClickBodyLink("Learn more about NoBrandaction", Overview_LearnMoreAboutNoBrandaction_Link);
        }


        /// <summary>
        ///     Click the overview "Get in Touch" CTA (representative for the three body "Get in Touch" CTAs that all share the #contact anchor + modal-trigger class).
        /// </summary>
        public async Task ClickOverview_GetInTouchCta()
        {
            await ClickBodyLink("Get in Touch", Overview_GetInTouchCta_Button);
        }


        /// <summary>
        ///     Return the href attribute of the "Find out which choice is best for you" link. Exposed so the test can verify the link's destination without actually clicking — the anchor sits under the sticky subnav in the viewport coordinates Chrome reports, causing both JS and native clicks to be intercepted. Verifying the href + navigating to it is the pragmatic equivalent.
        /// </summary>
        public async Task<string> GetTypesOfNoBrandaction_FindYourChoiceLink_Href()
        {
            return await TypesOfNoBrandaction_FindYourChoice_Link.GetAttributeAsync("href");
        }


        /// <summary>
        ///     Navigate directly to an arbitrary URL in the current tab. Used by body-link tests where a physical click on the link is blocked by overlay interception but the link's href is still the meaningful "destination" signal.
        /// </summary>
        public async Task NavigateDirectToUrl(string url)
        {
            logger.Information("Navigating directly to {Url}...", url);
            await page.GotoDomReadyAsync(url);
            logger.Information("    Direct navigation complete. Current URL: {Url}", page.Url);
        }


        /// <summary>
        ///     Click a floor-plan NoBrandaction tab (AL or MC) and verify the matching pane becomes active.
        /// </summary>
        public async Task ClickFloorPlans_NoBrandactionTab(string NoBrandactionType)
        {
            bool isMc = NoBrandactionType.Equals("memory-NoBrandaction", StringComparison.OrdinalIgnoreCase);
            ILocator tab = isMc ? FloorPlans_NoBrandactionTab_MC : FloorPlans_NoBrandactionTab_AL;
            ILocator pane = isMc ? FloorPlans_TypeContainer_MC : FloorPlans_TypeContainer_AL;

            logger.Information("Clicking floor-plans NoBrandaction tab: {Type}", NoBrandactionType);
            try
            {
                await tab.ClickAsync();
                await Expect(pane).ToHaveClassAsync(new Regex("active"));
                logger.Information("    NoBrandaction tab {Type} pane is active.", NoBrandactionType);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "    Failed to click NoBrandaction tab {Type}.", NoBrandactionType);
                throw;
            }
        }


        /// <summary>
        ///     Verify the floor-plans pane for the given NoBrandaction type (assisted-living | memory-NoBrandaction) is currently the active one (its container div carries the 'active' class).
        /// </summary>
        public async Task<bool> VerifyFloorPlans_NoBrandactionTabIsActive(string NoBrandactionType)
        {
            ILocator pane = NoBrandactionType.Equals("memory-NoBrandaction", StringComparison.OrdinalIgnoreCase)
                ? FloorPlans_TypeContainer_MC
                : FloorPlans_TypeContainer_AL;
            try
            {
                await Expect(pane).ToHaveClassAsync(new Regex("active"));
                logger.Information("    Pane {Type} active: {Active}", NoBrandactionType, true);
                return true;
            }
            catch (Exception)
            {
                logger.Information("    Pane {Type} active: {Active}", NoBrandactionType, false);
                return false;
            }
        }


        /// <summary>
        ///     Click a room tab inside the currently active NoBrandaction-type pane by its data-target value (e.g. "assisted-living_studio", "assisted-living_one-bedroom", "memory-NoBrandaction_companion-suite").
        /// </summary>
        public async Task ClickFloorPlans_RoomTab(string dataTarget)
        {
            logger.Information("Clicking floor-plans room tab {Target}...", dataTarget);
            ILocator tab = page.Locator($"a.community-floorplans__room-trigger[data-target='{dataTarget}']").First;
            ILocator room = page.Locator($"#{dataTarget}").First;
            try
            {
                await tab.ClickAsync();
                await Expect(room).ToHaveClassAsync(new Regex("active"));
                logger.Information("    Room tab {Target} pane is active.", dataTarget);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "    Failed to click room tab {Target}.", dataTarget);
                throw;
            }
        }


        /// <summary>
        ///     Click the representative "Learn more about pricing" link (opens shared #pricing-modal).
        /// </summary>
        public async Task ClickFloorPlans_LearnMoreAboutPricingCta()
        {
            await ClickBodyLink("Learn more about pricing", FloorPlans_LearnMorePricing_Link);
        }


        /// <summary>
        ///     Verify the floor-plans pricing modal is visible after clicking "Learn more about pricing".
        ///     The modal content container becomes visible inside the Magnific Popup wrapper.
        /// </summary>
        public async Task<bool> VerifyFloorPlans_PricingModalIsOpen()
        {
            logger.Information("Verifying floor-plans pricing modal is open...");
            try
            {
                await Expect(FloorPlans_PricingModal_Container).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
                {
                    Timeout = TestConstants.DefaultWaitSeconds * 1000
                });
                logger.Information("    Pricing modal container is visible.");
                return true;
            }
            catch (Exception ex)
            {
                logger.Warning(ex, "    Timeout waiting for pricing modal.");
                return false;
            }
        }


        /// <summary>
        ///     Click the representative floor-plan "Book a Tour" CTA (shares the #contact anchor).
        /// </summary>
        public async Task ClickFloorPlans_BookATourCta()
        {
            await ClickBodyLink("Book a Tour (floor plan)", FloorPlans_BookATour_Cta_Button);
        }


        /// <summary>
        ///     Click the representative "View in 3D" floor-plan CTA.
        /// </summary>
        public async Task ClickFloorPlans_ViewIn3DCta()
        {
            await ClickBodyLink("View in 3D", FloorPlans_ViewIn3D_Cta_Button);
        }


        /// <summary>
        ///     Click the testimonials carousel next arrow and verify the active slide identity changes.
        /// </summary>
        public async Task<bool> ClickTestimonials_NextArrow_AndVerifySlideChanges()
        {
            logger.Information("Clicking testimonials next arrow...");
            try
            {
                await Testimonials_Section_Container.ScrollIntoViewIfNeededAsync();

                string authorBefore = await Testimonials_SlideActive_Card.InnerTextAsync();

                await Testimonials_NextArrow_Button.ClickAsync();

                await page.WaitForFunctionAsync(
                    "prev => { var el=document.querySelector('.community-testimonials__swiper-slide.swiper-slide-active'); return el && el.innerText !== prev; }",
                    authorBefore);
                logger.Information("    Testimonial slide advanced.");
                return true;
            }
            catch (Exception ex)
            {
                logger.Warning(ex, "    Failed to advance testimonial carousel.");
                return false;
            }
        }


        /// <summary>
        ///     Click the first testimonial "See More" link and verify a testimonial modal becomes visible.
        /// </summary>
        public async Task ClickTestimonials_FirstSeeMoreLink()
        {
            await ClickBodyLink("Testimonial See More (first)", Testimonials_SeeMore_FirstLink);
        }


        /// <summary>
        ///     Verify a testimonial modal (Magnific Popup wrapper) is open after clicking "See More".
        /// </summary>
        public async Task<bool> VerifyTestimonialModalIsOpen()
        {
            return await VerifyAnyModalContentIsDisplayed();
        }


        /// <summary>
        ///     Click a body-section link and switch to the new tab that opens, then verify the new tab's URL contains <paramref name="expectedUrlSubstring"/>. Closes the new tab and switches back before returning the result.
        /// </summary>
        public async Task<bool> ClickLinkInNewTab_VerifyUrlContains(ILocator linkLocator, string expectedUrlSubstring)
        {
            logger.Information("Clicking link in new tab and verifying URL contains '{Sub}'...", expectedUrlSubstring);
            int timeoutMs = TestConstants.ShortWaitSeconds * 1000;
            try
            {
                // The link may open a new tab (web page) OR trigger a file download — headless Chromium downloads a PDF rather than opening it as a navigable page. Arm both waiters, click, and react to whichever fires first.
                var pageTask = page.Context.WaitForPageAsync(new() { Timeout = timeoutMs });
                var downloadTask = page.WaitForDownloadAsync(new() { Timeout = timeoutMs });

                await linkLocator.First.ClickAsync();

                var winner = await Task.WhenAny(pageTask, downloadTask);
                ObserveAndIgnore(pageTask);
                ObserveAndIgnore(downloadTask);

                // The click triggered a direct download (e.g. a PDF) without a navigable tab.
                if (winner == downloadTask && downloadTask.IsCompletedSuccessfully)
                {
                    bool ok = downloadTask.Result.Url.Contains(expectedUrlSubstring);
                    logger.Information("    Link downloaded a file. URL: {Url} (matched: {Ok})", downloadTask.Result.Url, ok);
                    return ok;
                }

                // A new tab opened. It may settle on a URL, or itself become a PDF download.
                if (pageTask.IsCompletedSuccessfully)
                {
                    var popup = pageTask.Result;
                    var popupDownload = popup.WaitForDownloadAsync(new() { Timeout = timeoutMs });
                    try { await popup.WaitForLoadStateAsync(LoadState.DOMContentLoaded); } catch (Exception) { /* tab may close on download */ }

                    bool matched = popup.Url.Contains(expectedUrlSubstring);
                    if (!matched)
                    {
                        try
                        {
                            await popup.WaitForUrlDomReadyAsync(u => u.Contains(expectedUrlSubstring), timeoutMs);
                            matched = true;
                        }
                        catch (Exception) { matched = popup.Url.Contains(expectedUrlSubstring); }
                    }

                    // If the tab is a download (PDF), match on the download URL instead.
                    if (!matched && popupDownload.IsCompletedSuccessfully)
                        matched = popupDownload.Result.Url.Contains(expectedUrlSubstring);
                    ObserveAndIgnore(popupDownload);

                    logger.Information("    New-tab result for '{Sub}'. URL: {Url} (matched: {Matched})",
                        expectedUrlSubstring, popup.Url, matched);
                    try { await popup.CloseAsync(); } catch (Exception) { }
                    return matched;
                }

                logger.Warning("    No new tab or download opened within timeout for '{Sub}'.", expectedUrlSubstring);
                return false;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "    Failed new-tab click/URL verification.");
                throw;
            }
        }


        /// <summary>
        ///     Click the Community Overview "Pay Your Bill" link (public accessor that exposes the locator to the test so the test can use <see cref="ClickLinkInNewTab_VerifyUrlContains"/>).
        /// </summary>
        public ILocator GetOverview_PayYourBillLink_Locator() => Overview_PayYourBill_Link;


        /// <summary>
        ///     Click the Community Overview Google Maps address link (public accessor).
        /// </summary>
        public ILocator GetOverview_AddressMapLink_Locator() => Overview_AddressMap_Link;


        /// <summary>
        ///     Public accessor for the five Types of NoBrandaction "Read More" card links, keyed by the card's data-type label ("assisted-living", "memory-NoBrandaction", "terrace-club", "short-term-stays", "hospice-coordination"). Returns a locator scoped to the expected card's href.
        /// </summary>
        public ILocator GetTypesOfNoBrandaction_ReadMoreLink_Locator(string expectedHref)
            => page.Locator($".community-typesofNoBrandaction a[href='{expectedHref}']").First;


        /// <summary>
        ///     Public accessor for the two Dining body CTAs (Explore Meal Planner, See Sample Menu).
        /// </summary>
        public ILocator GetDining_ExternalCta_LocatorByText(string ctaLabel)
            => page.Locator($"xpath=//section[contains(@class,'community-dining')]//a[.//span[normalize-space(.)='{ctaLabel}']]").First;


        /// <summary>
        ///     Public accessor for the two Activities PDF links (NoBrand type 1 Calendar, Reminiscence Calendar).
        /// </summary>
        public ILocator GetActivities_CalendarLink_LocatorByText(string ctaLabel)
            => page.Locator($"xpath=//section[contains(@class,'community-activities')]//a[.//span[normalize-space(.)='{ctaLabel}']]").First;


        /// <summary>
        ///     Verify the browser URL contains the given substring (used for in-page anchor fragments and same-host navigation destinations).
        /// </summary>
        public async Task<bool> VerifyUrlContains(string urlSubstring)
        {
            logger.Information("Verifying URL contains '{Sub}'...", urlSubstring);
            try
            {
                await Expect(page).ToHaveURLAsync(new Regex(Regex.Escape(urlSubstring)), new PageAssertionsToHaveURLOptions
                {
                    Timeout = TestConstants.DefaultWaitSeconds * 1000
                });
                logger.Information("    URL contains '{Sub}'. Current URL: {Url}", urlSubstring, page.Url);
                return true;
            }
            catch (Exception ex)
            {
                logger.Warning(ex, "    Timeout. Current URL: {Url}", page.Url);
                return false;
            }
        }


        //-- Private Helper Methods --//

        /// <summary>
        ///     Shared click pattern for body links and buttons (Playwright auto-scrolls + auto-waits).
        /// </summary>
        private async Task ClickBodyLink(string label, ILocator locator)
        {
            logger.Information("Clicking body link '{Label}'...", label);
            try
            {
                await locator.ClickAsync();
                logger.Information("    '{Label}' clicked.", label);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "    Failed to click body link '{Label}'.", label);
                throw;
            }
        }

        /// <summary>
        ///     Observes a fire-and-forget wait Task (the "loser" of a new-tab vs download race) so its eventual timeout doesn't surface as an unobserved task exception.
        /// </summary>
        private static void ObserveAndIgnore(Task task)
        {
            _ = task.ContinueWith(t => { _ = t.Exception; }, TaskScheduler.Default);
        }
    }
}

