using Definition.TestData.FormData;
using Microsoft.Playwright;
using Serilog;
using Utility;
using static Microsoft.Playwright.Assertions;

namespace Definition.Pages.CommunityPages.US_CommunityPages
{
    public class CommunityPage_NoBrandEFS(IPage page, ILogger logger)
    {
        private const string PageUrl = "https://uat.NoBrand.com";
        private const string InquirePageUrl = "https://uat.NoBrand.com";

        // Scope prefix binds every lead-form field to the single inline "Get In Touch with NoBrand at NoBrandEFSth" form.
        // The page has only one form, but scoping to <section id="Contact-Us"> keeps locators unambiguous if additional forms are added in future.
        private const string FormScopeXPath = "//section[@id='Contact-Us']//form[contains(@class,'contact-us__form')]";

        // Inquire page hosts the contact-us widget inside <div id="Contact-Us"> (not <section>), but with the same uppercase id. Same field @name attributes; heading copy and tag differ (h1 on Inquire).
        private const string InquireFormScopeXPath = "//div[@id='Contact-Us']//form[contains(@class,'contact-us__form')]";


        //-- Page Elements --//
        private ILocator PageH1Heading_Text => page.Locator("xpath=//div[@class='media-masthead__content']//h1[@class='media-masthead__heading']").First;


        //-- Cookie Consent Elements --//
        private ILocator AcceptCookies_Button => page.Locator("#onetrust-accept-btn-handler");


        //-- Lead Form Elements --//
        // Field @id attributes include a random GUID, so locators target the stable @name attributes.
        // .First mirrors the original Selenium FindElement(first-match) semantics so Playwright strict mode doesn't reject a selector that happens to resolve to more than one element.
        private ILocator FormHeading => page.Locator("xpath=//h2[contains(@class,'contact-us__heading') and normalize-space(.)='Get In Touch with NoBrand at NoBrandEFSth']").First;
        private ILocator FirstName_Input => page.Locator($"xpath={FormScopeXPath}//input[@name='firstname']").First;
        private ILocator LastName_Input => page.Locator($"xpath={FormScopeXPath}//input[@name='lastname']").First;
        private ILocator Email_Input => page.Locator($"xpath={FormScopeXPath}//input[@name='emailAddress']").First;
        private ILocator Phone_Input => page.Locator($"xpath={FormScopeXPath}//input[@name='phoneNumber']").First;
        private ILocator Zip_Input => page.Locator($"xpath={FormScopeXPath}//input[@name='zipCode']").First;
        private ILocator InterestedIn_Select => page.Locator($"xpath={FormScopeXPath}//select[@name='topic']").First;
        private ILocator DeluxeSuites_CheckbNoBrand => page.Locator($"xpath={FormScopeXPath}//input[@type='checkbNoBrand' and @name='moreinformation']").First;
        private ILocator MarketingConsent_CheckbNoBrand => page.Locator($"xpath={FormScopeXPath}//input[@type='checkbNoBrand' and @name='smsConsent']").First;
        private ILocator MarketingConsent_CheckbNoBrand_Fallback => page.Locator($"xpath=({FormScopeXPath}//input[@type='checkbNoBrand'])[last()]").First;
        private ILocator Submit_Button => page.Locator($"xpath={FormScopeXPath}//button[@type='submit']").First;


        //-- Thank You Confirmation Elements --//
        // The form replaces itself in-place with a success block (no URL change). Filter to the visible confirmation so a hidden template doesn't false-positive (the original hand-rolled a stale-element + JS-innerText dance to dodge that; the web-first assertion + Visible filter is the equivalent).
        private ILocator ThankYouHeader =>
            page.Locator("xpath=//div[contains(@class,'contact-us__success')]//*[contains(@class,'contact-us__success-heading')]")
                .Filter(new LocatorFilterOptions { Visible = true });
        private ILocator ThankYouText =>
            page.GetByText("Thank you for your interest in NoBrand at NoBrandEFSth")
                .Filter(new LocatorFilterOptions { Visible = true });


        //-- Inquire Page: Form B Elements --//
        // Inquire page hosts a single contact-us form under <div id="Contact-Us">. Field @name attributes are identical to Form A; field @id attributes embed a per-render GUID. Heading is an <h1> with different copy.
        private ILocator InquireFormHeading => page.Locator("xpath=//*[contains(@class,'contact-us__heading') and normalize-space(.)='Take a Tour. Reserve a Residence.']").First;
        private ILocator InquireFormContainer => page.Locator($"xpath={InquireFormScopeXPath}").First;
        private ILocator Inquire_FirstName_Input => page.Locator($"xpath={InquireFormScopeXPath}//input[@name='firstname']").First;
        private ILocator Inquire_LastName_Input => page.Locator($"xpath={InquireFormScopeXPath}//input[@name='lastname']").First;
        private ILocator Inquire_Email_Input => page.Locator($"xpath={InquireFormScopeXPath}//input[@name='emailAddress']").First;
        private ILocator Inquire_Phone_Input => page.Locator($"xpath={InquireFormScopeXPath}//input[@name='phoneNumber']").First;
        private ILocator Inquire_Zip_Input => page.Locator($"xpath={InquireFormScopeXPath}//input[@name='zipCode']").First;
        private ILocator Inquire_InterestedIn_Select => page.Locator($"xpath={InquireFormScopeXPath}//select[@name='topic']").First;
        // On the Inquire page the @name='moreinformation' checkbNoBrand carries Founders Club copy rather than Deluxe Suites copy.
        private ILocator Inquire_MoreInformation_CheckbNoBrand => page.Locator($"xpath={InquireFormScopeXPath}//input[@type='checkbNoBrand' and @name='moreinformation']").First;
        private ILocator Inquire_MarketingConsent_CheckbNoBrand => page.Locator($"xpath={InquireFormScopeXPath}//input[@type='checkbNoBrand' and @name='smsConsent']").First;
        private ILocator Inquire_MarketingConsent_CheckbNoBrand_Fallback => page.Locator($"xpath=({InquireFormScopeXPath}//input[@type='checkbNoBrand'])[last()]").First;
        private ILocator Inquire_Submit_Button => page.Locator($"xpath={InquireFormScopeXPath}//button[@type='submit']").First;


        //-- Inquire Page: Thank You Confirmation Elements --//
        private ILocator InquireThankYouHeader =>
            page.Locator("xpath=//div[@id='Contact-Us']//div[contains(@class,'contact-us__success')]//*[contains(@class,'contact-us__success-heading')]")
                .Filter(new LocatorFilterOptions { Visible = true });
        private ILocator InquireThankYouText =>
            page.GetByText("Someone will be in touch with you shortly")
                .Filter(new LocatorFilterOptions { Visible = true });


        //-- Page Methods --//

        /// <summary>
        ///     Verify that the NoBrandEFS page loads as expected.
        /// </summary>
        /// <returns>
        ///     Boolean value indicating if the NoBrandEFS page heading is visible.
        /// </returns>
        public async Task<bool> LoadNoBrandEFSPage_VerifyHeading()
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
        ///     Navigate to the NoBrand at NoBrandEFSth community page (waits for DOMContentLoaded - suite navigation policy, see Utility.PageNavigation).
        /// </summary>
        public async Task NavigateToNoBrandEFSPage()
        {
            logger.Information("Navigating to the NoBrand at NoBrandEFSth community page...");
            await page.GotoDomReadyAsync(PageUrl);
            logger.Information("    Successfully navigated to the NoBrand at NoBrandEFSth page.");
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
        ///     Bring the "Get In Touch with NoBrand at NoBrandEFSth" heading and its lead form into view.
        ///     (Playwright auto-scrolls before actions; kept for parity with the test flow.)
        /// </summary>
        public async Task ScrollToLeadForm()
        {
            logger.Information("Scrolling to the 'Get In Touch with NoBrand at NoBrandEFSth' lead form...");
            await FormHeading.ScrollIntoViewIfNeededAsync();
            logger.Information("    Successfully scrolled to lead form.");
        }

        /// <summary>
        ///     Fill out all lead form fields with the provided test data, then check the optional deluxe-suites information checkbNoBrand and the required marketing agreement checkbNoBrand.
        /// </summary>
        /// <param name="formData">Test data DTO containing all field values.</param>
        public async Task FillLeadForm(CP_NoBrandEFS_FormData formData)
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

            await Zip_Input.FillAsync(formData.Zip);
            logger.Information($"    Zip: {formData.Zip}");

            // I'm interested in (topic)
            await InterestedIn_Select.SelectOptionAsync(new SelectOptionValue { Label = formData.InterestedIn });
            logger.Information($"    Interested In: {formData.InterestedIn}");

            // Deluxe Suites optional checkbNoBrand
            await CheckDeluxeSuitesCheckbNoBrand();

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
        }

        /// <summary>
        ///     Verify that the in-page thank-you confirmation message has appeared.
        ///     Does NOT check for URL change — this form replaces itself in-place.
        /// </summary>
        /// <returns>True if a confirmation element is visible; false otherwise.</returns>
        public async Task<bool> VerifyThankYouConfirmation()
        {
            logger.Information("Verifying in-page thank-you confirmation...");

            // Tier 1: the dedicated success-heading element inside the in-place confirmation block.
            try
            {
                await Expect(ThankYouHeader.First).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
                {
                    Timeout = TestConstants.ShortWaitSeconds * 1000
                });
                logger.Information("    Confirmation element found via success-heading locator.");
                return true;
            }
            catch (Exception)
            {
                logger.Information("    Success-heading confirmation not found, checking for the success copy by text...");
            }

            // Tier 2: the exact success copy rendered anywhere visible on the page.
            try
            {
                await Expect(ThankYouText.First).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
                {
                    Timeout = TestConstants.SubmitWaitSeconds * 1000
                });
                logger.Information("    Confirmation found via visible page text check.");
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
        ///     Navigate to the NoBrand at NoBrandEFSth Inquire sub-page (waits for DOMContentLoaded - suite navigation policy, see Utility.PageNavigation).
        /// </summary>
        public async Task NavigateToNoBrandEFSInquirePage()
        {
            logger.Information("Navigating to the NoBrand at NoBrandEFSth Inquire page...");
            await page.GotoDomReadyAsync(InquirePageUrl);
            logger.Information("    Successfully navigated to the NoBrand at NoBrandEFSth Inquire page.");
        }

        /// <summary>
        ///     Bring the 'Take a Tour. Reserve a Residence.' Inquire form heading and its lead form into view.
        ///     (Playwright auto-scrolls before actions; kept for parity with the test flow.)
        /// </summary>
        public async Task ScrollToInquireLeadForm()
        {
            logger.Information("Scrolling to the 'Take a Tour. Reserve a Residence.' Inquire lead form...");
            await InquireFormHeading.ScrollIntoViewIfNeededAsync();
            logger.Information("    Successfully scrolled to Inquire lead form.");
        }

        /// <summary>
        ///     Fill out the Inquire page lead form (Form B) with the provided test data, then check the optional 'more information' (Founders Club) checkbNoBrand and the required marketing agreement checkbNoBrand.
        /// </summary>
        /// <param name="formData">Test data DTO containing all field values.</param>
        public async Task FillInquireLeadForm(CP_NoBrandEFS_FormData formData)
        {
            logger.Information("Filling out Inquire lead form with test data...");

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

            await CheckInquireMoreInformationCheckbNoBrand();

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
        }

        /// <summary>
        ///     Verify that the in-page thank-you confirmation message for the Inquire form has appeared.
        ///     Does NOT check for URL change — this form replaces itself in-place.
        /// </summary>
        /// <returns>True if a confirmation element is visible; false otherwise.</returns>
        public async Task<bool> VerifyInquireThankYouConfirmation()
        {
            logger.Information("Verifying in-page Inquire thank-you confirmation...");

            // Tier 1: the dedicated success-heading element inside the Inquire in-place confirmation block.
            try
            {
                await Expect(InquireThankYouHeader.First).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
                {
                    Timeout = TestConstants.ShortWaitSeconds * 1000
                });
                logger.Information("    Confirmation element found via success-heading locator.");
                return true;
            }
            catch (Exception)
            {
                logger.Information("    Success-heading confirmation not found, checking for the success copy by text...");
            }

            // Tier 2: the Inquire form's exact success copy rendered anywhere visible on the page.
            try
            {
                await Expect(InquireThankYouText.First).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
                {
                    Timeout = TestConstants.SubmitWaitSeconds * 1000
                });
                logger.Information("    Confirmation found via visible page text check.");
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
        ///     Check the optional 'I'd like more information about the deluxe suites at NoBrand at NoBrandEFSth' checkbNoBrand.
        ///     The faux-checkbNoBrand overlay (div.field__faux-checkbNoBrand) sits above the native input, so we dispatch a native .click() on the input itself (the Playwright equivalent of the original JS click); no-ops if checked.
        /// </summary>
        private async Task CheckDeluxeSuitesCheckbNoBrand()
        {
            logger.Information("    Checking deluxe suites information checkbNoBrand...");

            if (await DeluxeSuites_CheckbNoBrand.IsCheckedAsync())
            {
                logger.Information("    Deluxe suites checkbNoBrand was already checked.");
                return;
            }

            await DeluxeSuites_CheckbNoBrand.EvaluateAsync("el => el.click()");
            logger.Information("    Deluxe suites checkbNoBrand checked (native input click).");
        }

        /// <summary>
        ///     Check the marketing agreement (SMS consent) checkbNoBrand using a two-tier locator fallback scoped to the inline form. Native input .click() bypasses the faux-checkbNoBrand overlay.
        /// </summary>
        private async Task SelectMarketingAgreementCheckbNoBrand()
        {
            logger.Information("    Checking marketing agreement checkbNoBrand...");

            // Tier 1: target by known @name='smsConsent'. Tier 2: last checkbNoBrand within the inline form.
            ILocator checkbNoBrand = await MarketingConsent_CheckbNoBrand.CountAsync() > 0
                ? MarketingConsent_CheckbNoBrand
                : MarketingConsent_CheckbNoBrand_Fallback;

            if (await MarketingConsent_CheckbNoBrand.CountAsync() == 0)
            {
                logger.Information("    Named checkbNoBrand not found, falling back to last checkbNoBrand in the form.");
            }

            if (await checkbNoBrand.IsCheckedAsync())
            {
                logger.Information("    Marketing agreement checkbNoBrand was already checked.");
                return;
            }

            await checkbNoBrand.EvaluateAsync("el => el.click()");
            logger.Information("    Marketing agreement checkbNoBrand checked (native input click).");
        }

        /// <summary>
        ///     Check the optional 'more information' checkbNoBrand on the Inquire page (Form B), which carries the 'NoBrand at NoBrandEFSth Founders Club' copy rather than the deluxe-suites copy used on Form A.
        ///     Uses @name='moreinformation' (identical to Form A). Native input .click() bypasses the faux-checkbNoBrand overlay.
        /// </summary>
        private async Task CheckInquireMoreInformationCheckbNoBrand()
        {
            logger.Information("    Checking Inquire more-information (Founders Club) checkbNoBrand...");

            if (await Inquire_MoreInformation_CheckbNoBrand.IsCheckedAsync())
            {
                logger.Information("    Inquire more-information checkbNoBrand was already checked.");
                return;
            }

            await Inquire_MoreInformation_CheckbNoBrand.EvaluateAsync("el => el.click()");
            logger.Information("    Inquire more-information checkbNoBrand checked (native input click).");
        }

        /// <summary>
        ///     Check the marketing agreement (SMS consent) checkbNoBrand on the Inquire page (Form B), using a two-tier locator fallback scoped to the Inquire form. Native input .click() bypasses the faux-checkbNoBrand overlay.
        /// </summary>
        private async Task SelectInquireMarketingAgreementCheckbNoBrand()
        {
            logger.Information("    Checking Inquire marketing agreement checkbNoBrand...");

            // Tier 1: target by known @name='smsConsent'. Tier 2: last checkbNoBrand within the Inquire form.
            ILocator checkbNoBrand = await Inquire_MarketingConsent_CheckbNoBrand.CountAsync() > 0
                ? Inquire_MarketingConsent_CheckbNoBrand
                : Inquire_MarketingConsent_CheckbNoBrand_Fallback;

            if (await Inquire_MarketingConsent_CheckbNoBrand.CountAsync() == 0)
            {
                logger.Information("    Named checkbNoBrand not found, falling back to last checkbNoBrand in the Inquire form.");
            }

            if (await checkbNoBrand.IsCheckedAsync())
            {
                logger.Information("    Inquire marketing agreement checkbNoBrand was already checked.");
                return;
            }

            await checkbNoBrand.EvaluateAsync("el => el.click()");
            logger.Information("    Inquire marketing agreement checkbNoBrand checked (native input click).");
        }
    }
}

