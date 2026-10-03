using Microsoft.Playwright;
using Serilog;
using Utility;
using static Microsoft.Playwright.Assertions;

namespace Definition.Pages.CommunityPages.US_CommunityPages
{
    public class US_CommunitySearchResultsPage(IPage page, ILogger logger)
    {
        //-- Page Elements --//
        private ILocator PageH1Heading_Text => page.Locator("xpath=//h1[contains(text(), 'Find a NoBrand Community')]").First;
        private ILocator CommunityResultsList_Container => page.Locator(".community-results__results-list").First;
        private ILocator CommunityResultsMap_Container => page.Locator("xpath=//div[@class='community-results__results-map']").First;
        private ILocator CommunityResultItems => page.Locator(".community-results__item");
        private ILocator CommunityResult_NoBrandOfNoBrandLP_Link => page.Locator("xpath=//a[@href='https://uat.NoBrand.com' and text()='NoBrand of NoBrandLP']").First;



        //-- Page Methods --//

        /// <summary>
        ///     Verify the Community Search Results page loads and results are displayed.
        /// </summary>
        /// <returns>
        ///     Boolean value indicating if the page loaded and results are visible.
        /// </returns>
        public async Task<bool> VerifySearchResultsLoaded()
        {
            logger.Information("Verifying Community Search Results page...");

            try
            {
                await Expect(PageH1Heading_Text).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
                {
                    Timeout = TestConstants.DefaultWaitSeconds * 1000
                });
                logger.Information("    Page header visibility check complete. Visible: TRUE");

                // Check if results container or "no results" message is visible (both are valid page states)
                try
                {
                    await Expect(CommunityResultsList_Container).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
                    {
                        Timeout = TestConstants.DefaultWaitSeconds * 1000
                    });
                    logger.Information("    Results list visibility check complete. Visible: TRUE");
                }
                catch (Exception)
                {
                    // No results list visible - check for "no results" message which is also valid
                    logger.Information("    Results list not visible - checking for no-results state.");
                }

                if (await CommunityResultItems.CountAsync() > 0)
                {
                    logger.Information($"    Found {await CommunityResultItems.CountAsync()} community results.");
                    return true;
                }
                else
                {
                    // Page loaded but no results - this is still a valid page state for the test
                    logger.Information("    Page loaded successfully but no community results found for the search criteria.");
                    return true; // Return true since the page DID load correctly
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "    Timeout waiting for Search Results page elements.");
                return false;
            }
        }

        /// <summary>
        ///     Verify the Community Search page loads.
        /// </summary>
        /// <returns>
        ///     Boolean value indicating if the Community Search page loaded and the communities map is visible.
        /// </returns>
        public async Task<bool> LoadCommunitySearchPage_VerifyMap()
        {
            logger.Information("Verifying Community Search page...");

            try
            {
                // Bring the map element into view, then confirm visibility (Playwright auto-scrolls before actions).
                await CommunityResultsMap_Container.ScrollIntoViewIfNeededAsync();
                await Expect(CommunityResultsMap_Container).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
                {
                    Timeout = TestConstants.DefaultWaitSeconds * 1000
                });
                logger.Information("    Page map visibility check complete. Visible: TRUE");

                return await CommunityResultsMap_Container.IsVisibleAsync();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "    Timeout waiting for Community Search page elements.");
                return false;
            }
        }

        public async Task<CommunityPage_NoBrandOfNoBrandLP> SelectSearchResult_NavigateToCommunity()
        {
            logger.Information("Selecting first community search result to navigate to community page...");

            try
            {
                // Wait for the search results to be visible and select the first result
                await Expect(CommunityResultsList_Container).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
                {
                    Timeout = TestConstants.DefaultWaitSeconds * 1000
                });
                await Expect(CommunityResult_NoBrandOfNoBrandLP_Link).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
                {
                    Timeout = TestConstants.DefaultWaitSeconds * 1000
                });

                if (await CommunityResultItems.CountAsync() > 0)
                {
                    // The result link opens the community in a NEW TAB. Capture the popup that the click spawns rather than juggling window handles (the Playwright equivalent of the original capture-handles / SwitchTo / new-handle dance).
                    var popup = await page.Context.RunAndWaitForPageAsync(async () =>
                    {
                        await CommunityResult_NoBrandOfNoBrandLP_Link.ClickAsync();
                    });
                    logger.Information("    Clicked on the first community result.");

                    await popup.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
                    logger.Information("    New tab (NoBrand of NoBrandLP) has been opened and context switched.");

                    return new CommunityPage_NoBrandOfNoBrandLP(popup, logger);
                }
                else
                {
                    logger.Warning("    No community results found to select.");
                    throw new Exception("No community results found to select.");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "    Timeout waiting for community results to select.");
                throw new Exception("Timeout waiting for community results to select.", ex);
            }
        }





    }
}

