using Microsoft.Playwright;
using Serilog;
using Utility;
using static Microsoft.Playwright.Assertions;

namespace Definition.Pages.ResourcePages
{
    public class ResourcesPage_SearchResults(IPage page, ILogger logger)
    {

        //-- Header and Navigation Menu Elements --//
        private ILocator SearchResults_H1Heading => page.Locator("xpath=//h1[text()='Search Results']").First;
        private ILocator SearchInputField_Input_TextInput => page.Locator("xpath=//input[@class='button' and @value='Search']").First;
        private ILocator SearchButton_Button => page.Locator("xpath=//input[@class='button' and @value='Search']").First;
        private ILocator SearchResults_Container => page.Locator("xpath=//div[@class='blog-listing-grid__grid']").First;
        private ILocator SearchResults_List => page.Locator(".blog-listing-grid__grid .blog-card, .blog-listing-grid__grid .video-resource-card");




        // -- Page Methods --//

        /// <summary>
        ///     Verify users see search results on the Resources Results page.
        /// </summary>
        /// <returns>
        ///     A boolean indicating whether search results are displayed.
        /// </returns>
        public async Task<bool> ResourceResults_VerifySearchResultsAreDisplayed()
        {
            logger.Information("Verifying users can see search results on the Resources Results page...");

            try
            {
                // Check that heading and hero information is displayed on the search results page
                await Expect(SearchResults_H1Heading).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
                {
                    Timeout = TestConstants.DefaultWaitSeconds * 1000
                });
                logger.Information("    Successfully verified search results heading is displayed.");

                // Bring the search results section into view (Playwright auto-scrolls before actions; kept for parity).
                await SearchResults_Container.ScrollIntoViewIfNeededAsync();
                logger.Information("    Successfully scrolled to the search results section.");

            }

            catch (Exception ex)
            {
                logger.Error(ex, "    Search results container not found.");
                throw new Exception("Search results container not found.", ex);
            }

            // The result cards render via AJAX after the heading appears; CountAsync() does NOT auto-wait, so wait for the first card to be visible before counting (web-first, auto-retrying).
            try
            {
                await Expect(SearchResults_List.First).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
                {
                    Timeout = TestConstants.DefaultWaitSeconds * 1000
                });
                int resultCount = await SearchResults_List.CountAsync();
                logger.Information($"    Check Passed: Search returned {resultCount} results.");
                return true;
            }
            catch (Exception)
            {
                logger.Error("    No search results were found.");
                return false;
            }

        }


    }
}

