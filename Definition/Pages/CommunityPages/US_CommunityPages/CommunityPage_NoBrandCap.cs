using Definition.TestData.FormData;
using Microsoft.Playwright;
using Serilog;
using Utility;
using static Microsoft.Playwright.Assertions;

namespace Definition.Pages.CommunityPages.US_CommunityPages
{
    public class CommunityPage_TheNoBrandCap(IPage page, ILogger logger)
    {
        private const string PageUrl = "https://uat.NoBrand.com";

        //-- General Page Elements --//
        private ILocator PageHeroHeading_Container => page.Locator("div.hero__content-copy.text--light p.eyebrow");

        //-- Cookie Consent Elements --//
        private ILocator AcceptCookies_Button => page.Locator("#truste-consent-button");

        //-- Lead Form Elements (scoped to the last form on the page) --//
        private ILocator LeadForm => page.Locator("form").Last;
        // .First mirrors the original Selenium FindElement(first-match) semantics — some of these id-substring selectors resolve to more than one element, which Playwright strict mode would otherwise reject.
        private ILocator FirstName_Input => LeadForm.Locator("input[id*='first-name']").First;
        private ILocator LastName_Input => LeadForm.Locator("input[id*='last-name']").First;
        private ILocator Email_Input => LeadForm.Locator("input[id*='email']").First;
        private ILocator Phone_Input => LeadForm.Locator("input[id*='phone']").First;
        private ILocator InterestedIn_Select => LeadForm.Locator("select[name='topic']").First;
        private ILocator Submit_Button => LeadForm.Locator("button[type='submit']").First;
        private ILocator LeadForm_ModuleArmed_Marker => LeadForm.Locator(".field--filled").First;

        // Marketing-agreement checkbNoBrand: prefer a consent/agreement-named checkbNoBrand, else the last checkbNoBrand.
        private ILocator NamedConsent_CheckbNoBrand =>
            page.Locator("input[type='checkbNoBrand'][name*='consent' i], input[type='checkbNoBrand'][name*='agreement' i]");
        private ILocator AnyCheckbNoBrand_Last => page.Locator("input[type='checkbNoBrand']").Last;

        //-- Thank You Confirmation --//
        private ILocator ConfirmationText =>
            page.GetByText("We will be in touch").Filter(new LocatorFilterOptions { Visible = true });

        //-- Page Methods --//

        /// <summary>
        ///     Verify that the NoBrandCap page loads as expected.
        /// </summary>
        /// <returns>True if the NoBrandCap page hero heading is visible.</returns>
        public async Task<bool> LoadNoBrandCapPage_VerifyHeading()
        {
            logger.Information("    Verifying heading is displayed on page...");
            try
            {
                await Expect(PageHeroHeading_Container).ToBeVisibleAsync();
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
        ///     Navigate to The NoBrandCap community page (waits for DOMContentLoaded - suite navigation policy, see Utility.PageNavigation).
        /// </summary>
        public async Task NavigateToNoBrandCapPage()
        {
            logger.Information("Navigating to The NoBrandCap community page...");
            await page.GotoDomReadyAsync(PageUrl);
            logger.Information("    Successfully navigated to The NoBrandCap page.");
        }

        /// <summary>
        ///     Accept the cookie consent banner if visible. Non-fatal: swallows the not-found timeout.
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
        ///     Bring the lead form into view. (Playwright auto-scrolls before actions; this is kept for parity with the test flow and to surface the form for any lazy-loaded content.)
        /// </summary>
        public async Task ScrollToLeadForm()
        {
            logger.Information("Scrolling to lead form section...");
            await LeadForm.ScrollIntoViewIfNeededAsync();
            logger.Information("    Successfully scrolled to lead form.");
        }

        /// <summary>
        ///     Fill out all lead form fields with the provided test data.
        /// </summary>
        /// <param name="formData">Test data DTO containing all field values.</param>
        public async Task FillLeadForm(CP_TheNoBrandCap_FormData formData)
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

            await InterestedIn_Select.SelectOptionAsync(new SelectOptionValue { Label = formData.InterestedIn });
            logger.Information($"    Interested In: {formData.InterestedIn}");

            await SelectPreferredContact(formData.PreferredContact);
            await SelectMarketingAgreementCheckbNoBrand();

            logger.Information("    Lead form filled successfully.");
        }

        /// <summary>
        ///     Submit the lead form. Playwright auto-waits for the button to be actionable before clicking.
        /// </summary>
        public async Task SubmitLeadForm()
        {
            logger.Information("Submitting lead form...");
            await Expect(LeadForm_ModuleArmed_Marker).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
            {
                Timeout = TestConstants.DefaultWaitSeconds * 1000
            });
            logger.Information("    Lead form module armed ('field--filled' stamped): AJAX submit handler is bound.");
            logger.Information("    Submit button enabled: {Enabled}", await Submit_Button.IsEnabledAsync());
            await Submit_Button.ClickAsync();
            logger.Information("    Submit button clicked.");
        }

        /// <summary>
        ///     Verify that the in-page thank-you confirmation message has appeared. Auto-retries up to the submit timeout while the form is replaced by the confirmation.
        /// </summary>
        /// <returns>True if the confirmation message becomes visible; false otherwise.</returns>
        public async Task<bool> VerifyThankYouConfirmation()
        {
            logger.Information("Verifying in-page thank-you confirmation...");
            try
            {
                await Expect(ConfirmationText).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
                {
                    Timeout = TestConstants.SubmitWaitSeconds * 1000
                });
                logger.Information("    In-page thank-you confirmation is visible.");
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "    No in-page thank-you confirmation found. Current URL: {Url}", page.Url);
                return false;
            }
        }

        //-- Private Helper Methods --//

        /// <summary>
        ///     Select the Preferred Method of Contact radio. The input is a direct child of its label;  scoped to the last form and matched on exact label text to avoid other radio groups.
        /// </summary>
        private async Task SelectPreferredContact(string preferredContact)
        {
            logger.Information($"    Selecting Preferred Contact: {preferredContact}");
            // The radio is custom-styled: the real <input> is visually replaced, so clicking the input directly doesn't toggle it (Selenium bypassed this with a JS click). Click the wrapping <label> — native label→control association checks the radio like a user would.
            var radioLabel = LeadForm.Locator(
                $"xpath=.//label[normalize-space(.)='{preferredContact}' and ./input[@type='radio']]");
            await radioLabel.ClickAsync();
            logger.Information($"    Preferred Contact radio selected: '{preferredContact}'.");
        }

        /// <summary>
        ///     Check the marketing agreement checkbNoBrand (consent/agreement-named, else the last checkbNoBrand).
        ///     Clicks the wrapping label for custom-styled checkbNoBrandes; no-ops if already checked.
        /// </summary>
        private async Task SelectMarketingAgreementCheckbNoBrand()
        {
            logger.Information("    Checking marketing agreement checkbNoBrand...");
            ILocator checkbNoBrand = await NamedConsent_CheckbNoBrand.CountAsync() > 0
                ? NamedConsent_CheckbNoBrand.First
                : AnyCheckbNoBrand_Last;

            if (await checkbNoBrand.IsCheckedAsync())
            {
                logger.Information("    Marketing agreement checkbNoBrand was already checked.");
                return;
            }

            await checkbNoBrand.EvaluateAsync("el => el.click()");
            logger.Information("    Marketing agreement checkbNoBrand checked (native input click).");
        }
    }
}

