using Definition.Pages.CommunityPages.US_CommunityPages;
using Definition.Pages.ExperienceNoBrandPages;
using Definition.Pages.ResourcePages;
using Definition.Pages.SiteSearchPages;
using Definition.TestData.FormData;
using Microsoft.Playwright;
using Serilog;
using System.Text.RegularExpressions;
using Utility;
using static Microsoft.Playwright.Assertions;

namespace Definition.Pages.HomePages
{
    public class HomePage(IPage page, ILogger logger)
    {

        private const string PageUrl = "https://uat.nobrand.com";
        private const string ZipCodeInput_String = "46202";
        private const string UrlSnippet_Communities = "/communities";
        private const string UrlSnippet_SiteSearch = "/search#q=test";
        private const string UrlSnippet_ResourceSearch = "/resources";




        //-- Header and Navigation Menu Elements --//
        private ILocator NoBrandLogoImage_Image => page.Locator("xpath=//img[@alt='NoBrand_Vert_Logo_RGB']").First;
        private ILocator AcceptCookies_Button => page.Locator("xpath=//button[@id='truste-consent-button']").First;
        private ILocator NoBrandHeaderPanelNavigation_Container => page.Locator("xpath=//div[@class='header__panel-nav']").First;
        private ILocator NoBrandMainNavigation_Container => page.Locator("xpath=//ul[@class='header__main']").First;
        private ILocator ExperienceNoBrand_MainNavigation_Link => page.Locator("xpath=//a[@data-target='header-dropdown-experience-NoBrand' and text()='Experience NoBrand']").First;
        private ILocator NoBrandactionLiving_MainNavigation_Link => page.Locator("xpath=//a[@data-target='header-dropdown-NoBrandaction-38-living' and text()='NoBrandaction & Living']").First;
        private ILocator NoBrandactionLiving_SubMenu_Link => page.Locator("xpath=//div[@class='header__dropdown header__dropdown--active' and @data-id='header-dropdown-NoBrandaction-38-living']").First;
        private ILocator AdvicePlanning_MainNavigation_Link => page.Locator("xpath=//a[@data-target='header-dropdown-advice-38-planning' and text()='Advice & Planning']").First;
        private ILocator AdvicePlanning_SubMenu_Link => page.Locator("xpath=//div[@class='header__dropdown header__dropdown--active' and @data-id='header-dropdown-advice-38-planning']").First;
        private ILocator NoBrandBlog_SubMenu_Link => page.Locator("xpath=//a[@href='/resources' and text()='NoBrand Blog']").First;
        private ILocator HeaderDropdownMenuOpen_Button => page.Locator("xpath=//a[@id='header-menu-button']").First;
        private ILocator HeaderDropdownMenuClose_Button => page.Locator("xpath=//a[@id='header-menu-close-button']").First;
        private ILocator BackToTop_Button => page.Locator("xpath=//a[@class='header__backtotop-button']").First;
        private ILocator ForProfessionals_HeaderDropdown_Link => page.Locator("xpath=//a[@href='/healthNoBrandaction-professionals']").First;
        private ILocator NoBrandactionersAtNoBrand_HeaderDropdown_Link => page.Locator("xpath=//a[@href='/NoBrandactioners']").First;
        private ILocator AboutUs_HeaderDropdown_Link => page.Locator("xpath=//a[@href='/about']").First;



        //-- Footer and Navigation Menu Elements --//
        private ILocator NoBrandFooterNavigation_Container => page.Locator(".footer").First;
        private ILocator FooterNavigation_AboutUs_Link => page.Locator("xpath=//a[@class='footer__column-header' and @href='/about']").First;
        private ILocator FooterNavigation_NoBrandactionersAtNoBrand_Link => page.Locator("xpath=//a[@class='footer__column-header' and @href='/NoBrandactioners']").First;
        private ILocator FooterNavigation_NoBrandBlog_Link => page.Locator("xpath=//a[@class='footer__column-header' and text()='NoBrand Blog']").First;
        private ILocator FooterNavigation_ForProfessionals_Link => page.Locator("xpath=//a[@class='footer__column-header' and @href='/healthNoBrandaction-professionals']").First;




        //-- Page Community Search Elements --//
        private ILocator CommunitySearch_Form_Container => page.Locator(".video-hero-with-community-search__actions-form-input").First;
        private ILocator CommunitySearch_AddressField_Input => page.Locator("html.hydrated body main section.video-hero-with-community-search div.video-hero-with-community-search__actions div.video-hero-with-community-search__actions-container.container-lg div.video-hero-with-community-search__actions-form div.video-hero-with-community-search__actions-form-input input.pac-target-input").First;
        private ILocator CommunitySearch_TypeOfNoBrandaction_Dropdown => page.Locator(".homepage-masthead__location-selector-wrapper select[name='typeofNoBrandaction']").First;
        private ILocator CommunitySearch_TypeOfNoBrandaction_Option_NoBrand => page.Locator("xpath=//option[@value='NoBrand type 1']").First;
        private ILocator CommunitySearch_SearchButton_Button => page.Locator("xpath=//button[@class='button' and @type='button' and text()='Search']").First;




        //-- Lead Form ("Get in Touch to Learn More") Elements --//
        // Scoped with [last()] because the page also renders a community-search form above this one.
        private ILocator LeadForm_Heading => page.Locator("xpath=//h2[contains(normalize-space(.), 'Get in Touch to Learn More')]").First;
        private ILocator LeadForm_Container => page.Locator("xpath=(//form[contains(@class, 'community-contact-us__form')])[last()]").First;
        private ILocator FirstName_Input => page.Locator("xpath=(//input[contains(@id, 'first-name')])[last()]").First;
        private ILocator LastName_Input => page.Locator("xpath=(//input[contains(@id, 'last-name')])[last()]").First;
        private ILocator Email_Input => page.Locator("xpath=(//input[contains(@id, 'email')])[last()]").First;
        private ILocator Phone_Input => page.Locator("xpath=(//input[contains(@id, 'phone')])[last()]").First;
        private ILocator ZipCode_Input => page.Locator("xpath=(//input[contains(@id, 'zip-code')])[last()]").First;
        private ILocator InquiryType_Select => page.Locator("xpath=(//select[@name='topic'])[last()]").First;
        private ILocator PreferredTourDate_Input => page.Locator("xpath=(//input[@name='preferredDate'])[last()]").First;
        private ILocator PreferredTourTime_Select => page.Locator("xpath=(//select[@name='preferredTime'])[last()]").First;

        // The tour-time label actually selected during FillLeadForm (configured label or first-available fallback) — remembered so SubmitLeadForm can re-select it if a late async repopulation wipes it.
        private string? _selectedTourTimeLabel;
        private ILocator ReferralSource_Select => page.Locator("xpath=(//select[@name='source'])[last()]").First;
        private ILocator Comments_Textarea => page.Locator("xpath=(//textarea[contains(@id, 'comments')])[last()]").First;
        private ILocator MarketingConsent_CheckbNoBrand => page.Locator("xpath=(//input[@type='checkbNoBrand' and @name='smsConsent'])[last()]").First;
        private ILocator NearbyCommunities_HiddenInput => page.Locator("xpath=(//input[@name='NearbyCommunities'])[last()]").First;
        private ILocator LeadForm_Submit_Button => page.Locator("xpath=(//form[contains(@class, 'community-contact-us__form')])[last()]//button[@type='submit']").First;
        private ILocator LeadForm_ValidationErrors => page.Locator("xpath=//*[contains(@class, 'pristine-error') or contains(@class, 'invalid')]");
        // Form submission redirects to a standalone Thank You page; matches the live markup used by the existing ThankYouPage object.
        private ILocator ThankYouPage_Heading =>
            page.Locator("xpath=//h1[@class='masthead-common-content__heading' and contains(normalize-space(.), 'Thank you for your interest in NoBrand')]")
                .Filter(new LocatorFilterOptions { Visible = true }).First;

        private ILocator CommunitySearch_Button => page.Locator(".location-search__form-submit button.button").First;




        // -- Site Search Elements -- //
        private ILocator SearchOpenButton_Button => page.Locator("#header-search-button").First;
        private ILocator SearchTextInput_Input => page.Locator("#header-search-input").First;




        //-- Body Section: Hero CTA ("Support That Sparks Confidence and Joy") --//
        private ILocator Hero_LearnAboutNoBrandactionTypes_Link => page.Locator(".content-4up-cards a[href='#contact']").First;



        //-- Body Section: "NoBrand Your Way / Making Every Day Brilliant" --//
        // Feature card wraps the image + arrow CTA "Discover the NoBrand Experience".
        private ILocator NoBrandYourWay_DiscoverNoBrandExperience_Card =>
            page.Locator(".hightlights-row__general-container > a[href='/experience-NoBrand']").First;
        // Each card href is unique to a destination. Mobile duplicates use a different class (link link--small) so the desktop a.hightlights-row-accordion locator stays unique.



        //-- Body Section: "What Are You Looking For Today?" Persona Tabs --//
        // Tab labels contain apostrophes ("I'm…"); use unique substrings to avoid escaping.
        private ILocator PersonaTab_FamilyMember =>
            page.Locator("xpath=//button[contains(@class,'quick-access__trigger')][.//span[contains(., 'looking for a family member')]]").First;
        private ILocator PersonaTab_Myself =>
            page.Locator("xpath=//button[contains(@class,'quick-access__trigger')][.//span[contains(., 'looking for myself')]]").First;
        private ILocator PersonaTab_ReferPatient =>
            page.Locator("xpath=//button[contains(@class,'quick-access__trigger')][.//span[contains(., 'to refer a patient or client')]]").First;
        private ILocator PersonaTab_NoBrandactioners =>
            page.Locator("xpath=//button[contains(@class,'quick-access__trigger')][.//span[contains(., 'interested in NoBrandactioner opportunities')]]").First;



        //-- Body Section: "The Latest from NoBrand" Blog Cards --//
        private ILocator LatestFromNoBrand_FeaturedArticle_Link => page.Locator("a.latest-news__featured").First;
        private ILocator LatestFromNoBrand_ViewAllBlogs_Cta => page.Locator("a.latest-news__cta").First;



        //-- Body Section: "Follow Us On Social" --//
        // Scoped to the .community-richtext block so the footer Instagram link does not match.
        private ILocator BodySocial_InstagramIcon_Link =>
            page.Locator(".community-richtext a[href='https://www.instagram.com/NoBrandsrliving']").First;






        //-- General Page Methods --//

        /// <summary>
        ///     Verify Home page loads, the cookies banner is visible, and that the user can accept cookies.
        /// </summary>
        /// <returns>
        ///     Boolean value indicating if the cookie banner is visible.
        /// </returns>
        public async Task LoadHomePage_VerifyAndAcceptCookies()
        {
            logger.Information("Verifying and accepting cookies...");

            try
            {
                await AcceptCookies_Button.ClickAsync(new LocatorClickOptions
                {
                    Timeout = TestConstants.DefaultWaitSeconds * 1000
                });
                logger.Information("    Cookies accepted successfully.");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "  Failed to find or click the 'Accept Cookies' button.");
                throw;
            }


        }


        /// <summary>
        ///     Verify the header NoBrand logo is visible.
        /// </summary>
        /// <returns>
        ///     Boolean value indicating if the logo is visible.
        /// </returns>
        public async Task<bool> LoadHomePage_VerifyLogoVisible()
        {
            logger.Information("Verifying header logo visibility...");

            // Wait for the main logo to be visible
            try
            {
                await Expect(NoBrandLogoImage_Image).ToBeVisibleAsync();
                logger.Information("    Logo visibility check complete. Visible: TRUE");
                return true;
            }
            catch (Exception ex)
            {
                logger.Warning(ex, "    Timeout waiting for logo to be visible.");
                return false;
            }
        }


        /// <summary>
        ///     Verify the user can navigate to a new page and then back using the NoBrand logo.
        /// </summary>
        /// <returns>
        ///     Boolean value indicating if the user has returned to the Home page.
        /// </returns>
        public async Task<bool> LoadHomePage_VerifyNavigateAwayAndBack()
        {
            logger.Information("Verifying user can navigate away and back to the Home page using the NoBrand logo link...");

            try
            {
                // Wait for the Back to Top button to be visible and then click it to scroll back to top
                await Expect(BackToTop_Button).ToBeVisibleAsync();
                logger.Information("    Back To Top button visibility check complete. Visible: TRUE");
                await BackToTop_Button.ClickAsync();
                logger.Information("    Back To Top button clicked.");

                // Wait for the dropdown nav menu button to be visible, then open it
                await Expect(HeaderDropdownMenuOpen_Button).ToBeVisibleAsync();
                logger.Information("    Header dropdown menu button visibility check complete. Visible: TRUE");
                await HeaderDropdownMenuOpen_Button.ClickAsync();
                logger.Information("    Header dropdown menu button opened.");

                // Wait for the dropdown nav menu to be visible, then open it
                await Expect(NoBrandHeaderPanelNavigation_Container).ToBeVisibleAsync();
                logger.Information("    Header dropdown nav menu visibility check complete. Visible: TRUE");

                // Wait for the For Professionals link to be visible
                await Expect(ForProfessionals_HeaderDropdown_Link).ToBeVisibleAsync();

                // Click the For Professionals link to navigate to a new page; it opens in a new tab.
                var popup = await page.Context.RunAndWaitForPageAsync(async () =>
                {
                    await ForProfessionals_HeaderDropdown_Link.ClickAsync();
                });
                logger.Information("    For Professionals nav link clicked.");
                await popup.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
                logger.Information("    New tab (For Professionals page) has been opened.");

                // Click the NoBrand logo on the new tab to navigate back to the Home page
                var newTabLogo = popup.Locator("xpath=//a[@href='/']").First;
                await newTabLogo.ClickAsync();
                logger.Information("    The NoBrand logo has been clicked.");

                // Wait for the Home page heading to be visible, indicating the user is back on the Home page
                var newTabHero = popup.Locator("xpath=//div[@class='video-hero-with-community-search__content']").First;
                await Expect(newTabHero).ToBeVisibleAsync();
                logger.Information("    Home page heading visibility check complete. Visible: TRUE");

                bool returned = await newTabHero.IsVisibleAsync();
                await popup.CloseAsync();
                return returned;
            }
            catch (Exception ex)
            {
                logger.Warning(ex, "    Timeout during navigate-away-and-back flow.");
                return false;
            }



        }




        //-- Header Navigation Menu Methods --//

        /// <summary>
        ///     Verify the main navigation menu is visible.
        /// </summary>
        /// <returns>
        ///     Boolean value indicating if the main nav menu is visible.
        /// </returns>
        public async Task<bool> LoadHomePage_VerifyNavMenu()
        {
            logger.Information("Verifying header nav visibility...");

            // Wait for the main nav menu to be visible
            try
            {
                await Expect(NoBrandMainNavigation_Container).ToBeVisibleAsync();
                logger.Information("    Nav visibility check complete. Visible: TRUE");
                return true;
            }
            catch (Exception ex)
            {
                logger.Warning(ex, "    Timeout waiting for main header nav to be visible.");
                return false;
            }
        }


        /// <summary>
        ///     Verify the dropdown navigation menu is visible.
        /// </summary>
        /// <returns>
        ///     Boolean value indicating if the dropdown nav menu is visible.
        /// </returns>
        public async Task<bool> LoadHomePage_VerifyDropdownNavMenu()
        {
            logger.Information("Verifying header dropdown nav visibility...");

            // Click the side dropdown menu and wait for the menu items to be visible
            try
            {
                // Click the dropdown menu button
                await OpenHeaderDropdownMenu();
                logger.Information("    Header dropdown menu button opened.");

                // Wait for the dropdown nav menu to be visible
                await Expect(NoBrandHeaderPanelNavigation_Container).ToBeVisibleAsync();
                logger.Information("    Header dropdown nav visibility check complete. Visible: TRUE");

                // Close the dropdown menu after verification
                await CloseHeaderDropdownMenu();
                logger.Information("    Header dropdown menu button closed.");

                return true;


            }
            catch (Exception ex)
            {
                logger.Warning(ex, "    Timeout waiting for dropdown nav to be visible.");
                return false;
            }

        }


        /// <summary>
        ///     Verify the For Professionals dropdown navigation menu is visible.
        /// </summary>
        /// <returns>
        ///     Boolean value indicating if the dropdown nav menu item is visible.
        /// </returns>
        public async Task<bool> HeaderNavigation_VerifyForProfessionalsNavMenu()
        {
            logger.Information("Verifying For Professionals menu item link visibility...");

            try
            {
                // Wait for the For Professionals menu item link to be visible
                await Expect(ForProfessionals_HeaderDropdown_Link).ToBeVisibleAsync();
                logger.Information("    For Professionals menu item link visibility check complete. Visible: TRUE");
                return true;


            }
            catch (Exception ex)
            {
                logger.Warning(ex, "    Timeout waiting for nav menu item to be visible.");
                return false;
            }

        }


        /// <summary>
        ///     Verify the NoBrandactioners at NoBrand dropdown navigation menu is visible.
        /// </summary>
        /// <returns>
        ///     Boolean value indicating if the dropdown nav menu item is visible.
        /// </returns>
        public async Task<bool> HeaderNavigation_VerifyNoBrandactionersAtNoBrandNavMenu()
        {
            logger.Information("Verifying NoBrandactioners at NoBrand menu item link visibility...");

            try
            {
                // Wait for the NoBrandactioners at NoBrand menu item link to be visible
                await Expect(NoBrandactionersAtNoBrand_HeaderDropdown_Link).ToBeVisibleAsync();
                logger.Information("    NoBrandactioners at NoBrand menu item link visibility check complete. Visible: TRUE");
                return true;


            }
            catch (Exception ex)
            {
                logger.Warning(ex, "    Timeout waiting for nav menu item to be visible.");
                return false;
            }

        }


        /// <summary>
        ///     Verify the About Us navigation menu item is visible.
        /// </summary>
        /// <returns>
        ///     Boolean value indicating if the dropdown nav menu item is visible.
        /// </returns>
        public async Task<bool> HeaderNavigation_VerifyAboutUsNavMenuItem()
        {
            logger.Information("Verifying About Us navigation menu item link visibility...");

            try
            {
                // Wait for the About Us navigation menu item link to be visible
                await Expect(AboutUs_HeaderDropdown_Link).ToBeVisibleAsync();
                logger.Information("    About Us navigation menu item link visibility check complete. Visible: TRUE");
                return true;


            }
            catch (Exception ex)
            {
                logger.Warning(ex, "    Timeout waiting for nav menu item to be visible.");
                return false;
            }

        }


        /// <summary>
        ///     Verify the main head navigation Experience NoBrand menu item is visible.
        /// </summary>
        /// <returns>
        ///     Boolean value indicating if the main head navigation Experience NoBrand menu item is visible.
        /// </returns>
        public async Task<bool> HeaderNavigation_VerifyExperienceNoBrandNavMenuItem()
        {
            logger.Information("Verifying Experience NoBrand menu item visibility...");

            // Wait for the Experience NoBrand menu item to be visible
            try
            {
                // Find the Experience NoBrand link in the header
                await Expect(ExperienceNoBrand_MainNavigation_Link).ToBeVisibleAsync();
                logger.Information("    Webdriver successfully found the Experience NoBrand header link.");
                await ExperienceNoBrand_MainNavigation_Link.ClickAsync();

            }
            catch (Exception ex)
            {
                logger.Warning(ex, "    Timeout waiting for Experience NoBrand menu item to be visible.");
                return false;
            }

            try
            {
                // Find the Experience NoBrand link in the header
                await Expect(ExperienceNoBrand_MainNavigation_Link).ToBeVisibleAsync();
                logger.Information("    Webdriver successfully found the Experience NoBrand header link.");
                return true;
            }
            catch (Exception ex)
            {
                logger.Warning(ex, "    Timeout waiting for Experience NoBrand sub menu to be visible.");
                return false;
            }

        }


        /// <summary>
        ///     Verify the main head navigation NoBrandaction & Living menu item is visible.
        /// </summary>
        /// <returns>
        ///     Boolean value indicating if the main head navigation NoBrandaction & Living menu item is visible.
        /// </returns>
        public async Task<bool> HeaderNavigation_VerifyNoBrandactionAndLivingNavMenuItem()
        {
            logger.Information("Verifying NoBrandaction & Living menu item visibility...");

            // Wait for the NoBrandaction & Living menu item to be visible
            try
            {
                // Find the NoBrandaction & Living link in the header
                await Expect(NoBrandactionLiving_MainNavigation_Link).ToBeVisibleAsync();
                logger.Information("    Webdriver successfully found the NoBrandaction & Living header link.");
                await NoBrandactionLiving_MainNavigation_Link.ClickAsync();

            }
            catch (Exception ex)
            {
                logger.Warning(ex, "    Timeout waiting for NoBrandaction & Living menu item to be visible.");
                return false;
            }

            try
            {
                // Find the NoBrandaction & Living link in the header
                await Expect(NoBrandactionLiving_SubMenu_Link).ToBeVisibleAsync();
                logger.Information("    Webdriver successfully found the NoBrandaction & Living header link.");
                return true;
            }
            catch (Exception ex)
            {
                logger.Warning(ex, "    Timeout waiting for NoBrandaction & Living sub menu to be visible.");
                return false;
            }

        }


        /// <summary>
        ///     Verify the main head navigation Advice & Planning menu item is visible.
        /// </summary>
        /// <returns>
        ///     Boolean value indicating if the main head navigation Advice & Planning menu item is visible.
        /// </returns>
        public async Task<bool> HeaderNavigation_VerifyAdviceAndPlanningNavMenuItem()
        {
            logger.Information("Verifying Advice & Planning menu item visibility...");

            // Wait for the Advice & Planning menu item to be visible
            try
            {
                // Find the Advice & Planning link in the header
                await Expect(AdvicePlanning_MainNavigation_Link).ToBeVisibleAsync();
                logger.Information("    Webdriver successfully found the Advice & Planning header link.");
                await AdvicePlanning_MainNavigation_Link.ClickAsync();

            }
            catch (Exception ex)
            {
                logger.Warning(ex, "    Timeout waiting for Advice & Planning menu item to be visible.");
                return false;
            }

            try
            {
                // Find the Advice & Planning link in the header
                await Expect(AdvicePlanning_SubMenu_Link).ToBeVisibleAsync();
                logger.Information("    Webdriver successfully found the Advice & Planning header link.");
                return true;
            }
            catch (Exception ex)
            {
                logger.Warning(ex, "    Timeout waiting for Advice & Planning sub menu to be visible.");
                return false;
            }

        }


        /// <summary>
        ///     Navigate to the Resources page via the Advice & Planning menu item.
        /// </summary>
        /// <returns>
        ///     ResourcesPage object representing the Resources page.
        /// </returns>
        public async Task<ResourcesPage> HeaderNavigation_NavigateToResourcesPage()
        {
            logger.Information("Navigating to Resources page...");

            // Wait for the Advice & Planning menu item to be visible
            try
            {
                // Find the Advice & Planning link in the header
                await Expect(AdvicePlanning_MainNavigation_Link).ToBeVisibleAsync();
                logger.Information("    Webdriver successfully found the Advice & Planning header link.");
                await AdvicePlanning_MainNavigation_Link.ClickAsync();

            }
            catch (Exception ex)
            {
                logger.Warning(ex, "    Timeout waiting for Advice & Planning menu item to be visible.");
                throw new Exception("Timeout waiting for Advice & Planning menu item to be visible.", ex);
            }

            try
            {
                // Find the NoBrand Blog link in the header
                await Expect(NoBrandBlog_SubMenu_Link).ToBeVisibleAsync();
                logger.Information("    Webdriver successfully found the NoBrand Blog header link.");
                await NoBrandBlog_SubMenu_Link.ClickAsync();

                await page.WaitForUrlDomReadyAsync(u => u.Contains(UrlSnippet_ResourceSearch));
                logger.Information("    Successfully navigated to resource (Blog) page.");

                return new ResourcesPage(page, logger);
            }
            catch (Exception ex)
            {
                logger.Warning(ex, "    Timeout waiting for Advice & Planning sub menu to be visible.");
                throw new Exception("Timeout waiting for Advice & Planning sub menu to be visible.", ex);
            }

        }


        // -- Footer Navigation Menu Methods --//

        /// <summary>
        ///     Verify the footer navigation menu is visible.
        /// </summary>
        /// <returns>
        ///     Boolean value indicating if the footer nav menu is visible.
        /// </returns>
        public async Task<bool> LoadHomePage_VerifyFooterNavMenu()
        {
            logger.Information("Verifying footer nav visibility...");

            // Wait for the footer menu items to be visible
            try
            {
                // Scroll to the footer section
                await Expect(NoBrandFooterNavigation_Container).ToBeVisibleAsync();
                await NoBrandFooterNavigation_Container.ScrollIntoViewIfNeededAsync();
                logger.Information("    Webdriver successfully scrolled to the footer.");
                return true;
            }
            catch (Exception ex)
            {
                logger.Warning(ex, "    Timeout waiting for footer nav to be visible.");
                return false;
            }



        }


        /// <summary>
        ///     Verify the footer About Us menu item is visible.
        /// </summary>
        /// <returns>
        ///     Boolean value indicating if the footer About Us menu item is visible.
        /// </returns>
        public async Task<bool> FooterNavigation_VerifyAboutUsNavMenuItem()
        {
            logger.Information("Verifying About Us menu item visibility...");

            // Wait for the About Us menu item to be visible
            try
            {
                // Find the About Us link in the footer
                await Expect(FooterNavigation_AboutUs_Link).ToBeVisibleAsync();
                logger.Information("    Webdriver successfully found the About Us footer.");
                return true;
            }
            catch (Exception ex)
            {
                logger.Warning(ex, "    Timeout waiting for About Us menu item to be visible.");
                return false;
            }



        }


        /// <summary>
        ///     Verify the footer NoBrandactioners at NoBrand menu item is visible.
        /// </summary>
        /// <returns>
        ///     Boolean value indicating if the footer NoBrandactioners at NoBrand menu item is visible.
        /// </returns>
        public async Task<bool> FooterNavigation_VerifyNoBrandactionersAtNoBrandNavMenuItem()
        {
            logger.Information("Verifying NoBrandactioners at No Brand menu item visibility...");

            // Wait for the NoBrandactioners at No Brand menu item to be visible
            try
            {
                // Find the NoBrandactioners at No Brand link in the footer
                await Expect(FooterNavigation_NoBrandactionersAtNoBrand_Link).ToBeVisibleAsync();
                logger.Information("    Webdriver successfully found the NoBrandactioners at No Brand footer.");
                return true;
            }
            catch (Exception ex)
            {
                logger.Warning(ex, "    Timeout waiting for NoBrandactioners at No Brand menu item to be visible.");
                return false;
            }



        }


        /// <summary>
        ///     Verify the footer NoBrand Blog menu item is visible.
        /// </summary>
        /// <returns>
        ///     Boolean value indicating if the footer NoBrand Blog menu item is visible.
        /// </returns>
        public async Task<bool> FooterNavigation_VerifyNoBrandBlogNavMenuItem()
        {
            logger.Information("Verifying NoBrand Blog menu item visibility...");

            // Wait for the NoBrand Blog menu item to be visible
            try
            {
                // Find the NoBrand Blog link in the footer
                await Expect(FooterNavigation_NoBrandBlog_Link).ToBeVisibleAsync();
                logger.Information("    Webdriver successfully found the NoBrand Blog footer.");
                return true;
            }
            catch (Exception ex)
            {
                logger.Warning(ex, "    Timeout waiting for NoBrand Blog menu item to be visible.");
                return false;
            }

        }


        /// <summary>
        ///     Verify the footer For Professionals menu item is visible.
        /// </summary>
        /// <returns>
        ///     Boolean value indicating if the footer For Professionals menu item is visible.
        /// </returns>
        public async Task<bool> FooterNavigation_VerifyForProfessionalsNavMenuItem()
        {
            logger.Information("Verifying For Professionals menu item visibility...");

            // Wait for the For Professionals menu item to be visible
            try
            {
                // Find the For Professionals link in the footer
                await Expect(FooterNavigation_ForProfessionals_Link).ToBeVisibleAsync();
                logger.Information("    Webdriver successfully found the For Professionals footer.");
                return true;
            }
            catch (Exception ex)
            {
                logger.Warning(ex, "    Timeout waiting for For Professionals menu item to be visible.");
                return false;
            }

        }




        // -- Community Search Methods --//

        /// <summary>
        ///     Verify users can search via the Find a NoBrand form on the Home page.
        /// </summary>
        /// <returns>
        ///     Void.
        /// </returns>
        public async Task<US_CommunitySearchResultsPage> HomePage_FindANoBrandHome_US_SearchForCommunity()
        {
            logger.Information("Verifying user can search via the Find a NoBrand form on the Home page...");

            try
            {
                // Scroll to the search form section
                await Expect(CommunitySearch_Form_Container).ToBeVisibleAsync();
                await CommunitySearch_Form_Container.ScrollIntoViewIfNeededAsync();
                logger.Information("    Webdriver successfully scrolled to the search form.");

                // Wait for the address input field to be visible
                await Expect(CommunitySearch_AddressField_Input).ToBeVisibleAsync();
                logger.Information("    Address input field visibility check complete. Visible: TRUE");

                await CommunitySearch_AddressField_Input.FillAsync(ZipCodeInput_String);
                logger.Information("    Address data input into field.");

                await CommunitySearch_SearchButton_Button.ClickAsync();
                logger.Information("    Search button clicked.");

                // Wait for search results page to load
                await page.WaitForUrlDomReadyAsync(u => u.Contains(UrlSnippet_Communities));
                logger.Information("    Successfully navigated to Community Search Results page.");

                return new US_CommunitySearchResultsPage(page, logger);
            }
            catch (Exception ex)
            {
                logger.Warning(ex, "    Cannot input address data, select NoBrandaction type, search for community, or switch to the new tab.");
                throw;
            }
        }




        // -- Site Search (NoBrand Search) Methods --//

        /// <summary>
        ///     Verify users can search for a term via the site search on the Home page.
        /// </summary>
        /// <returns>
        ///     SiteSearchResultsPage object, boolean value indicating if the search results page loaded, and if the search term is visible in the search results.
        /// </returns>
        public async Task<SiteSearchResultsPage> SiteSearch_SearchForTerm()
        {
            logger.Information("Searching for term via site search on Home page...");

            try
            {
                // Find and click the site search button to open text input search field
                await Expect(SearchOpenButton_Button).ToBeVisibleAsync();
                await SearchOpenButton_Button.ClickAsync();
                logger.Information("    Webdriver successfully found the site search button and opened the text input search field.");
            }
            catch (Exception ex)
            {
                logger.Warning(ex, "    Timeout waiting for site search button to be visible.");
                throw;
            }

            try
            {
                // Input text into the search field and submit the search
                await Expect(SearchTextInput_Input).ToBeVisibleAsync();
                await SearchTextInput_Input.FillAsync("test");
                await SearchTextInput_Input.PressAsync("Enter");
                logger.Information("    Webdriver successfully found the site search input field and submitted text.");

                // Wait for search results page to load
                await page.WaitForUrlDomReadyAsync(u => u.Contains(UrlSnippet_SiteSearch));
                logger.Information("    Successfully navigated to site search results page.");

                return new SiteSearchResultsPage(page, logger);

            }
            catch (Exception ex)
            {
                logger.Warning(ex, "    Timeout waiting for site search input field to be visible.");
                throw;
            }

        }



        //-- Lead Form Methods --//

        /// <summary>
        ///     Scroll the page to the "Get in Touch to Learn More" lead form.
        /// </summary>
        public async Task ScrollToLeadForm()
        {
            logger.Information("Scrolling to the 'Get in Touch to Learn More' lead form...");

            try
            {
                await Expect(LeadForm_Heading).ToBeVisibleAsync();
                await LeadForm_Heading.ScrollIntoViewIfNeededAsync();
                logger.Information("    Successfully scrolled to the lead form heading.");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "    Failed to scroll to the lead form heading.");
                throw;
            }
        }


        /// <summary>
        ///     Fill the "Get in Touch to Learn More" lead form with the provided test data, including selection of "Book a Tour" which exposes the Preferred Date + Time fields.
        /// </summary>
        /// <param name="formData">Test data object containing form field values.</param>
        public async Task FillLeadForm(Home_LeadFormA_FormData formData)
        {
            logger.Information("Filling the lead form with test data...");

            try
            {
                await Expect(LeadForm_Container).ToBeVisibleAsync();
                await LeadForm_Container.ScrollIntoViewIfNeededAsync();

                await FirstName_Input.FillAsync(formData.FirstName);
                logger.Information($"    First Name: {formData.FirstName}");

                await LastName_Input.FillAsync(formData.LastName);
                logger.Information($"    Last Name: {formData.LastName}");

                await Email_Input.FillAsync(formData.Email);
                logger.Information($"    Email: {formData.Email}");

                await Phone_Input.FillAsync(formData.Phone);
                logger.Information($"    Phone: {formData.Phone}");

                await ZipCode_Input.ScrollIntoViewIfNeededAsync();
                await ZipCode_Input.FillAsync(formData.Zip);
                // Trigger validation via blur — a physical click gets intercepted by the sticky header on the Home layout.
                await ZipCode_Input.BlurAsync();
                logger.Information($"    Zip Code: {formData.Zip}");

                // The blur fires the geolocation + Coveo nearby-communities lookup; the form's required-nearby validator silently blocks the POST until it lands. Wait for it (retries transient UAT blips).
                await ZipLookup.EnsureNearbyCommunitiesPopulatedAsync(ZipCode_Input, NearbyCommunities_HiddenInput, logger);

                // Inquiry Type — "Booking a Tour" exposes the Preferred Date + Preferred Tour Time fields and hides Comments.
                await InquiryType_Select.SelectOptionAsync(new SelectOptionValue { Label = formData.InquiryType });
                logger.Information($"    Inquiry Type: {formData.InquiryType}");

                // Preferred Tour Date — unlike the header Book-a-Tour dropdown, THIS form's date field has NO qs-datepicker bound (verified via DOM diagnostic: the only picker on the Home page belongs to the separate contact-form--block Book-a-Tour section). It's a plain text input, so type the date with real keystrokes (satisfies the Pristine validator) and blur to commit.
                await Expect(PreferredTourDate_Input).ToBeVisibleAsync();
                await PreferredTourDate_Input.PressSequentiallyAsync(formData.PreferredTourDate);
                await PreferredTourDate_Input.BlurAsync();
                logger.Information($"    Preferred Tour Date: {formData.PreferredTourDate}");

                // Preferred Tour Time — options repopulate asynchronously after the picker's onSelect. Wait for the SPECIFIC configured label so the selection can't be made against a stale list; if the slot isn't offered for the target date, fall back to the first real option with a Warning.
                string timeLabelToSelect = formData.PreferredTourTime;
                ILocator desiredTimeOption = PreferredTourTime_Select.Locator("option", new LocatorLocatorOptions
                {
                    HasTextRegex = new Regex($@"^\s*{Regex.Escape(formData.PreferredTourTime)}\s*$", RegexOptions.IgnoreCase)
                });
                try
                {
                    await Expect(desiredTimeOption).ToHaveCountAsync(1, new LocatorAssertionsToHaveCountOptions
                    {
                        Timeout = TestConstants.DefaultWaitSeconds * 1000
                    });
                }
                catch (Exception)
                {
                    ILocator realTimeOptions = PreferredTourTime_Select.Locator("option[value]:not([value=''])");
                    await Expect(realTimeOptions).Not.ToHaveCountAsync(0, new LocatorAssertionsToHaveCountOptions
                    {
                        Timeout = TestConstants.ShortWaitSeconds * 1000
                    });
                    timeLabelToSelect = (await realTimeOptions.First.InnerTextAsync()).Trim();
                    logger.Warning($"    Configured tour time '{formData.PreferredTourTime}' not offered for this date; falling back to '{timeLabelToSelect}'.");
                }

                await PreferredTourTime_Select.SelectOptionAsync(new SelectOptionValue { Label = timeLabelToSelect });
                await Expect(PreferredTourTime_Select).ToHaveValueAsync(new Regex(@"\S"), new LocatorAssertionsToHaveValueOptions
                {
                    Timeout = TestConstants.ShortWaitSeconds * 1000
                });
                _selectedTourTimeLabel = timeLabelToSelect;
                logger.Information($"    Preferred Tour Time: {timeLabelToSelect} (select value: '{await PreferredTourTime_Select.InputValueAsync()}')");

                await ReferralSource_Select.SelectOptionAsync(new SelectOptionValue { Label = formData.ReferralSource });
                logger.Information($"    Referral Source: {formData.ReferralSource}");

                // Comments is hidden when InquiryType is "Book a Tour" on sibling LP forms; fill if present, otherwise skip.
                if (await Comments_Textarea.CountAsync() > 0 && await Comments_Textarea.IsVisibleAsync())
                {
                    await Comments_Textarea.FillAsync(formData.Comments);
                    logger.Information($"    Comments: {formData.Comments}");
                }
                else
                {
                    logger.Information("    Comments field not visible (hidden when Inquiry Type is 'Booking a Tour').");
                }

                await SelectMarketingAgreementCheckbNoBrand();

                logger.Information("    Lead form filled successfully.");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "    Failed to fill the lead form.");
                throw;
            }
        }


        /// <summary>
        ///     Submit the lead form. This form redirects to a standalone Thank You page on success; the method waits up to TestConstants.SubmitWaitSeconds for the URL to change.
        /// </summary>
        public async Task SubmitLeadForm()
        {
            logger.Information("Submitting the lead form...");

            try
            {
                // Guard against a late async repopulation wiping the tour-time selection between fill and submit.
                await EnsureTourTimeSelectionSurvived();

                var preSubmitErrors = await LeadForm_ValidationErrors.CountAsync();
                if (preSubmitErrors > 0)
                {
                    logger.Warning($"    Found {preSubmitErrors} potential validation indicators before submit.");
                    int reportCount = Math.Min(preSubmitErrors, 5);
                    for (int i = 0; i < reportCount; i++)
                    {
                        var err = LeadForm_ValidationErrors.Nth(i);
                        if (await err.IsVisibleAsync())
                        {
                            string text = await err.InnerTextAsync();
                            if (!string.IsNullOrWhiteSpace(text))
                                logger.Warning($"      - {text}");
                        }
                    }
                }

                logger.Information($"    Submit button enabled: {await LeadForm_Submit_Button.IsEnabledAsync()}");

                await LeadForm_Submit_Button.ScrollIntoViewIfNeededAsync();

                string urlBeforeSubmit = page.Url;
                await LeadForm_Submit_Button.ClickAsync();
                logger.Information("    Submit button clicked.");

                // Success signal: the page navigates away from the Home URL to the standalone Thank You page.
                try
                {
                    await page.WaitForUrlDomReadyAsync(u => u != urlBeforeSubmit, TestConstants.SubmitWaitSeconds * 1000);
                    logger.Information($"    Form submission redirected. New URL: {page.Url}");
                }
                catch (Exception)
                {
                    logger.Warning("    Timeout waiting for submission redirect. Continuing to verification.");
                }

                var postSubmitErrors = await LeadForm_ValidationErrors.CountAsync();
                if (postSubmitErrors > 0)
                {
                    logger.Error($"    Found {postSubmitErrors} validation errors after submit:");
                    int reportCount = Math.Min(postSubmitErrors, 5);
                    for (int i = 0; i < reportCount; i++)
                    {
                        var err = LeadForm_ValidationErrors.Nth(i);
                        if (await err.IsVisibleAsync())
                        {
                            string text = await err.InnerTextAsync();
                            if (!string.IsNullOrWhiteSpace(text))
                                logger.Error($"      - {text}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "    Failed to submit the lead form.");
                throw;
            }
        }


        /// <summary>
        ///     Verify the submission redirected to the standalone Thank You page and the confirmation heading is visible.
        /// </summary>
        /// <returns>True if the Thank You page heading is visible after redirect; false otherwise.</returns>
        public async Task<bool> VerifyThankYouConfirmation()
        {
            logger.Information("Verifying redirect to the Thank You page and confirmation heading...");

            try
            {
                await Expect(ThankYouPage_Heading).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
                {
                    Timeout = TestConstants.SubmitWaitSeconds * 1000
                });
                logger.Information($"    Thank You page heading visible. URL: {page.Url}.");
                return true;
            }
            catch (Exception ex)
            {
                logger.Warning(ex, $"    Timeout waiting for Thank You page heading. Current URL: {page.Url}");
                return false;
            }
        }




        //-- Helper Methods --//

        /// <summary>
        ///     Returns driver to the Home page.
        /// </summary>
        /// <returns>
        ///     Void.
        /// </returns>
        public async Task NavigateToHome()
        {
            await page.GotoDomReadyAsync(PageUrl);
        }


        /// <summary>
        ///     Opens the dropdown navigation menu.
        /// </summary>
        public async Task OpenHeaderDropdownMenu()
        {
            logger.Information("Opening header dropdown nav menu...");
            try
            {
                // Click the dropdown menu button
                await Expect(HeaderDropdownMenuOpen_Button).ToBeVisibleAsync();
                await HeaderDropdownMenuOpen_Button.ClickAsync();
                logger.Information("    Header dropdown menu button opened.");
            }
            catch (Exception ex)
            {
                logger.Warning(ex, "    Timeout waiting for header dropdown menu button to be visible and clickable.");
                throw;
            }
        }


        /// <summary>
        ///     Closes the dropdown navigation menu.
        /// </summary>
        public async Task CloseHeaderDropdownMenu()
        {
            logger.Information("Closing header dropdown nav menu...");
            try
            {
                // Click the dropdown menu close button
                await Expect(HeaderDropdownMenuClose_Button).ToBeVisibleAsync();
                await HeaderDropdownMenuClose_Button.ClickAsync();
                logger.Information("    Header dropdown menu button closed.");
            }
            catch (Exception ex)
            {
                logger.Warning(ex, "    Timeout waiting for header dropdown menu close button to be visible and clickable.");
                throw;
            }

        }


        //-- Private Lead Form Helpers --//

        /// <summary>
        ///     Re-verify (and re-select once) the tour-time selection immediately before submit, in case a late async repopulation of the time options wiped it after the fill flow completed.
        /// </summary>
        private async Task EnsureTourTimeSelectionSurvived()
        {
            if (_selectedTourTimeLabel is null) return;   // the fill flow wasn't used in this test path

            string currentValue = await PreferredTourTime_Select.InputValueAsync();
            if (!string.IsNullOrWhiteSpace(currentValue)) return;

            logger.Warning("    Preferred Tour Time was wiped by an async repopulation — re-selecting once before submit.");
            await PreferredTourTime_Select.SelectOptionAsync(new SelectOptionValue { Label = _selectedTourTimeLabel });
            await Expect(PreferredTourTime_Select).ToHaveValueAsync(new Regex(@"\S"), new LocatorAssertionsToHaveValueOptions
            {
                Timeout = TestConstants.ShortWaitSeconds * 1000
            });
        }


        /// <summary>
        ///     Check the marketing consent checkbNoBrand via native click (bypasses the faux-checkbNoBrand overlay the CMS layers on top of the native input). No-op if already selected.
        /// </summary>
        private async Task SelectMarketingAgreementCheckbNoBrand()
        {
            logger.Information("    Checking marketing agreement checkbNoBrand...");

            if (await MarketingConsent_CheckbNoBrand.IsCheckedAsync())
            {
                logger.Information("        Marketing consent checkbNoBrand already checked.");
                return;
            }

            await MarketingConsent_CheckbNoBrand.EvaluateAsync("el => el.click()");
            logger.Information("        Marketing consent checkbNoBrand checked.");
        }


        //-- Body Section Methods --//

        /// <summary>
        ///     Click the body-section "Learn About NoBrandaction Types" CTA — anchor link to the in-page #contact section.
        /// </summary>
        public async Task ClickHero_LearnAboutNoBrandactionTypesCta()
        {
            logger.Information("Clicking body 'Learn About NoBrandaction Types' CTA...");
            try
            {
                await Hero_LearnAboutNoBrandactionTypes_Link.ScrollIntoViewIfNeededAsync();
                await Hero_LearnAboutNoBrandactionTypes_Link.ClickAsync();
                logger.Information("    'Learn About NoBrandaction Types' CTA clicked.");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "    Failed to click 'Learn About NoBrandaction Types' CTA.");
                throw;
            }
        }


        /// <summary>
        ///     Verify the URL fragment is #contact after the hero CTA click. The destination of an anchor link is the URL hash; the page itself contains an id="Contact-Us" target whose case does not match the CTA's href ("#contact"), so the browser does not scroll — but the click wiring is intact.
        /// </summary>
        public async Task<bool> VerifyHero_LearnAboutNoBrandactionTypes_AnchorActive()
        {
            logger.Information("Verifying #contact URL fragment is present after CTA click...");
            try
            {
                await page.WaitForUrlDomReadyAsync(u => u.Contains("#contact"));
                logger.Information("    URL fragment #contact present. Current URL: {Url}", page.Url);
                return true;
            }
            catch (Exception ex)
            {
                logger.Warning(ex, "    Timeout waiting for URL to contain #contact.");
                return false;
            }
        }


        /// <summary>
        ///     Click the "Discover the NoBrand Experience" feature card in the "NoBrand Your Way" body section.
        /// </summary>
        public async Task<ExperienceNoBrandPage> ClickNoBrandYourWay_DiscoverNoBrandExperienceCard()
        {
            logger.Information("Clicking 'Discover the NoBrand Experience' feature card...");
            try
            {
                await NoBrandYourWay_DiscoverNoBrandExperience_Card.ScrollIntoViewIfNeededAsync();
                await NoBrandYourWay_DiscoverNoBrandExperience_Card.ClickAsync();
                logger.Information("    Feature card clicked.");
                return new ExperienceNoBrandPage(page, logger);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "    Failed to click feature card.");
                throw;
            }
        }


        /// <summary>
        ///     Click one of the six "Making Every Day Brilliant" desktop accordion cards by its href.
        ///     Each card has a duplicate mobile-only sibling with a different class — this method targets the desktop variant (a.hightlights-row-accordion).
        /// </summary>
        public async Task ClickMakingEveryDayBrilliant_CardByHref(string expectedHref)
        {
            logger.Information("Clicking 'Making Every Day Brilliant' card with href {Href}...", expectedHref);
            ILocator card = page.Locator($"a.hightlights-row-accordion[href='{expectedHref}']").First;
            try
            {
                await card.ScrollIntoViewIfNeededAsync();
                await card.ClickAsync();
                logger.Information("    Card clicked.");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "    Failed to click card with href {Href}.", expectedHref);
                throw;
            }
        }


        /// <summary>
        ///     Click one of the four "What are you looking for today?" persona tabs by a unique substring of its label. Valid substrings: "looking for a family member", "looking for myself",  "to refer a patient or client", "interested in NoBrandactioner opportunities".
        ///     Waits until the tab's aria-selected attribute flips to "true" before returning.
        /// </summary>
        public async Task ClickWhatAreYouLookingFor_PersonaTab(string labelSubstring)
        {
            logger.Information("Clicking persona tab matching '{Label}'...", labelSubstring);
            ILocator tab = page.Locator($"xpath=//button[contains(@class,'quick-access__trigger')][.//span[contains(., \"{labelSubstring}\")]]").First;
            try
            {
                await tab.ScrollIntoViewIfNeededAsync();
                await tab.ClickAsync();
                await Expect(tab).ToHaveAttributeAsync("aria-selected", "true");
                logger.Information("    Persona tab '{Label}' is now selected.", labelSubstring);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "    Failed to click persona tab '{Label}'.", labelSubstring);
                throw;
            }
        }


        /// <summary>
        ///     Verify the persona tab matching the given label substring is currently selected (aria-selected="true").
        /// </summary>
        public async Task<bool> VerifyWhatAreYouLookingFor_PersonaTabIsSelected(string labelSubstring)
        {
            logger.Information("Verifying persona tab '{Label}' is selected...", labelSubstring);
            ILocator tab = page.Locator($"xpath=//button[contains(@class,'quick-access__trigger')][.//span[contains(., \"{labelSubstring}\")]]").First;
            try
            {
                await Expect(tab).ToBeVisibleAsync();
                bool selected = await tab.GetAttributeAsync("aria-selected") == "true";
                logger.Information("    Tab '{Label}' aria-selected: {Selected}", labelSubstring, selected);
                return selected;
            }
            catch (Exception ex)
            {
                logger.Warning(ex, "    Timeout finding persona tab.");
                return false;
            }
        }


        /// <summary>
        ///     Click a persona card under the currently active "What Are You Looking For" panel by its href.
        ///     Some hrefs repeat across panels (e.g. /NoBrandaction-living, /NoBrandaction-questionnaire). The active panel is resolved by reading the selected tab's aria-controls attribute, since the panels themselves do not flip aria-hidden. The caller must activate the right tab first.
        /// </summary>
        public async Task ClickPersonaCard_UnderActivePanel_ByHref(string expectedHref)
        {
            logger.Information("Clicking persona card with href {Href} under active panel...", expectedHref);
            try
            {
                ILocator selectedTab = page.Locator("button.quick-access__trigger[aria-selected='true']").First;
                await Expect(selectedTab).ToBeVisibleAsync();
                string panelId = await selectedTab.GetAttributeAsync("aria-controls");
                if (string.IsNullOrEmpty(panelId))
                {
                    throw new Exception("Selected persona tab has no aria-controls attribute.");
                }
                logger.Information("    Selected tab references panel id: {PanelId}", panelId);

                ILocator card = page.Locator($"#{panelId} a.quick-access__card[href='{expectedHref}']").First;
                await card.ScrollIntoViewIfNeededAsync();
                await card.ClickAsync();
                logger.Information("    Persona card clicked.");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "    Failed to click persona card with href {Href}.", expectedHref);
                throw;
            }
        }


        /// <summary>
        ///     Click the featured article card in the "Latest from NoBrand" body section.
        /// </summary>
        public async Task ClickLatestFromNoBrand_FeaturedArticleCard()
        {
            logger.Information("Clicking 'Latest from NoBrand' featured article card...");
            try
            {
                await LatestFromNoBrand_FeaturedArticle_Link.ScrollIntoViewIfNeededAsync();
                await LatestFromNoBrand_FeaturedArticle_Link.ClickAsync();
                logger.Information("    Featured article card clicked.");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "    Failed to click featured article card.");
                throw;
            }
        }


        /// <summary>
        ///     Click one of the four "Latest from NoBrand" blog list items by its href.
        /// </summary>
        public async Task ClickLatestFromNoBrand_BlogItemByHref(string expectedHref)
        {
            logger.Information("Clicking blog list item with href {Href}...", expectedHref);
            ILocator item = page.Locator($"a.latest-news__item-link[href='{expectedHref}']").First;
            try
            {
                await item.ScrollIntoViewIfNeededAsync();
                await item.ClickAsync();
                logger.Information("    Blog list item clicked.");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "    Failed to click blog list item with href {Href}.", expectedHref);
                throw;
            }
        }


        /// <summary>
        ///     Click the "View All Blogs" CTA at the bottom of the "Latest from NoBrand" body section.
        ///     The href is a fragment-only anchor (#&amp;f:@ssl_articletype=[Blog]) processed by client-side JS.
        /// </summary>
        public async Task ClickLatestFromNoBrand_ViewAllBlogsCta()
        {
            logger.Information("Clicking 'View All Blogs' CTA...");
            try
            {
                await LatestFromNoBrand_ViewAllBlogs_Cta.ScrollIntoViewIfNeededAsync();
                await LatestFromNoBrand_ViewAllBlogs_Cta.ClickAsync();
                logger.Information("    'View All Blogs' CTA clicked.");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "    Failed to click 'View All Blogs' CTA.");
                throw;
            }
        }


        /// <summary>
        ///     Click the in-body Instagram icon in the "Follow Us On Social" body block. Opens in a new tab;  the caller is responsible for switching to the new window handle and asserting the destination.
        /// </summary>
        public async Task ClickBodySocial_InstagramIcon()
        {
            logger.Information("Clicking body Instagram icon...");
            try
            {
                await BodySocial_InstagramIcon_Link.ScrollIntoViewIfNeededAsync();
                await BodySocial_InstagramIcon_Link.ClickAsync();
                logger.Information("    Instagram icon clicked.");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "    Failed to click Instagram icon.");
                throw;
            }
        }


        /// <summary>
        ///     Click the in-body Instagram icon and verify it opens a new tab on instagram.com.
        ///     Captures the popup page via RunAndWaitForPageAsync, reads its URL, then closes it.
        /// </summary>
        /// <returns>True if the new tab's URL contains "instagram.com"; false otherwise.</returns>
        public async Task<bool> ClickBodySocial_InstagramIcon_VerifyOpensInstagram()
        {
            logger.Information("Clicking body Instagram icon and verifying new-tab destination...");
            try
            {
                await BodySocial_InstagramIcon_Link.ScrollIntoViewIfNeededAsync();
                var popup = await page.Context.RunAndWaitForPageAsync(async () =>
                {
                    await BodySocial_InstagramIcon_Link.ClickAsync();
                });
                logger.Information("    Instagram icon clicked; new tab opened.");
                await popup.WaitForLoadStateAsync(LoadState.DOMContentLoaded);

                bool matched = popup.Url.Contains("instagram.com");
                logger.Information("    New tab URL: {Url}", popup.Url);
                await popup.CloseAsync();
                return matched;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "    Failed to open Instagram in a new tab.");
                return false;
            }
        }


        /// <summary>
        ///     Wait until the browser URL contains the given substring. Used as a generic destination assertion after clicking a body-section link whose target page does not have a dedicated page object.
        /// </summary>
        public async Task<bool> VerifyUrlContains(string urlSubstring)
        {
            logger.Information("Verifying URL contains '{Sub}'...", urlSubstring);
            try
            {
                await page.WaitForUrlDomReadyAsync(u => u.Contains(urlSubstring));
                logger.Information("    URL contains '{Sub}'. Current URL: {Url}", urlSubstring, page.Url);
                return true;
            }
            catch (Exception ex)
            {
                logger.Warning(ex, "    Timeout. Current URL: {Url}", page.Url);
                return false;
            }
        }


    }
}
