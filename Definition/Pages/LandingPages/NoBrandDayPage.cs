using Definition.TestData.FormData;
using Microsoft.Playwright;
using Serilog;
using Utility;
using static Microsoft.Playwright.Assertions;

namespace Definition.Pages.LandingPages
{
    public class NoBrandEveryDayPage(IPage page, ILogger logger)
    {
        private const string PageUrl = "https://uat.NoBrand.com";

        //-- Cookie Consent Elements --//
        private ILocator AcceptCookies_Button => page.Locator("#truste-consent-button");

        //-- Lead Form Elements (scoped to the last contact form on the page) --//
        private ILocator LeadForm => page.Locator("form.community-contact-us__form").Last;
        private ILocator FormHeading => page.GetByText("Discover NoBranduser Living");
        // .First mirrors the original FindElement(first-match) semantics for id-substring selectors.
        private ILocator FirstName_Input => LeadForm.Locator("input[id*='first-name']").First;
        private ILocator LastName_Input => LeadForm.Locator("input[id*='last-name']").First;
        private ILocator Email_Input => LeadForm.Locator("input[id*='email']").First;
        private ILocator Phone_Input => LeadForm.Locator("input[id*='phone']").First;
        private ILocator ZipCode_Input => LeadForm.Locator("input[id*='zip-code']").First;
        private ILocator InquiryType_Select => LeadForm.Locator("select[name='topic']").First;   // How can we help you?
        private ILocator ReferralSource_Select => LeadForm.Locator("select[name='source']").First; // How did you hear about us?
        private ILocator Comments_Input => LeadForm.Locator("textarea[id*='comments']").First;
        private ILocator MarketingConsent_CheckbNoBrand => LeadForm.Locator("input[type='checkbNoBrand'][name='smsConsent']").First;
        private ILocator NearbyCommunities_HiddenInput => LeadForm.Locator("input[name='NearbyCommunities']").First;
        private ILocator Submit_Button => LeadForm.Locator("button[type='submit']").First;

        //-- Thank You Elements --//
        // The form redirects to a Thank You page. Filter to the visible heading to avoid hidden templates.
        private ILocator ThankYouHeader =>
            page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Thank you for your interest in NoBrand" })
                .Filter(new LocatorFilterOptions { Visible = true });

        //-- Page Methods --//

        /// <summary>
        ///     Navigate to the NoBrand Every Day landing page (waits for DOMContentLoaded - suite navigation policy, see Utility.PageNavigation).
        /// </summary>
        public async Task NavigateToLandingPage()
        {
            logger.Information("Navigating to NoBrand Every Day landing page...");
            await page.GotoDomReadyAsync(PageUrl);
            logger.Information("    Successfully navigated to landing page.");
        }

        /// <summary>
        ///     Accept cookie consent banner if visible. Non-fatal: swallows the not-found timeout.
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
            catch (Exception)
            {
                logger.Information("    Cookie banner not found or already accepted.");
            }
        }

        /// <summary>
        ///     Bring the lead form into view. (Playwright auto-scrolls before actions; kept for parity.)
        /// </summary>
        public async Task ScrollToLeadForm()
        {
            logger.Information("Scrolling to lead form section...");
            await FormHeading.First.ScrollIntoViewIfNeededAsync();
            logger.Information("    Successfully scrolled to lead form.");
        }

        /// <summary>
        ///     Fill out the lead form with provided test data.
        /// </summary>
        /// <param name="formData">Test data object containing form field values.</param>
        public async Task FillLeadForm(LP_LeadFormA_FormData formData)
        {
            logger.Information("Filling out lead form with test data...");

            await FirstName_Input.FillAsync(formData.FirstName);
            logger.Information($"    First Name: {formData.FirstName}");

            await LastName_Input.FillAsync(formData.LastName);
            logger.Information($"    Last Name: {formData.LastName}");

            await Email_Input.FillAsync(formData.Email);
            logger.Information($"    Email: {formData.Email}");

            await Phone_Input.FillAsync(formData.Phone);
            logger.Information($"    Phone: {formData.Phone}");

            await ZipCode_Input.PressSequentiallyAsync(formData.Zip);
            await ZipCode_Input.BlurAsync();
            logger.Information($"    Zip Code: {formData.Zip}");

            await ZipLookup.EnsureNearbyCommunitiesPopulatedAsync(ZipCode_Input, NearbyCommunities_HiddenInput, logger);

            await InquiryType_Select.SelectOptionAsync(new SelectOptionValue { Label = formData.InquiryType });
            logger.Information($"    Inquiry Type: {formData.InquiryType}");

            await ReferralSource_Select.SelectOptionAsync(new SelectOptionValue { Label = formData.ReferralSource });
            logger.Information($"    Referral Source: {formData.ReferralSource}");

            // Comments only appears for some inquiry types — fill if present, otherwise skip.
            if (await Comments_Input.CountAsync() > 0 && await Comments_Input.IsVisibleAsync())
            {
                await Comments_Input.FillAsync(formData.Comments);
                logger.Information($"    Comments: {formData.Comments}");
            }
            else
            {
                logger.Information("    Comments field not visible (conditional on Inquiry Type).");
            }

            await CheckMarketingConsent();

            logger.Information("    Lead form filled successfully.");
        }

        /// <summary>
        ///     Submit the lead form. Playwright auto-waits for the button to be actionable.
        /// </summary>
        public async Task SubmitLeadForm()
        {
            logger.Information("Submitting lead form...");
            logger.Information("    Submit button enabled: {Enabled}", await Submit_Button.IsEnabledAsync());
            await Submit_Button.ClickAsync();
            logger.Information("    Submit button clicked.");
        }

        /// <summary>
        ///     Verify that the Thank You page has loaded after submission.
        /// </summary>
        /// <returns>True if the Thank You heading becomes visible; false otherwise.</returns>
        public async Task<bool> VerifyThankYouPage()
        {
            logger.Information("Verifying Thank You message...");

            // Tier 1: the dedicated Thank You page heading (form redirects on success).
            try
            {
                await Expect(ThankYouHeader.First).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
                {
                    Timeout = TestConstants.SubmitWaitSeconds * 1000
                });
                logger.Information("    Thank You page heading is visible.");
                return true;
            }
            catch (Exception)
            {
                logger.Information("    H1 Thank You heading not found; checking for an inline confirmation...");
            }

            // Tier 2: any visible "thank you" confirmation rendered in-place (case-insensitive substring).
            try
            {
                await Expect(page.GetByText("Thank you").Filter(new LocatorFilterOptions { Visible = true }).First)
                    .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
                    {
                        Timeout = TestConstants.ShortWaitSeconds * 1000
                    });
                logger.Information("    Inline thank-you confirmation is visible.");
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "    No Thank You confirmation found. Current URL: {Url}", page.Url);
                return false;
            }
        }

        //-- Private Helper Methods --//

        /// <summary>
        ///     Check the required smsConsent marketing checkbNoBrand. It's a React-controlled, visually hidden input, so we dispatch a native .click() on it (the Playwright equivalent of the original JS click); no-ops if already checked.
        /// </summary>
        private async Task CheckMarketingConsent()
        {
            if (await MarketingConsent_CheckbNoBrand.IsCheckedAsync())
            {
                logger.Information("    Marketing consent checkbNoBrand already checked.");
                return;
            }
            await MarketingConsent_CheckbNoBrand.EvaluateAsync("el => el.click()");
            logger.Information("    Marketing consent checkbNoBrand checked.");
        }
    }
}

