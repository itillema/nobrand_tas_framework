using Definition.TestData.FormData;
using Microsoft.Playwright;
using Serilog;
using Utility;
using static Microsoft.Playwright.Assertions;

namespace Definition.Pages.CommunityPages.US_CommunityPages
{
    public class CommunityPage_NoBrandOfNoBrandIS(IPage page, ILogger logger)
    {
        private const string PageUrl = "https://uat.NoBrand.com/";

        // Scope prefix used to bind every lead-form field to the inline "Get in Touch" form on the page.
        // The modal "GET IN TOUCH" popup form uses the same field names, so anchoring to the first form that follows the inline "Get in Touch" heading avoids any locator collision.
        private const string FormScopeXPath = "(//h2[normalize-space(.)='Get in Touch']/following::form)[1]";


        //-- Cookie Consent Elements --//
        private ILocator AcceptCookies_Button => page.Locator("xpath=//button[@id='truste-consent-button']");


        //-- Lead Form Elements --//
        // The inline lead form is the first <form> that follows the "Get in Touch" heading. All inline fields are scoped off this ILocator so they can never collide with the modal form's fields.
        // .First mirrors the original Selenium FindElement(first-match) semantics — the id-substring contains() selectors can resolve to more than one element, which Playwright strict mode rejects.
        private ILocator FormHeading => page.Locator("xpath=//h2[normalize-space(.)='Get in Touch']");
        private ILocator LeadForm => page.Locator($"xpath={FormScopeXPath}");
        private ILocator FirstName_Input => LeadForm.Locator("xpath=.//input[contains(@id,'first-name')]").First;
        private ILocator LastName_Input => LeadForm.Locator("xpath=.//input[contains(@id,'last-name')]").First;
        private ILocator Email_Input => LeadForm.Locator("xpath=.//input[contains(@id,'email')]").First;
        private ILocator Phone_Input => LeadForm.Locator("xpath=.//input[contains(@id,'phone')]").First;
        private ILocator InterestedIn_Select => LeadForm.Locator("xpath=.//select[@name='topic']").First;
        private ILocator ReferralSource_Select => LeadForm.Locator("xpath=.//select[@name='source']").First;
        private ILocator MarketingConsent_CheckbNoBrand => LeadForm.Locator("xpath=.//input[@type='checkbNoBrand' and (contains(@name,'Consent') or contains(@name,'consent') or contains(@name,'agreement') or contains(@name,'smsConsent'))]").First;
        private ILocator MarketingConsent_CheckbNoBrand_Fallback => LeadForm.Locator("xpath=.//input[@type='checkbNoBrand']").Last;
        private ILocator Submit_Button => LeadForm.Locator("xpath=.//button[@type='submit']").First;


        //-- Thank You Confirmation Elements --//
        // The page can carry multiple hidden confirmation templates (one per lead form), so the bare text match resolves to several elements. Filtering to the VISIBLE one targets the single confirmation that the submitted form reveals in-place — and side-steps the false-positive the original code hand-rolled a stale-element + JS-innerText dance to avoid. Auto-retries to appear.
        private ILocator ThankYouHeader =>
            page.GetByText("Thank you for your interest in NoBrand").Filter(new LocatorFilterOptions { Visible = true });


        //-- Limited Availability Section Elements (modal form trigger) --//

        // The 'Limited Availability' status badge lives inside a pricing/overview card. The card also contains a 'GET IN TOUCH' anchor that opens the modal form. The scope picks the closest ancestor containing both so the button on other page sections is never matched.
        private const string LimitedAvailabilityScopeXPath =
            "(//*[normalize-space(.)='Limited Availability']" +
            "/ancestor::*[.//a[contains(@href,'#contact') or contains(translate(normalize-space(.),'abcdefghijklmnopqrstuvwxyz','ABCDEFGHIJKLMNOPQRSTUVWXYZ'),'GET IN TOUCH')]])[1]";

        private ILocator LimitedAvailability_Container => page.Locator($"xpath={LimitedAvailabilityScopeXPath}");
        private ILocator LimitedAvailability_GetInTouch_Button => page.Locator(
            $"xpath={LimitedAvailabilityScopeXPath}//a[contains(@href,'#contact') " +
            $"or contains(translate(normalize-space(.),'abcdefghijklmnopqrstuvwxyz','ABCDEFGHIJKLMNOPQRSTUVWXYZ'),'GET IN TOUCH')]").First;


        //-- Modal Lead Form Elements (Lead Form B) --//

        // When the modal opens, its form is injected at the end of the DOM (after the inline form), so the LAST <form> is the modal form. The inline form remains at its original position as the first <form>. The CLOSE button persists even after the form is replaced by the thank-you block, so it doubles as the reliable "modal is open" and "modal is still on screen" signal.
        private ILocator AllForms => page.Locator("form");
        private ILocator Modal_FormContainer => page.Locator("form").Last;
        private ILocator Modal_FirstName_Input => Modal_FormContainer.Locator("xpath=.//input[contains(@id,'first-name')]").First;
        private ILocator Modal_LastName_Input => Modal_FormContainer.Locator("xpath=.//input[contains(@id,'last-name')]").First;
        private ILocator Modal_Email_Input => Modal_FormContainer.Locator("xpath=.//input[contains(@id,'email')]").First;
        private ILocator Modal_Phone_Input => Modal_FormContainer.Locator("xpath=.//input[contains(@id,'phone')]").First;
        private ILocator Modal_InterestedIn_Select => Modal_FormContainer.Locator("xpath=.//select[@name='topic']").First;
        private ILocator Modal_ReferralSource_Select => Modal_FormContainer.Locator("xpath=.//select[@name='source']").First;
        private ILocator Modal_MarketingConsent_CheckbNoBrand => Modal_FormContainer.Locator("xpath=.//input[@type='checkbNoBrand' and (contains(@name,'Consent') or contains(@name,'consent') or contains(@name,'agreement') or contains(@name,'smsConsent'))]").First;
        private ILocator Modal_MarketingConsent_CheckbNoBrand_Fallback => Modal_FormContainer.Locator("xpath=.//input[@type='checkbNoBrand']").Last;
        private ILocator Modal_Submit_Button => Modal_FormContainer.Locator("xpath=.//button[@type='submit']").First;

        // After the modal form is replaced, the thank-you heading appears on the page. The inline form is NOT submitted in this test, so any visible matching heading came from the modal — filtering to the visible one targets the single confirmation the modal reveals.
        private ILocator ThankYouHeader_PageWide =>
            page.Locator("xpath=//*[self::h1 or self::h2 or self::h3][contains(normalize-space(.),'Thank you for your interest in NoBrand')]")
                .Filter(new LocatorFilterOptions { Visible = true });


        //-- Page Methods --//

        /// <summary>
        ///     Navigate to the NoBrand of NoBrandIS community page (waits for DOMContentLoaded - suite navigation policy, see Utility.PageNavigation).
        /// </summary>
        public async Task NavigateToNoBrandISPage()
        {
            logger.Information("Navigating to the NoBrand of NoBrandIS community page...");
            await page.GotoDomReadyAsync(PageUrl);
            logger.Information("    Successfully navigated to the NoBrand of NoBrandIS page.");
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
        ///     Bring the "Get in Touch" heading and its inline lead form into view. (Playwright auto-scrolls before actions; this is kept for parity with the test flow and to surface lazy-loaded content.)
        /// </summary>
        public async Task ScrollToLeadForm()
        {
            logger.Information("Scrolling to the inline 'Get in Touch' lead form...");
            await FormHeading.First.ScrollIntoViewIfNeededAsync();
            logger.Information("    Successfully scrolled to lead form.");
        }

        /// <summary>
        ///     Fill out all inline lead form fields with the provided test data and check the marketing agreement.
        /// </summary>
        /// <param name="formData">Test data DTO containing all field values.</param>
        public async Task FillLeadForm(CP_NoBrandOfNoBrandIS_FormData formData)
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

            await ReferralSource_Select.SelectOptionAsync(new SelectOptionValue { Label = formData.ReferralSource });
            logger.Information($"    Referral Source: {formData.ReferralSource}");

            await SelectMarketingAgreementCheckbNoBrand(MarketingConsent_CheckbNoBrand, MarketingConsent_CheckbNoBrand_Fallback);

            logger.Information("    Lead form filled successfully.");
        }

        /// <summary>
        ///     Submit the inline lead form. Playwright auto-waits for the button to be actionable before clicking.
        /// </summary>
        public async Task SubmitLeadForm()
        {
            logger.Information("Submitting lead form...");
            logger.Information("    Submit button enabled: {Enabled}", await Submit_Button.IsEnabledAsync());
            await SubmitAndAwaitLeadPost(Submit_Button);
            logger.Information($"    Current URL after submit: {page.Url}");
        }

        /// <summary>
        ///     Verify that the in-page thank-you confirmation message has appeared. Auto-retries up to the submit timeout while the form is replaced by the confirmation. Does NOT check for a URL change — this form replaces itself in-place.
        /// </summary>
        /// <returns>True if a confirmation element becomes visible; false otherwise.</returns>
        public async Task<bool> VerifyThankYouConfirmation()
        {
            logger.Information("Verifying in-page thank-you confirmation...");
            try
            {
                await Expect(ThankYouHeader.First).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
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


        //-- Modal Lead Form Methods (Lead Form B) --//

        /// <summary>
        ///     Bring the 'Limited Availability' container in the pricing/overview card into view.
        /// </summary>
        public async Task ScrollToLimitedAvailability()
        {
            logger.Information("Scrolling to the 'Limited Availability' container section...");
            await LimitedAvailability_Container.ScrollIntoViewIfNeededAsync();
            logger.Information("    Successfully scrolled to the 'Limited Availability' container.");
        }

        /// <summary>
        ///     Click the 'GET IN TOUCH' button inside the 'Limited Availability' container and wait for the pop-up modal lead form to become visible.
        /// </summary>
        public async Task OpenLeadFormModal()
        {
            logger.Information("Opening the lead form modal via the 'GET IN TOUCH' button...");
            await LimitedAvailability_GetInTouch_Button.ClickAsync();
            logger.Information("    'GET IN TOUCH' clicked.");

            // The page hosts several always-present <form> elements, so a fixed form-count check is not a reliable "modal opened" signal. Instead wait for the modal form itself (the last <form>, injected at the end of the DOM) to become visible — that is the unambiguous signal.
            await Expect(Modal_FormContainer).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
            {
                Timeout = TestConstants.DefaultWaitSeconds * 1000
            });
            logger.Information("    Modal lead form is visible.");
        }

        /// <summary>
        ///     Fill out the modal lead form with the provided test data and check the marketing agreement.
        /// </summary>
        /// <param name="formData">Test data DTO containing all field values.</param>
        public async Task FillLeadFormModal(CP_NoBrandOfNoBrandIS_FormData formData)
        {
            logger.Information("Filling out modal lead form with test data...");

            await Modal_FirstName_Input.FillAsync(formData.FirstName);
            logger.Information($"    First Name: {formData.FirstName}");

            await Modal_LastName_Input.FillAsync(formData.LastName);
            logger.Information($"    Last Name: {formData.LastName}");

            await Modal_Email_Input.FillAsync(formData.Email);
            logger.Information($"    Email: {formData.Email}");

            await Modal_Phone_Input.FillAsync(formData.Phone);
            logger.Information($"    Phone: {formData.Phone}");

            await Modal_InterestedIn_Select.SelectOptionAsync(new SelectOptionValue { Label = formData.InterestedIn });
            logger.Information($"    Interested In: {formData.InterestedIn}");

            await Modal_ReferralSource_Select.SelectOptionAsync(new SelectOptionValue { Label = formData.ReferralSource });
            logger.Information($"    Referral Source: {formData.ReferralSource}");

            await SelectMarketingAgreementCheckbNoBrand(Modal_MarketingConsent_CheckbNoBrand, Modal_MarketingConsent_CheckbNoBrand_Fallback);

            logger.Information("    Modal lead form filled successfully.");
        }

        /// <summary>
        ///     Submit the modal lead form. Playwright auto-waits for the button to be actionable before clicking.
        /// </summary>
        public async Task SubmitLeadFormModal()
        {
            logger.Information("Submitting modal lead form...");
            logger.Information("    Modal submit button enabled: {Enabled}", await Modal_Submit_Button.IsEnabledAsync());
            await SubmitAndAwaitLeadPost(Modal_Submit_Button);
            logger.Information($"    Current URL after submit: {page.Url}");
        }

        /// <summary>
        ///     Verify that the thank-you confirmation message has appeared after submitting the modal form.
        ///     This test only submits the modal form, so any visible "Thank you for your interest in NoBrand" heading on the page after submission is proof the modal submission succeeded.
        /// </summary>
        /// <returns>True if the confirmation is visible; false otherwise.</returns>
        public async Task<bool> VerifyThankYouConfirmationInModal()
        {
            logger.Information("Verifying in-modal thank-you confirmation...");

            // Tier 1 (quick): visible heading element check (short timeout).
            try
            {
                await Expect(ThankYouHeader_PageWide.First).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
                {
                    Timeout = TestConstants.ShortWaitSeconds * 1000
                });
                logger.Information("    Confirmation heading found.");
                return true;
            }
            catch (Exception)
            {
                logger.Information("    Heading not found, trying a broader visible-text check...");
            }

            // Tier 2 (primary): any visible "Thank you for your interest in NoBrand" text — survives non-heading wrappers (the Playwright equivalent of the original JS page-text scan).
            try
            {
                await Expect(ThankYouHeader.First).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
                {
                    Timeout = TestConstants.SubmitWaitSeconds * 1000
                });
                logger.Information("    Confirmation text found via broader visible-text check.");
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "    No thank-you confirmation text found. Current URL: {Url}", page.Url);
                return false;
            }
        }


        //-- Private Helper Methods --//

        /// <summary>
        ///     Check the marketing agreement checkbNoBrand using a two-tier locator fallback scoped to the given form.
        ///     Prefers a consent/agreement-named checkbNoBrand, else the last checkbNoBrand in the form. Dispatches a native .click() on the input (the Playwright equivalent of the original JS click) for the  custom-styled, React-controlled, visually hidden control; no-ops if already checked.
        /// </summary>
        private async Task SelectMarketingAgreementCheckbNoBrand(ILocator named, ILocator fallback)
        {
            logger.Information("    Checking marketing agreement checkbNoBrand...");

            // Tier 1: @name contains known consent/agreement patterns. Tier 2: last checkbNoBrand in the form.
            ILocator checkbNoBrand = await named.CountAsync() > 0 ? named : fallback;
            if (await named.CountAsync() == 0)
            {
                logger.Information("    Named checkbNoBrand not found, falling back to last checkbNoBrand in the form.");
            }

            if (await checkbNoBrand.IsCheckedAsync())
            {
                logger.Information("    Marketing agreement checkbNoBrand was already checked.");
                return;
            }

            // The smsConsent checkbNoBrand is a React-controlled input that is visually hidden (no hit bNoBrand, so it can't be force-clicked) and wrapped in a label whose only visible content is consent copy containing links — clicking the label text could hit a link and navigate away. So we dispatch a native .click() on the input itself (the Playwright equivalent of the original Selenium JS click); React's onChange handles it and toggles the bNoBrand.
            await checkbNoBrand.EvaluateAsync("el => el.click()");
            logger.Information("    Marketing agreement checkbNoBrand checked (native input click).");
        }

        /// <summary>
        ///     Submit the form via a native JS click and wait for the lead POST (/api/contact-form) to actually fire, retrying the click up to two more times if the submission stays silent.
        ///     Why: on this (NoBrandMan) template the submit handler routes through an invisible reCAPTCHA execute whose first token fetch can stall silently — no error, no validation message, and no network traffic. Network capture showed a repeat click once the client is warm completes the full pipeline (reCAPTCHA exchange -> POST /api/contact-form -> success:true). The JS click is the deliberate retention of the original Selenium submit for this template.
        ///     Non-fatal on total silence — the caller's verification step decides the test outcome.
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
    }
}

