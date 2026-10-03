using Definition.TestData.FormData;
using Microsoft.Playwright;
using Serilog;
using System.Globalization;
using System.Text.RegularExpressions;
using Utility;
using static Microsoft.Playwright.Assertions;

namespace Definition.Pages.HomePages
{
    public class BookATour_DropdownMenu(IPage page, ILogger logger)
    {
        // Panel scope — the Book a Tour dropdown is identified by data-id='header-dropdown-tour'.
        // The `--active` modifier class is added when the panel is open.
        private const string DropdownScopeXPath = "//div[@data-id='header-dropdown-tour']";
        private const string DropdownActiveXPath = "//div[@data-id='header-dropdown-tour' and contains(@class,'header__dropdown--active')]";

        // Form scope — narrows locators to the Book a Tour contact-form specifically, since the panel also hosts the hidden success block and the Now Viewing / Call sidebar.
        private const string FormScopeXPath = "//div[@data-id='header-dropdown-tour']//form[contains(@class,'contact-form')]";

        // The tour-time label actually selected during FillBookATourLeadForm (the configured label, or the first-available fallback). Remembered so SubmitBookATourLeadForm can re-assert/re-select it if a late async repopulation of the time options wipes the selection — without any method-signature change.
        private string? _selectedTourTimeLabel;


        //-- Trigger / Panel Elements --//
        private ILocator BookATourTrigger_Link => page.Locator("#header-book-a-tour-trigger").First;
        private ILocator DropdownPanelActive_Container => page.Locator($"xpath={DropdownActiveXPath}").First;


        //-- Now Viewing Elements --//
        private ILocator NowViewing_Heading => page.Locator($"xpath={DropdownScopeXPath}//div[contains(@class,'header__dropdown-current')]//h3[normalize-space()='Now Viewing']").First;
        private ILocator NowViewing_CommunityLink => page.Locator($"xpath={DropdownScopeXPath}//a[contains(@class,'header__communities-item') and contains(@href,'/communities/il/NoBrand-of-lincoln-park')]").First;
        private ILocator NowViewing_CommunityName_Text => page.Locator($"xpath={DropdownScopeXPath}//a[contains(@class,'header__communities-item')]//p[contains(@class,'header__communities-info-title')]").First;


        //-- Call Elements --//
        private ILocator Call_Heading => page.Locator($"xpath={DropdownScopeXPath}//div[contains(@class,'header__dropdown-current')]//h4[normalize-space()='Call']").First;
        private ILocator Call_PricingAndAvailability_Link => page.Locator($"xpath={DropdownScopeXPath}//div[contains(@class,'header__dropdown-current-phone')]//a[contains(.,'Pricing & Availability')]").First;
        private ILocator Call_ResidentsAndFamily_Link => page.Locator($"xpath={DropdownScopeXPath}//div[contains(@class,'header__dropdown-current-phone')]//a[contains(.,'Residents & Family')]").First;


        //-- Lead Form Elements --//
        private ILocator FormContainer => page.Locator($"xpath={FormScopeXPath}").First;
        private ILocator FirstName_Input => page.Locator($"xpath={FormScopeXPath}//input[@name='firstName']").First;
        private ILocator LastName_Input => page.Locator($"xpath={FormScopeXPath}//input[@name='lastName']").First;
        private ILocator Email_Input => page.Locator($"xpath={FormScopeXPath}//input[@name='emailAddress']").First;
        private ILocator Phone_Input => page.Locator($"xpath={FormScopeXPath}//input[@name='phoneNumber']").First;
        private ILocator PreferredTourDate_Input => page.Locator($"xpath={FormScopeXPath}//input[@name='preferredDate']").First;
        private ILocator PreferredTourTime_Select => page.Locator($"xpath={FormScopeXPath}//select[@name='preferredTime']").First;
        private ILocator HearAboutUs_Select => page.Locator($"xpath={FormScopeXPath}//select[@name='source']").First;
        private ILocator MarketingConsent_CheckbNoBrand => page.Locator($"xpath={FormScopeXPath}//input[@type='checkbNoBrand' and @name='smsConsent']").First;
        private ILocator Submit_Button => page.Locator($"xpath={FormScopeXPath}//button[@type='submit']").First;
        private ILocator ValidationErrors => page.Locator($"xpath={DropdownScopeXPath}//*[contains(@class,'pristine-error') or (contains(@class,'invalid') and not(self::form))]");


        //-- Datepicker (qs-datepicker) Elements --//
        // qs-datepicker appends its calendar container to the bound input's parentElement, so this form's calendar lives inside the form scope — chaining off FormContainer means another picker instance elsewhere on the page can never collide. ':not(.qs-hidden)' + the Visible filter match only the OPEN calendar (closed instances stay in the DOM with the qs-hidden class).
        private ILocator DatePicker_Container =>
            FormContainer.Locator("div.qs-datepicker-container:not(.qs-hidden)")
                         .Filter(new LocatorFilterOptions { Visible = true }).First;

        private ILocator DatePicker_Month_Text => DatePicker_Container.Locator("span.qs-month");
        private ILocator DatePicker_Year_Text => DatePicker_Container.Locator("span.qs-year");
        private ILocator DatePicker_NextMonth_Arrow => DatePicker_Container.Locator("div.qs-arrow.qs-right");
        private ILocator DatePicker_PrevMonth_Arrow => DatePicker_Container.Locator("div.qs-arrow.qs-left");

        // Selectable day-number squares only: excludes the weekday headers (qs-day), leading/trailing blanks (qs-empty), disabled days (qs-disabled), and adjacent-month days (qs-outside-current-month).
        private ILocator DatePicker_EnabledDaySquares =>
            DatePicker_Container.Locator("div.qs-square.qs-num:not(.qs-disabled):not(.qs-empty):not(.qs-outside-current-month)");

        // Exact-text day match — anchored regex so day '7' can never match '17' or '27'.
        private ILocator DatePicker_EnabledDay(int day) =>
            DatePicker_EnabledDaySquares.Filter(new LocatorFilterOptions { HasTextRegex = new Regex($@"^\s*{day}\s*$") });


        //-- Thank You Elements --//
        // Pre-rendered in the DOM as <div class="form__success">…</div> — hidden until submit succeeds.
        // The visible filter (not just existence) is required so we don't get a false positive before submit.
        private ILocator ThankYou_Container =>
            page.Locator($"xpath={DropdownScopeXPath}//div[contains(@class,'form__success')]").Filter(new LocatorFilterOptions { Visible = true });
        private ILocator ThankYou_Heading =>
            page.Locator($"xpath={DropdownScopeXPath}//div[contains(@class,'form__success')]//h2[contains(normalize-space(),'Thank you for your interest')]").Filter(new LocatorFilterOptions { Visible = true });


        //-- Trigger / Panel Methods --//

        /// <summary>
        ///     Click the header 'Book a Tour' trigger and wait for the dropdown panel to become active.
        /// </summary>
        public async Task OpenBookATourDropdown()
        {
            logger.Information("Opening the Book a Tour header dropdown...");

            try
            {
                // Playwright auto-scrolls and auto-waits for actionability before clicking — no manual scrollIntoView/JS-click needed for this normal trigger button.
                await BookATourTrigger_Link.ClickAsync();
                logger.Information("    Book a Tour trigger clicked.");

                await Expect(DropdownPanelActive_Container).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
                {
                    Timeout = TestConstants.DefaultWaitSeconds * 1000
                });
                logger.Information("    Book a Tour dropdown panel is active and visible.");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "    Failed to open the Book a Tour dropdown.");
                throw;
            }
        }

        /// <summary>
        ///     Verify the Book a Tour dropdown panel has expanded (header__dropdown--active class is present and the panel is displayed).
        /// </summary>
        /// <returns>True if the active panel is visible; false otherwise.</returns>
        public async Task<bool> VerifyDropdownIsExpanded()
        {
            logger.Information("Verifying the Book a Tour dropdown is expanded...");

            try
            {
                await Expect(DropdownPanelActive_Container).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
                {
                    Timeout = TestConstants.DefaultWaitSeconds * 1000
                });
                logger.Information("    Book a Tour dropdown is expanded. Visible: TRUE");
                return true;
            }
            catch (Exception ex)
            {
                logger.Warning(ex, "    Timeout waiting for the Book a Tour dropdown to expand.");
                return false;
            }
        }

        /// <summary>
        ///     Verify the lead form renders inside the dropdown. Checks the form container plus a first input and the submit button so a partial render is caught.
        /// </summary>
        /// <returns>True if the form container, first-name input, and submit button are all displayed; false otherwise.</returns>
        public async Task<bool> VerifyLeadFormIsVisible()
        {
            logger.Information("Verifying the Book a Tour lead form is visible inside the dropdown...");

            try
            {
                await Expect(FormContainer).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
                {
                    Timeout = TestConstants.DefaultWaitSeconds * 1000
                });
                await Expect(FirstName_Input).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
                {
                    Timeout = TestConstants.DefaultWaitSeconds * 1000
                });
                await Expect(Submit_Button).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
                {
                    Timeout = TestConstants.DefaultWaitSeconds * 1000
                });
                logger.Information("    Book a Tour lead form visibility check complete. Visible: TRUE");
                return true;
            }
            catch (Exception ex)
            {
                logger.Warning(ex, "    Timeout waiting for the Book a Tour lead form to be visible.");
                return false;
            }
        }


        //-- Now Viewing / Call Verification Methods --//

        /// <summary>
        ///     Verify the Now Viewing section shows the NoBrand of NoBrandLP community link: heading visible, link visible, community name text matches "NoBrand of NoBrandLP" (case-insensitive), and the link href points to the NoBrandLP community page.
        /// </summary>
        /// <returns>True if every sub-check passes; false otherwise.</returns>
        public async Task<bool> VerifyNowViewingSection_NoBrandOfNoBrandLP()
        {
            logger.Information("Verifying 'Now Viewing' section shows NoBrand of NoBrandLP...");
            const string ExpectedName = "NoBrand of NoBrandLP";
            const string ExpectedHrefFragment = "/communities/il/NoBrand-of-lincoln-park";

            try
            {
                await Expect(NowViewing_Heading).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
                {
                    Timeout = TestConstants.DefaultWaitSeconds * 1000
                });

                await Expect(NowViewing_CommunityLink).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
                {
                    Timeout = TestConstants.DefaultWaitSeconds * 1000
                });

                // The community name sits inside a <p class="header__communities-info-title"> — targeting it directly avoids dragging along the <img> alt text in the link's rendered text.
                string nameText = (await NowViewing_CommunityName_Text.InnerTextAsync() ?? string.Empty).Trim();
                if (!string.Equals(nameText, ExpectedName, StringComparison.OrdinalIgnoreCase))
                {
                    logger.Warning($"    Community name text mismatch. Expected (ci): '{ExpectedName}', Actual: '{nameText}'.");
                    return false;
                }

                string href = await NowViewing_CommunityLink.GetAttributeAsync("href") ?? string.Empty;
                if (!href.Contains(ExpectedHrefFragment, StringComparison.OrdinalIgnoreCase))
                {
                    logger.Warning($"    Community link href does not contain expected fragment. Expected fragment: '{ExpectedHrefFragment}', Actual href: '{href}'.");
                    return false;
                }

                logger.Information($"    'Now Viewing' section verified. Text: '{nameText}', href contains '{ExpectedHrefFragment}'.");
                return true;
            }
            catch (Exception ex)
            {
                logger.Warning(ex, "    Timeout waiting for 'Now Viewing' section elements.");
                return false;
            }
        }

        /// <summary>
        ///     Verify the Call section renders: heading visible plus both phone-number links visible.
        ///     Does not assert phone-number values, since numbers may change per community/campaign.
        /// </summary>
        /// <returns>True if the heading and both links are displayed; false otherwise.</returns>
        public async Task<bool> VerifyCallSection()
        {
            logger.Information("Verifying 'Call' section shows both phone-number links...");

            try
            {
                await Expect(Call_Heading).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
                {
                    Timeout = TestConstants.DefaultWaitSeconds * 1000
                });
                await Expect(Call_PricingAndAvailability_Link).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
                {
                    Timeout = TestConstants.DefaultWaitSeconds * 1000
                });
                await Expect(Call_ResidentsAndFamily_Link).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
                {
                    Timeout = TestConstants.DefaultWaitSeconds * 1000
                });
                logger.Information("    'Call' section verified. Pricing & Availability and Residents & Family links visible.");
                return true;
            }
            catch (Exception ex)
            {
                logger.Warning(ex, "    Timeout waiting for 'Call' section elements.");
                return false;
            }
        }


        //-- Lead Form Methods --//

        /// <summary>
        ///     Fill the Book a Tour lead form fields with the provided test data and check the marketing consent checkbNoBrand.
        /// </summary>
        /// <param name="formData">Test data DTO containing all field values.</param>
        public async Task FillBookATourLeadForm(BookATour_FormData formData)
        {
            logger.Information("Filling out Book a Tour lead form with test data...");

            try
            {
                // Playwright auto-scrolls before actions; bring the form into view for parity with the original flow.
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

                // Phone — the field has a keystroke-driven mask/validator ("Please enter 10 digits"); FillAsync sets the value in one shot and doesn't satisfy it, so type it character-by-character.
                await Phone_Input.PressSequentiallyAsync(formData.Phone);
                await Phone_Input.BlurAsync();
                logger.Information($"    Phone: {formData.Phone}");

                // Preferred Tour Date
                await SelectDateViaDatePicker(formData.PreferredTourDate);
                logger.Information($"    Preferred Tour Date: {formData.PreferredTourDate}");

                // Preferred Tour Time 
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

                // How did you hear about us? — maps to <select name="source">.
                await HearAboutUs_Select.SelectOptionAsync(new SelectOptionValue { Label = formData.HearAboutUs });
                logger.Information($"    How did you hear about us?: {formData.HearAboutUs}");

                // Marketing consent checkbNoBrand
                await SelectMarketingAgreementCheckbNoBrand();

                logger.Information("    Book a Tour lead form filled successfully.");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "    Failed to fill the Book a Tour lead form.");
                throw;
            }
        }

        /// <summary>
        ///     Submit the Book a Tour lead form. Treats appearance of the form__success block as the success signal (this form swaps the content region in-place rather than redirecting or unmounting the whole form).
        /// </summary>
        public async Task SubmitBookATourLeadForm()
        {
            logger.Information("Submitting the Book a Tour lead form...");

            try
            {
                // Guard against a late async repopulation wiping the tour-time selection between fill and submit.
                await EnsureTourTimeSelectionSurvived();

                int preValidationCount = await ValidationErrors.CountAsync();
                if (preValidationCount > 0)
                {
                    logger.Warning($"    Found {preValidationCount} potential validation indicators before submit.");
                    for (int i = 0; i < Math.Min(5, preValidationCount); i++)
                    {
                        ILocator err = ValidationErrors.Nth(i);
                        if (await err.IsVisibleAsync())
                        {
                            string text = (await err.InnerTextAsync() ?? string.Empty).Trim();
                            if (!string.IsNullOrWhiteSpace(text))
                                logger.Warning($"      - {text}");
                        }
                    }
                }

                logger.Information("    Submit button enabled: {Enabled}", await Submit_Button.IsEnabledAsync());

                // Playwright auto-scrolls and auto-waits for actionability — a plain click replaces the original JS scrollIntoView + JS click.
                await Submit_Button.ClickAsync();
                logger.Information("    Submit button clicked.");

                // Success signal: the form__success block becomes visible. Web-first wait auto-retries.
                try
                {
                    await Expect(ThankYou_Container).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
                    {
                        Timeout = TestConstants.SubmitWaitSeconds * 1000
                    });
                    logger.Information("    Form submission response detected (success block visible).");
                }
                catch (Exception)
                {
                    logger.Warning("    Timeout waiting for submission response. Continuing to verification.");
                }

                int postValidationCount = await ValidationErrors.CountAsync();
                if (postValidationCount > 0)
                {
                    logger.Error($"    Found {postValidationCount} validation errors after submit:");
                    for (int i = 0; i < Math.Min(5, postValidationCount); i++)
                    {
                        ILocator err = ValidationErrors.Nth(i);
                        if (await err.IsVisibleAsync())
                        {
                            string text = (await err.InnerTextAsync() ?? string.Empty).Trim();
                            if (!string.IsNullOrWhiteSpace(text))
                                logger.Error($"      - {text}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "    Failed to submit the Book a Tour lead form.");
                throw;
            }
        }

        /// <summary>
        ///     Verify the in-place thank-you confirmation has replaced the Book a Tour form content.
        ///     Waits for the pre-rendered form__success block (hidden on page load) to become visible.
        /// </summary>
        /// <returns>True if the success heading is visible; false otherwise.</returns>
        public async Task<bool> VerifyThankYouConfirmation()
        {
            logger.Information("Verifying in-place Book a Tour thank-you confirmation...");

            try
            {
                await Expect(ThankYou_Heading).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
                {
                    Timeout = TestConstants.SubmitWaitSeconds * 1000
                });
                logger.Information($"    Thank-you confirmation visible. Text: {await ThankYou_Heading.InnerTextAsync()}");
                return true;
            }
            catch (Exception ex)
            {
                logger.Warning(ex, "    Timeout waiting for thank-you confirmation to become visible.");
                return false;
            }
        }


        //-- Private Helper Methods --//

        /// <summary>
        ///     Drive the real qs-datepicker UI: click the input (the library's delegated document handler opens the calendar on input click/focusin), navigate to the target month, and click the day square.
        ///     This fires the picker's internal selectDay → onSelect, which is what the site hooks to repopulate the Preferred Tour Time slots — JS value-assignment + synthetic events never reach that callback.
        /// </summary>
        private async Task SelectDateViaDatePicker(string dateValue)
        {
            DateTime target = DateTime.ParseExact(dateValue, "MM/dd/yyyy", CultureInfo.InvariantCulture);

            // 1) Open the calendar. The library shows it on click OR focusin of the bound input, so if the click is swallowed (e.g., an icon overlay intercepts the pointer), focus is the fallback.
            try
            {
                await PreferredTourDate_Input.ClickAsync(new LocatorClickOptions
                {
                    Timeout = TestConstants.ShortWaitSeconds * 1000
                });
            }
            catch (Exception)
            {
                logger.Warning("    Date input click was blocked/intercepted; opening the calendar via focus instead.");
            }

            try
            {
                await Expect(DatePicker_Container).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
                {
                    Timeout = TestConstants.ShortWaitSeconds * 1000
                });
            }
            catch (Exception)
            {
                logger.Warning("    Calendar did not open on click; retrying via focus (library also shows on focusin).");
                await PreferredTourDate_Input.FocusAsync();
                try
                {
                    await Expect(DatePicker_Container).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
                    {
                        Timeout = TestConstants.ShortWaitSeconds * 1000
                    });
                }
                catch (Exception)
                {
                    await LogDatePickerDomDiagnostic();
                    throw;
                }
            }

            // 2) Navigate to the target month/year. The target is today+7, so at most one forward click is ever needed; the loop tolerates a couple and fails loudly if the header diverges.
            for (int attempt = 0; ; attempt++)
            {
                string monthName = (await DatePicker_Month_Text.InnerTextAsync() ?? string.Empty).Trim();
                string yearText = (await DatePicker_Year_Text.InnerTextAsync() ?? string.Empty).Trim();

                if (!DateTime.TryParseExact($"{monthName} {yearText}", "MMMM yyyy",
                        CultureInfo.GetCultureInfo("en-US"), DateTimeStyles.None, out DateTime displayed))
                {
                    await LogDatePickerDomDiagnostic();
                    throw new InvalidOperationException(
                        $"Could not parse datepicker header '{monthName} {yearText}' — customMonths in play?");
                }

                int monthDelta = ((target.Year - displayed.Year) * 12) + (target.Month - displayed.Month);
                if (monthDelta == 0) break;

                if (attempt >= 2)
                {
                    throw new InvalidOperationException(
                        $"Datepicker did not reach {target:MMMM yyyy}; still showing '{monthName} {yearText}'.");
                }

                await (monthDelta > 0 ? DatePicker_NextMonth_Arrow : DatePicker_PrevMonth_Arrow).ClickAsync();

                // Adjacent months never share a name, so the header text change is the settle signal.
                await Expect(DatePicker_Month_Text).Not.ToHaveTextAsync(monthName, new LocatorAssertionsToHaveTextOptions
                {
                    Timeout = TestConstants.ShortWaitSeconds * 1000
                });
            }

            // 3) Click the exact day. The library's click handler requires class qs-num and rejects qs-disabled, so the locator pre-filters to exactly what is clickable.
            ILocator dayCell = DatePicker_EnabledDay(target.Day);
            try
            {
                await Expect(dayCell).ToHaveCountAsync(1, new LocatorAssertionsToHaveCountOptions
                {
                    Timeout = TestConstants.ShortWaitSeconds * 1000
                });
            }
            catch (Exception)
            {
                await LogDatePickerDomDiagnostic();
                throw new InvalidOperationException(
                    $"Day {target.Day} in {target:MMMM yyyy} is not selectable in the Book a Tour datepicker " +
                    "(disabled, missing, or ambiguous). See the datepicker DOM diagnostic in the log.");
            }
            await dayCell.ClickAsync();   // selectDay → sets the input value, fires onSelect, hides the calendar

            // 4) Verify the picker committed a value. The library writes via its own formatter, so assert non-empty rather than an exact string, and log what it actually wrote.
            await Expect(PreferredTourDate_Input).ToHaveValueAsync(new Regex(@"\S"), new LocatorAssertionsToHaveValueOptions
            {
                Timeout = TestConstants.ShortWaitSeconds * 1000
            });
            await Expect(DatePicker_Container).Not.ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
            {
                Timeout = TestConstants.ShortWaitSeconds * 1000
            });
            logger.Information($"    Preferred Tour Date committed by picker: '{await PreferredTourDate_Input.InputValueAsync()}' (target: {dateValue}).");
        }

        /// <summary>
        ///     Log a truncated DOM dump of every qs-datepicker container on the page. Safety net for site-fork drift — if the deployed picker's structure ever diverges from the qs-datepicker reference DOM, the failing run shows what actually rendered.
        /// </summary>
        private async Task LogDatePickerDomDiagnostic()
        {
            try
            {
                string dump = await page.EvaluateAsync<string>(@"() => {
                    const els = [...document.querySelectorAll('.qs-datepicker-container')];
                    return 'qs-datepicker containers: ' + els.length + '\n' + els.map((e, i) =>
                        '#' + i + ' hidden=' + e.classList.contains('qs-hidden') + '\n' + e.outerHTML.slice(0, 1500)
                    ).join('\n---\n');
                }");
                logger.Warning("    Datepicker DOM diagnostic:\n{Dump}", dump);
            }
            catch (Exception ex)
            {
                logger.Warning(ex, "    Failed to capture datepicker DOM diagnostic.");
            }
        }

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
        ///     Check the marketing consent checkbNoBrand via a native input .click() (bypasses the faux-checkbNoBrand overlay the CMS layers on top of the native input — the Playwright equivalent of the original  Selenium JS click). No-op if already selected.
        /// </summary>
        private async Task SelectMarketingAgreementCheckbNoBrand()
        {
            logger.Information("    Checking marketing agreement checkbNoBrand...");

            if (await MarketingConsent_CheckbNoBrand.IsCheckedAsync())
            {
                logger.Information("    Marketing agreement checkbNoBrand was already checked.");
                return;
            }

            await MarketingConsent_CheckbNoBrand.EvaluateAsync("el => el.click()");
            logger.Information("    Marketing agreement checkbNoBrand checked via native input click.");
        }
    }
}


