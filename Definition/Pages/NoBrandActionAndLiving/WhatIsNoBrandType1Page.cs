using Definition.TestData.FormData;
using Microsoft.Playwright;
using Serilog;
using Utility;
using static Microsoft.Playwright.Assertions;

namespace Definition.Pages.NoBrandactionAndLiving
{
    public class WhatIsNoBrandType1Page(IPage page, ILogger logger)
    {
        private const string PageUrl = "https://uat.NoBrand.com/";

        //-- Cookie Consent Elements --//
        private ILocator AcceptCookies_Button => page.Locator("#truste-consent-button");

        //-- Lead Form Elements (scoped to the last contact form on the page) --//
        private ILocator LeadForm => page.Locator("form.community-contact-us__form").Last;
        private ILocator FormHeading =>
            page.Locator("xpath=//h2[contains(normalize-space(.), 'Are you interested in NoBrand type 1?')]").First;
        private ILocator FirstName_Input => LeadForm.Locator("input[name='firstName']").First;
        private ILocator LastName_Input => LeadForm.Locator("input[name='lastName']").First;
        private ILocator Email_Input => LeadForm.Locator("input[name='emailAddress']").First;
        private ILocator Phone_Input => LeadForm.Locator("input[name='phoneNumber']").First;
        private ILocator ZipCode_Input => LeadForm.Locator("input[name='zipCode']").First;
        private ILocator InquiryType_Select => LeadForm.Locator("select[name='topic']").First;     // How can we help you?
        private ILocator Comments_Input => LeadForm.Locator("textarea[name='comments']").First;
        private ILocator ReferralSource_Select => LeadForm.Locator("select[name='source']").First; // How did you hear about us?
        private ILocator MarketingConsent_CheckbNoBrand => LeadForm.Locator("input[type='checkbNoBrand'][name='smsConsent']").First;
        private ILocator NearbyCommunities_HiddenInput => LeadForm.Locator("input[name='NearbyCommunities']").First;
        private ILocator Submit_Button => LeadForm.Locator("button[type='submit']").First;
        private ILocator PostSubmitErrorMessage_Text =>
            page.Locator("xpath=//*[contains(@class, 'pristine-error') or contains(@class, 'field__error') or contains(@class, 'invalid')]");

        //-- Thank You Elements --//
        // On success the form redirects to a Thank You page. Filter to the VISIBLE heading to avoid hidden templates that the bare text match would also resolve to.
        private ILocator ThankYouHeader =>
            page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Thank you for your interest in NoBrand" })
                .Filter(new LocatorFilterOptions { Visible = true });

        //-- Page Methods --//

        /// <summary>
        ///     Navigate to the What Is NoBrand type 1 page (waits for DOMContentLoaded - suite navigation policy, see Utility.PageNavigation).
        /// </summary>
        public async Task NavigateToLandingPage()
        {
            logger.Information("Navigating to What Is NoBrand type 1 page...");
            await page.GotoDomReadyAsync(PageUrl);
            logger.Information("    Successfully navigated to What Is NoBrand type 1 page.");
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
            await FormHeading.ScrollIntoViewIfNeededAsync();
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

            // Zip drives a keystroke-triggered "nearby communities" typeahead. FillAsync sets the value in one shot and doesn't fire the per-key events the lookup listens for, so type it character-by-character (the equivalent of the original Selenium SendKeys). PressSequentially focuses programmatically — no pointer click — so a sticky/floating element can't intercept it.
            await ZipCode_Input.PressSequentiallyAsync(formData.Zip);
            await ZipCode_Input.BlurAsync();
            logger.Information($"    Zip Code: {formData.Zip}");

            // The blur fires the geolocation + Coveo nearby-communities lookup; the form's required-nearby validator silently blocks the POST until it lands. Wait for it (retries transient UAT blips).
            await ZipLookup.EnsureNearbyCommunitiesPopulatedAsync(ZipCode_Input, NearbyCommunities_HiddenInput, logger);

            await InquiryType_Select.SelectOptionAsync(new SelectOptionValue { Label = formData.InquiryType });
            logger.Information($"    Inquiry Type: {formData.InquiryType}");

            // Comments only appears when Inquiry Type is not "Book a Tour" — fill if present, otherwise skip.
            if (await Comments_Input.CountAsync() > 0 && await Comments_Input.IsVisibleAsync())
            {
                await Comments_Input.FillAsync(formData.Comments);
                logger.Information($"    Comments: {formData.Comments}");
            }
            else
            {
                logger.Information("    Comments field not visible (may be conditional based on Inquiry Type).");
            }

            await ReferralSource_Select.SelectOptionAsync(new SelectOptionValue { Label = formData.ReferralSource });
            logger.Information($"    Referral Source: {formData.ReferralSource}");

            await CheckMarketingConsent();

            logger.Information("    Lead form filled successfully.");
        }

        /// <summary>
        ///     Submit the lead form. Playwright auto-waits for the button to be actionable before clicking.
        ///     After clicking, any post-submit validation errors are surfaced to the log for diagnosability.
        /// </summary>
        public async Task SubmitLeadForm()
        {
            logger.Information("Submitting lead form...");
            logger.Information("    Submit button enabled: {Enabled}", await Submit_Button.IsEnabledAsync());

            await Submit_Button.ClickAsync();
            logger.Information("    Submit button clicked.");
            logger.Information($"    Current URL after submit: {page.Url}");

            // Surface any validation errors so failing submissions are diagnosable from the log alone.
            int errorCount = await PostSubmitErrorMessage_Text.CountAsync();
            if (errorCount > 0)
            {
                logger.Error($"    Found {errorCount} validation errors after submit:");
                for (int i = 0; i < Math.Min(errorCount, 5); i++)
                {
                    ILocator error = PostSubmitErrorMessage_Text.Nth(i);
                    if (await error.IsVisibleAsync())
                    {
                        string text = await error.InnerTextAsync();
                        if (!string.IsNullOrWhiteSpace(text))
                        {
                            logger.Error($"      - {text}");
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Verify that the Thank You confirmation has loaded successfully.
        /// </summary>
        /// <returns>True if a Thank You signal is visible, false otherwise.</returns>
        public async Task<bool> VerifyThankYouPage()
        {
            logger.Information("Verifying Thank You confirmation...");

            // Signal 1: the dedicated Thank You heading on a redirected page.
            try
            {
                await Expect(ThankYouHeader.First).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
                {
                    Timeout = TestConstants.SubmitWaitSeconds * 1000
                });
                logger.Information("    Thank You page heading found.");
                return true;
            }
            catch (Exception)
            {
                logger.Information("    H1 Thank You heading not found, checking for inline confirmation...");
            }

            // Signal 2: any visible inline "thank you" confirmation rendered in-place (case-insensitive).
            try
            {
                await Expect(page.GetByText("Thank you").Filter(new LocatorFilterOptions { Visible = true }).First)
                    .ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
                    {
                        Timeout = TestConstants.ShortWaitSeconds * 1000
                    });
                logger.Information("    Thank you message found on page.");
                return true;
            }
            catch (Exception)
            {
                logger.Information("    No thank you message found, checking URL...");
            }

            // Signal 3: URL fallback.
            string currentUrl = page.Url ?? string.Empty;
            if (currentUrl.Contains("thank-you", StringComparison.OrdinalIgnoreCase)
                || currentUrl.Contains("/thank/", StringComparison.OrdinalIgnoreCase))
            {
                logger.Information($"    Thank You URL detected: {currentUrl}");
                return true;
            }

            logger.Error("    No Thank You confirmation found.");
            logger.Error($"    Current URL: {currentUrl}");
            return false;
        }

        //-- Private Helper Methods --//

        /// <summary>
        ///     Check the required smsConsent marketing checkbNoBrand. It's a React-controlled, visually hidden
        ///     input, so we dispatch a native .click() on it (the Playwright equivalent of the original JS
        ///     click); no-ops if already checked.
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

