using Definition.TestData.FormData;
using Microsoft.Playwright;
using Serilog;
using Utility;
using static Microsoft.Playwright.Assertions;

namespace Definition.Pages.CommunityPages.US_CommunityPages
{
    public class CommunityPage_TheNoBrandAp(IPage page, ILogger logger)
    {
        private const string PageUrl = "https://uat.NoBrand.com";
        private const string InquirePageUrl = "https://uat.NoBrand.com";

        // Scope prefix binds every lead-form field to the single inline "GET IN TOUCH WITH THE NoBrandAp" form.
        // The page has only one form, but scoping to <section id="Contact-Us"> keeps locators unambiguous if additional forms are added in future.
        private const string FormScopeXPath = "//section[@id='Contact-Us']//form[contains(@class,'contact-us__form')]";

        // Inquire page uses the same CMS contact-us widget, but with a lowercase section id ('contact-us').
        private const string InquireFormScopeXPath = "//section[@id='contact-us']//form[contains(@class,'contact-us__form')]";


        //-- Page Elements --//
        private ILocator PageH1Heading_Text => page.Locator("xpath=//div[@class='media-masthead__bottom-content']//h1[@class='media-masthead__heading']").First;


        //-- Cookie Consent Elements --//
        private ILocator AcceptCookies_Button => page.Locator("#onetrust-accept-btn-handler");


        //-- Lead Form Elements --//
        // Field @id attributes include a random GUID, so locators target the stable @name attributes.
        private ILocator FormHeading => page.Locator("xpath=//h2[contains(@class,'contact-us__heading') and normalize-space(.)='GET IN TOUCH WITH THE NoBrandAp']").First;
        private ILocator FormContainer => page.Locator($"xpath={FormScopeXPath}").First;
        private ILocator FirstName_Input => page.Locator($"xpath={FormScopeXPath}//input[@name='firstname']").First;
        private ILocator LastName_Input => page.Locator($"xpath={FormScopeXPath}//input[@name='lastname']").First;
        private ILocator Email_Input => page.Locator($"xpath={FormScopeXPath}//input[@name='emailAddress']").First;
        private ILocator Phone_Input => page.Locator($"xpath={FormScopeXPath}//input[@name='phoneNumber']").First;
        private ILocator Zip_Input => page.Locator($"xpath={FormScopeXPath}//input[@name='zipCode']").First;
        private ILocator InterestedIn_Select => page.Locator($"xpath={FormScopeXPath}//select[@name='topic']").First;
        private ILocator FoundersClub_CheckbNoBrand => page.Locator($"xpath={FormScopeXPath}//input[@type='checkbNoBrand' and @name='moreinformation']").First;
        private ILocator MarketingConsent_CheckbNoBrand => page.Locator($"xpath={FormScopeXPath}//input[@type='checkbNoBrand' and @name='smsConsent']").First;
        private ILocator MarketingConsent_CheckbNoBrand_Fallback => page.Locator($"xpath=({FormScopeXPath}//input[@type='checkbNoBrand'])[last()]").First;
        private ILocator Submit_Button => page.Locator($"xpath={FormScopeXPath}//button[@type='submit']").First;


        //-- Thank You Confirmation Elements --//
        // The page carries hidden confirmation templates; filter to the VISIBLE confirmation that the submitted form reveals in-place. Form A's exact success copy is "Thank you for your interest in The NoBrandAp".
        private ILocator ConfirmationText =>
            page.GetByText("Thank you for your interest in The NoBrandAp").Filter(new LocatorFilterOptions { Visible = true });
        private ILocator ThankYouHeader =>
            page.Locator("xpath=//div[contains(@class,'contact-us__success')]//*[contains(@class,'contact-us__success-heading')]").Filter(new LocatorFilterOptions { Visible = true });


        //-- Inquire Page: Form B Elements --//
        // Inquire page hosts a single contact-us form under <section id="contact-us">.
        // Field @name attributes are identical to Form A; field @id attributes embed a per-render GUID.
        private ILocator InquireFormHeading => page.Locator("xpath=//section[@id='contact-us']//h2[contains(@class,'contact-us__heading') and normalize-space(.)='Discover The NoBrandAp']").First;
        private ILocator InquireFormContainer => page.Locator($"xpath={InquireFormScopeXPath}").First;
        private ILocator Inquire_FirstName_Input => page.Locator($"xpath={InquireFormScopeXPath}//input[@name='firstname']").First;
        private ILocator Inquire_LastName_Input => page.Locator($"xpath={InquireFormScopeXPath}//input[@name='lastname']").First;
        private ILocator Inquire_Email_Input => page.Locator($"xpath={InquireFormScopeXPath}//input[@name='emailAddress']").First;
        private ILocator Inquire_Phone_Input => page.Locator($"xpath={InquireFormScopeXPath}//input[@name='phoneNumber']").First;
        private ILocator Inquire_Zip_Input => page.Locator($"xpath={InquireFormScopeXPath}//input[@name='zipCode']").First;
        private ILocator Inquire_InterestedIn_Select => page.Locator($"xpath={InquireFormScopeXPath}//select[@name='topic']").First;
        private ILocator Inquire_FoundersClub_CheckbNoBrand => page.Locator($"xpath={InquireFormScopeXPath}//input[@type='checkbNoBrand' and @name='moreinformation']").First;
        private ILocator Inquire_MarketingConsent_CheckbNoBrand => page.Locator($"xpath={InquireFormScopeXPath}//input[@type='checkbNoBrand' and @name='smsConsent']").First;
        private ILocator Inquire_MarketingConsent_CheckbNoBrand_Fallback => page.Locator($"xpath=({InquireFormScopeXPath}//input[@type='checkbNoBrand'])[last()]").First;
        private ILocator Inquire_Submit_Button => page.Locator($"xpath={InquireFormScopeXPath}//button[@type='submit']").First;


        //-- Inquire Page: Thank You Confirmation Elements --//
        // Form B's exact success copy is "We appreciate your interest in The NoBrandAp".
        private ILocator InquireConfirmationText =>
            page.GetByText("We appreciate your interest in The NoBrandAp").Filter(new LocatorFilterOptions { Visible = true });
        private ILocator InquireThankYouHeader =>
            page.Locator("xpath=//section[@id='contact-us']//div[contains(@class,'contact-us__success')]//*[contains(@class,'contact-us__success-heading')]").Filter(new LocatorFilterOptions { Visible = true });


        //-- Page Methods --//

        /// <summary>
        ///     Verify that the NoBrandAp page loads as expected.
        /// </summary>
        /// <returns>
        ///     Boolean value indicating if the NoBrandAp page heading is visible.
        /// </returns>
        public async Task<bool> LoadNoBrandApPage_VerifyHeading()
        {
            logger.Information("    Verifying heading is displayed on page...");
            try
            {
                await Expect(PageH1Heading_Text).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
                {
                    Timeout = TestConstants.DefaultWaitSeconds * 1000
                });
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
        ///     Navigate to The NoBrandAp community page (waits for DOMContentLoaded - suite navigation policy, see Utility.PageNavigation).
        /// </summary>
        public async Task NavigateToNoBrandApPage()
        {
            logger.Information("Navigating to The NoBrandAp community page...");
            await page.GotoDomReadyAsync(PageUrl);
            logger.Information("    Successfully navigated to The NoBrandAp page.");
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
        ///     Scroll the page to bring the "GET IN TOUCH WITH THE NoBrandAp" heading and its lead form into view.
        /// </summary>
        public async Task ScrollToLeadForm()
        {
            logger.Information("Scrolling to the 'GET IN TOUCH WITH THE NoBrandAp' lead form...");
            await FormHeading.ScrollIntoViewIfNeededAsync();
            logger.Information("    Successfully scrolled to lead form.");
        }

        /// <summary>
        ///     Fill out all lead form fields with the provided test data, then check the optional Founders Club checkbNoBrand and the required marketing agreement checkbNoBrand.
        /// </summary>
        /// <param name="formData">Test data DTO containing all field values.</param>
        public async Task FillLeadForm(CP_TheNoBrandAp_FormData formData)
        {
            logger.Information("Filling out lead form with test data...");

            // Center the form in the viewport (Playwright auto-scrolls before actions; kept for parity).
            await FormContainer.ScrollIntoViewIfNeededAsync();

            // First Name
            await FirstName_Input.FillAsync(formData.FirstName);
            logger.Information($"    First Name: {formData.FirstName}");

            // Last Name
            await LastName_Input.FillAsync(formData.LastName);
            logger.Information($"    Last Name: {formData.LastName}");

            // Email
            await Email_Input.FillAsync(formData.Email);
            logger.Information($"    Email: {formData.Email}");

            // Phone
            await Phone_Input.FillAsync(formData.Phone);
            logger.Information($"    Phone: {formData.Phone}");

            // Zip
            await Zip_Input.FillAsync(formData.Zip);
            logger.Information($"    Zip: {formData.Zip}");

            // I'm interested in (topic)
            await InterestedIn_Select.SelectOptionAsync(new SelectOptionValue { Label = formData.InterestedIn });
            logger.Information($"    Interested In: {formData.InterestedIn}");

            // Founders Club optional checkbNoBrand
            await CheckFoundersClubCheckbNoBrand();

            // Marketing agreement checkbNoBrand
            await SelectMarketingAgreementCheckbNoBrand();

            logger.Information("    Lead form filled successfully.");
        }

        /// <summary>
        ///     Submit the inline lead form. Playwright auto-waits for the button to be actionable before clicking.
        /// </summary>
        public async Task SubmitLeadForm()
        {
            logger.Information("Submitting lead form...");
            logger.Information("    Submit button enabled: {Enabled}", await Submit_Button.IsEnabledAsync());
            await Submit_Button.ClickAsync();
            logger.Information("    Submit button clicked.");
            logger.Information($"    Current URL after submit: {page.Url}");
        }

        /// <summary>
        ///     Verify that the in-page thank-you confirmation message has appeared.
        ///     Does NOT check for URL change — this form replaces itself in-place.
        /// </summary>
        /// <returns>True if a confirmation element is visible; false otherwise.</returns>
        public async Task<bool> VerifyThankYouConfirmation()
        {
            logger.Information("Verifying in-page thank-you confirmation...");

            // Tier 1 (quick): the dedicated success heading element with a short timeout.
            try
            {
                await Expect(ThankYouHeader.First).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
                {
                    Timeout = TestConstants.ShortWaitSeconds * 1000
                });
                logger.Information("    Confirmation element found via success heading.");
                return true;
            }
            catch (Exception)
            {
                logger.Information("    Success-heading confirmation not found, checking for the success copy text...");
            }

            // Tier 2 (primary): the form's exact success copy rendered in-place (visible only).
            try
            {
                await Expect(ConfirmationText.First).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
                {
                    Timeout = TestConstants.SubmitWaitSeconds * 1000
                });
                logger.Information("    Confirmation found via success copy text.");
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "    No in-page thank-you confirmation found. Current URL: {Url}", page.Url);
                return false;
            }
        }


        //-- Inquire Page (Form B) Methods --//

        /// <summary>
        ///     Navigate to The NoBrandAp Inquire sub-page (waits for DOMContentLoaded - suite navigation policy, see Utility.PageNavigation).
        /// </summary>
        public async Task NavigateToNoBrandApInquirePage()
        {
            logger.Information("Navigating to The NoBrandAp Inquire page...");
            await page.GotoDomReadyAsync(InquirePageUrl);
            logger.Information("    Successfully navigated to The NoBrandAp Inquire page.");
        }

        /// <summary>
        ///     Scroll the page to bring the 'Discover The NoBrandAp' Form B heading and its lead form into view.
        /// </summary>
        public async Task ScrollToInquireLeadForm()
        {
            logger.Information("Scrolling to the 'Discover The NoBrandAp' lead form...");
            await InquireFormHeading.ScrollIntoViewIfNeededAsync();
            logger.Information("    Successfully scrolled to Inquire lead form.");
        }

        /// <summary>
        ///     Fill out the Inquire page lead form (Form B) with the provided test data, then check the optional Founders Club checkbNoBrand and the required marketing agreement checkbNoBrand.
        /// </summary>
        /// <param name="formData">Test data DTO containing all field values.</param>
        public async Task FillInquireLeadForm(CP_TheNoBrandAp_FormData formData)
        {
            logger.Information("Filling out Inquire lead form with test data...");

            await InquireFormContainer.ScrollIntoViewIfNeededAsync();

            await Inquire_FirstName_Input.FillAsync(formData.FirstName);
            logger.Information($"    First Name: {formData.FirstName}");

            await Inquire_LastName_Input.FillAsync(formData.LastName);
            logger.Information($"    Last Name: {formData.LastName}");

            await Inquire_Email_Input.FillAsync(formData.Email);
            logger.Information($"    Email: {formData.Email}");

            await Inquire_Phone_Input.FillAsync(formData.Phone);
            logger.Information($"    Phone: {formData.Phone}");

            await Inquire_Zip_Input.FillAsync(formData.Zip);
            logger.Information($"    Zip: {formData.Zip}");

            await Inquire_InterestedIn_Select.SelectOptionAsync(new SelectOptionValue { Label = formData.InterestedIn });
            logger.Information($"    Interested In: {formData.InterestedIn}");

            await CheckInquireFoundersClubCheckbNoBrand();

            await SelectInquireMarketingAgreementCheckbNoBrand();

            logger.Information("    Inquire lead form filled successfully.");
        }

        /// <summary>
        ///     Submit the Inquire lead form. Playwright auto-waits for the button to be actionable before clicking.
        /// </summary>
        public async Task SubmitInquireLeadForm()
        {
            logger.Information("Submitting Inquire lead form...");
            logger.Information("    Submit button enabled: {Enabled}", await Inquire_Submit_Button.IsEnabledAsync());
            await Inquire_Submit_Button.ClickAsync();
            logger.Information("    Submit button clicked.");
            logger.Information($"    Current URL after submit: {page.Url}");
        }

        /// <summary>
        ///     Verify that the in-page thank-you confirmation message for the Inquire form has appeared.
        ///     Does NOT check for URL change — this form replaces itself in-place.
        /// </summary>
        /// <returns>True if a confirmation element is visible; false otherwise.</returns>
        public async Task<bool> VerifyInquireThankYouConfirmation()
        {
            logger.Information("Verifying in-page Inquire thank-you confirmation...");

            // Tier 1 (quick): the dedicated success heading element with a short timeout.
            try
            {
                await Expect(InquireThankYouHeader.First).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
                {
                    Timeout = TestConstants.ShortWaitSeconds * 1000
                });
                logger.Information("    Confirmation element found via success heading.");
                return true;
            }
            catch (Exception)
            {
                logger.Information("    Success-heading confirmation not found, checking for the success copy text...");
            }

            // Tier 2 (primary): the Inquire form's exact success copy rendered in-place (visible only).
            try
            {
                await Expect(InquireConfirmationText.First).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
                {
                    Timeout = TestConstants.SubmitWaitSeconds * 1000
                });
                logger.Information("    Confirmation found via success copy text.");
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "    No in-page Inquire thank-you confirmation found. Current URL: {Url}", page.Url);
                return false;
            }
        }


        //-- Private Helper Methods --//

        /// <summary>
        ///     Check the optional 'I'd like more information about The NoBrandAp's Founders Club' checkbNoBrand.
        ///     The native input sits under a faux-checkbNoBrand overlay (div.field__faux-checkbNoBrand), so we dispatch a native .click() on the input itself (the Playwright equivalent of the original Selenium JS click).
        /// </summary>
        private async Task CheckFoundersClubCheckbNoBrand()
        {
            logger.Information("    Checking Founders Club checkbNoBrand...");

            if (await FoundersClub_CheckbNoBrand.IsCheckedAsync())
            {
                logger.Information("    Founders Club checkbNoBrand was already checked.");
                return;
            }

            await FoundersClub_CheckbNoBrand.EvaluateAsync("el => el.click()");
            logger.Information("    Founders Club checkbNoBrand checked via native input click.");
        }

        /// <summary>
        ///     Check the marketing agreement (SMS consent) checkbNoBrand using a two-tier locator fallback scoped to the inline form. A native input .click() bypasses the faux-checkbNoBrand overlay.
        /// </summary>
        private async Task SelectMarketingAgreementCheckbNoBrand()
        {
            logger.Information("    Checking marketing agreement checkbNoBrand...");

            ILocator checkbNoBrand;

            // Tier 1: target by known @name='smsConsent'; Tier 2: last checkbNoBrand within the inline form.
            if (await MarketingConsent_CheckbNoBrand.CountAsync() > 0)
            {
                checkbNoBrand = MarketingConsent_CheckbNoBrand;
            }
            else
            {
                logger.Information("    Named checkbNoBrand not found, falling back to last checkbNoBrand in the form.");
                checkbNoBrand = MarketingConsent_CheckbNoBrand_Fallback;
            }

            if (await checkbNoBrand.IsCheckedAsync())
            {
                logger.Information("    Marketing agreement checkbNoBrand was already checked.");
                return;
            }

            await checkbNoBrand.EvaluateAsync("el => el.click()");
            logger.Information("    Marketing agreement checkbNoBrand checked via native input click.");
        }

        /// <summary>
        ///     Check the optional Founders Club checkbNoBrand on the Inquire page (Form B). A native input .click() bypasses the faux-checkbNoBrand overlay.
        /// </summary>
        private async Task CheckInquireFoundersClubCheckbNoBrand()
        {
            logger.Information("    Checking Inquire Founders Club checkbNoBrand...");

            if (await Inquire_FoundersClub_CheckbNoBrand.IsCheckedAsync())
            {
                logger.Information("    Inquire Founders Club checkbNoBrand was already checked.");
                return;
            }

            await Inquire_FoundersClub_CheckbNoBrand.EvaluateAsync("el => el.click()");
            logger.Information("    Inquire Founders Club checkbNoBrand checked via native input click.");
        }

        /// <summary>
        ///     Check the marketing agreement (SMS consent) checkbNoBrand on the Inquire page (Form B), using a two-tier locator fallback scoped to the Inquire form. A native input .click() bypasses the faux-checkbNoBrand overlay.
        /// </summary>
        private async Task SelectInquireMarketingAgreementCheckbNoBrand()
        {
            logger.Information("    Checking Inquire marketing agreement checkbNoBrand...");

            ILocator checkbNoBrand;

            // Tier 1: target by known @name='smsConsent'; Tier 2: last checkbNoBrand within the Inquire form.
            if (await Inquire_MarketingConsent_CheckbNoBrand.CountAsync() > 0)
            {
                checkbNoBrand = Inquire_MarketingConsent_CheckbNoBrand;
            }
            else
            {
                logger.Information("    Named checkbNoBrand not found, falling back to last checkbNoBrand in the Inquire form.");
                checkbNoBrand = Inquire_MarketingConsent_CheckbNoBrand_Fallback;
            }

            if (await checkbNoBrand.IsCheckedAsync())
            {
                logger.Information("    Inquire marketing agreement checkbNoBrand was already checked.");
                return;
            }

            await checkbNoBrand.EvaluateAsync("el => el.click()");
            logger.Information("    Inquire marketing agreement checkbNoBrand checked via native input click.");
        }
    }
}

