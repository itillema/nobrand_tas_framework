using Definition.TestData.FormData;
using Microsoft.Playwright;
using Serilog;
using Utility;
using static Microsoft.Playwright.Assertions;

namespace Definition.Pages.CommunityPages.US_CommunityPages
{
    public class CommunityPage_NoBrandBCCourt(IPage page, ILogger logger)
    {
        private const string PageUrl = "https://uat.NoBrand.com/";
        private const string FormScopeXPath = "//section[@id='Contact-Us']//form[contains(@class,'community-contact-us__form')]";


        //-- Page Elements --//
        private ILocator PageH1Heading_Text => page.Locator("xpath=//div[@class='NoBrandBC-community-landing-masthead__headings container']//h1[@class='NoBrandBC-community-landing-masthead__heading' and text()='NoBrandBC']").First;


        //-- Cookie Consent Elements --//
        private ILocator AcceptCookies_Button => page.Locator("#onetrust-accept-btn-handler");


        //-- Lead Form Elements (scoped to the inline NoBrandBC contact form) --//
        private ILocator FormHeading => page.Locator("xpath=//h2[contains(normalize-space(.),'Want to know more about NoBrandBC')]").First;
        private ILocator FormContainer => page.Locator($"xpath={FormScopeXPath}").First;
        // Fields are scoped to the form container
        private ILocator FirstName_Input => FormContainer.Locator("xpath=.//input[@name='firstName']").First;
        private ILocator LastName_Input => FormContainer.Locator("xpath=.//input[@name='lastName']").First;
        private ILocator Email_Input => FormContainer.Locator("xpath=.//input[@name='emailAddress']").First;
        private ILocator Phone_Input => FormContainer.Locator("xpath=.//input[@name='phoneNumber']").First;
        private ILocator InterestedIn_Select => FormContainer.Locator("xpath=.//select[@name='topic']").First;
        private ILocator ReferralSource_Select => FormContainer.Locator("xpath=.//select[@name='source']").First;
        // Marketing-agreement checkbNoBrand: prefer a consent/agreement-named checkbNoBrand, else the last checkbNoBrand in the form.
        private ILocator MarketingConsent_CheckbNoBrand => FormContainer.Locator("xpath=.//input[@type='checkbNoBrand' and (contains(@name,'Consent') or contains(@name,'consent') or contains(@name,'agreement') or contains(@name,'smsConsent'))]").First;
        private ILocator MarketingConsent_CheckbNoBrand_Fallback => FormContainer.Locator("xpath=.//input[@type='checkbNoBrand']").Last;
        private ILocator Submit_Button => FormContainer.Locator("xpath=.//button[@type='submit']").First;


        //-- Thank You Confirmation Page Elements --//
        // The form redirects to a separate Thank You page. Filter to the visible heading to avoid hidden templates.
        private ILocator ThankYouPageHeading =>
            page.Locator("xpath=//h1[contains(normalize-space(.),'Thank you for your interest in NoBrandBC')]")
                .Filter(new LocatorFilterOptions { Visible = true });
        // Tier-2 fallback: any visible text carrying the confirmation phrase, regardless of DOM structure.
        private ILocator ThankYouConfirmationText =>
            page.GetByText("Thank you for your interest in NoBrandBC")
                .Filter(new LocatorFilterOptions { Visible = true });


        //-- Page Methods --//

        /// <summary>
        ///     Verify that the NoBrandBC page loads as expected.
        /// </summary>
        /// <returns>
        ///     Boolean value indicating if the NoBrandBC page heading is visible.
        /// </returns>
        public async Task<bool> LoadNoBrandBCCourtPage_VerifyHeading()
        {
            logger.Information("    Verifying heading is displayed on page...");
            try
            {
                await Expect(PageH1Heading_Text).ToBeVisibleAsync();
                logger.Information("    Heading is displayed on page.");
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "    Failed to verify heading is displayed on the page.");
                return false;
            }
        }

        /// <summary>
        ///     Navigate to the NoBrandBC community page (waits for DOMContentLoaded - suite navigation policy, see Utility.PageNavigation).
        /// </summary>
        public async Task NavigateToNoBrandBCCourtPage()
        {
            logger.Information("Navigating to the NoBrandBC community page...");
            await page.GotoDomReadyAsync(PageUrl);
            logger.Information("    Successfully navigated to the NoBrandBC page.");
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
            catch (Exception)
            {
                logger.Information("    Cookie banner not found or already accepted.");
            }
        }

        /// <summary>
        ///     Scroll the page to bring the "Want to know more about NoBrandBC?" heading and its lead form into view.
        /// </summary>
        public async Task ScrollToLeadForm()
        {
            logger.Information("Scrolling to the 'Want to know more about NoBrandBC?' lead form...");
            await FormHeading.ScrollIntoViewIfNeededAsync();
            logger.Information("    Successfully scrolled to lead form.");
        }

        /// <summary>
        ///     Fill out all lead form fields with the provided test data and check the marketing agreement checkbNoBrand.
        /// </summary>
        /// <param name="formData">Test data DTO containing all field values.</param>
        public async Task FillLeadForm(CP_NoBrandBCCourt_FormData formData)
        {
            logger.Information("Filling out lead form with test data...");

            // Center the form in the viewport
            await FormContainer.ScrollIntoViewIfNeededAsync();

            await FirstName_Input.FillAsync(formData.FirstName);
            logger.Information($"    First Name: {formData.FirstName}");

            await LastName_Input.FillAsync(formData.LastName);
            logger.Information($"    Last Name: {formData.LastName}");

            await Email_Input.FillAsync(formData.Email);
            logger.Information($"    Email: {formData.Email}");

            await Phone_Input.FillAsync(formData.Phone);
            logger.Information($"    Phone: {formData.Phone}");

            // Interested In (topic)
            await InterestedIn_Select.SelectOptionAsync(new SelectOptionValue { Label = formData.InterestedIn });
            logger.Information($"    Interested In: {formData.InterestedIn}");

            // How did you hear about us? (source)
            await ReferralSource_Select.SelectOptionAsync(new SelectOptionValue { Label = formData.ReferralSource });
            logger.Information($"    Referral Source: {formData.ReferralSource}");

            // Marketing agreement checkbNoBrand
            await SelectMarketingAgreementCheckbNoBrand();

            logger.Information("    Lead form filled successfully.");
        }

        /// <summary>
        ///     Submit the lead form. Redirects to a separate Thank You URL on success.
        /// </summary>
        public async Task SubmitLeadForm()
        {
            logger.Information("Submitting lead form...");
            logger.Information("    Submit button enabled: {Enabled}", await Submit_Button.IsEnabledAsync());
            await SubmitAndAwaitLeadPost(Submit_Button);
        }

        /// <summary>
        ///     Submit the form via a native JS click and wait for the lead POST (/api/contact-form) to actually fire, retrying the click up to two more times if the submission stays silent. On the NoBrandBC template the submit handler routes through an invisible reCAPTCHA execute whose first token fetch can stall silently with no error, no validation message, and no network traffic.
        /// </summary>
        private async Task SubmitAndAwaitLeadPost(ILocator submitButton)
        {
            const string LeadEndpointFragment = "/api/contact-form";

            for (int attempt = 1; attempt <= 3; attempt++)
            {
                try
                {
                    IResponse response = await page.RunAndWaitForResponseAsync(
                        async () =>
                        {
                            if (attempt == 1)
                            {
                                // First attempt is a TRUSTED pointer click: the invisible reCAPTCHA needs a real user gesture on a fresh headless session before it will issue tokens.
                                try
                                {
                                    await submitButton.ClickAsync(new LocatorClickOptions
                                    {
                                        Timeout = TestConstants.ShortWaitSeconds * 1000
                                    });
                                    logger.Information("    Submit button clicked (attempt 1, trusted click).");
                                }
                                catch (Exception)
                                {
                                    logger.Warning("    Trusted click blocked/intercepted — falling back to native JS click.");
                                    await submitButton.EvaluateAsync("el => el.click()");
                                }
                            }
                            else
                            {
                                // Follow-up attempts re-run the handler once the reCAPTCHA client is warm (its first execute can stall silently); the token is cached by now.
                                await submitButton.EvaluateAsync("el => el.click()");
                                logger.Information("    Submit button clicked (attempt {Attempt}, native JS click).", attempt);
                            }
                        },
                        r => r.Request.Method == "POST" &&
                             r.Url.Contains(LeadEndpointFragment, StringComparison.OrdinalIgnoreCase),
                        new PageRunAndWaitForResponseOptions { Timeout = TestConstants.SubmitWaitSeconds * 1000 });

                    logger.Information("    Lead POST observed on attempt {Attempt}: {Status} {Url}",
                        attempt, response.Status, response.Url);
                    return;
                }
                catch (TimeoutException)
                {
                    logger.Warning("    No lead POST within {Seconds}s of submit click (attempt {Attempt}) — retrying.",
                        TestConstants.SubmitWaitSeconds, attempt);
                }
            }

            logger.Warning("    Lead POST never observed after 3 submit attempts; proceeding to verification.");
        }

        /// <summary>
        ///     Verify that the browser was navigated to the NoBrandBC Thank You confirmation page.
        /// </summary>
        /// <returns>True if the community-specific thank-you heading is visible; false otherwise.</returns>
        public async Task<bool> VerifyThankYouPageNavigation()
        {
            logger.Information("Verifying navigation to the NoBrandBC Thank You page...");

            // Tier 1: the dedicated Thank You page heading (form redirects on success).
            try
            {
                await Expect(ThankYouPageHeading.First).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
                {
                    Timeout = TestConstants.ShortWaitSeconds * 1000
                });
                logger.Information("    Thank-you heading found. Current URL: {Url}", page.Url);
                return true;
            }
            catch (Exception)
            {
                logger.Information("    XPath heading not found, trying a broader visible-text confirmation check...");
            }

            // Tier 2 (primary): any visible text carrying the confirmation phrase, regardless of DOM structure.
            try
            {
                await Expect(ThankYouConfirmationText.First).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
                {
                    Timeout = TestConstants.SubmitWaitSeconds * 1000
                });
                logger.Information("    Thank-you confirmation found via visible-text check. Current URL: {Url}", page.Url);
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "    No NoBrandBC thank-you confirmation found. Current URL: {Url}", page.Url);
                return false;
            }
        }


        //-- Private Helper Methods --//

        /// <summary>
        ///     Check the marketing agreement checkbNoBrand using a two-tier locator fallback scoped to the form.
        ///     A native input click bypasses the faux-checkbNoBrand overlay that sits above the hidden native input.
        /// </summary>
        private async Task SelectMarketingAgreementCheckbNoBrand()
        {
            logger.Information("    Checking marketing agreement checkbNoBrand...");

            // Tier 1: @name contains known consent/agreement patterns; else Tier 2: last checkbNoBrand within the form.
            ILocator checkbNoBrand = await MarketingConsent_CheckbNoBrand.CountAsync() > 0
                ? MarketingConsent_CheckbNoBrand
                : MarketingConsent_CheckbNoBrand_Fallback;

            if (await checkbNoBrand.IsCheckedAsync())
            {
                logger.Information("    Marketing agreement checkbNoBrand was already checked.");
                return;
            }

            // The checkbNoBrand is custom-styled: the real <input> is visually replaced by a faux overlay, so a normal click can't reach it. Dispatch a native .click() on the input itself — the Playwright equivalent of the original JS click — so the bound handler toggles the bNoBrand. This is the deliberate per-site JS-click retention for custom controls.
            await checkbNoBrand.EvaluateAsync("el => el.click()");
            logger.Information("    Marketing agreement checkbNoBrand checked (native input click).");
        }
    }
}

