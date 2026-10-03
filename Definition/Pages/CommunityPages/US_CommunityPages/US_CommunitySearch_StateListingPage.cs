using Microsoft.Playwright;
using Serilog;
using static Microsoft.Playwright.Assertions;


namespace Definition.Pages.CommunityPages.US_CommunityPages
{
    public class US_CommunitySearch_StateListingPage(IPage page, ILogger logger)
    {
        //-- Page Elements --//
        private ILocator PageH1Heading_Text => page.Locator("xpath=//div[@class='community-results__hero']//h1[contains(text(),'Illinois')]").First;
        private ILocator CommunityResultsList_Container => page.Locator("xpath=//div[@class='community-results__results-list']").First;
        private ILocator CommunityResultsList_ListItem => page.Locator("xpath=//div[@class='community-results__item' and @data-id='{33BF98C7-D29F-4800-88BA-9C7532D8C082}']").First;



        //-- Page Methods --//


        /// <summary>
        ///     Verify that the Community Search State Listing page loads as expected.
        /// </summary>
        /// <returns>
        ///     Boolean value indicating if the Community Search State Listing page heading is visible.
        /// </returns>
        public async Task<bool> LoadCommunitySearchStateListingPage_VerifyHeading()
        {
            logger.Information("    Verifying heading is displayed on page...");

            try
            {
                await Expect(PageH1Heading_Text.First).ToBeVisibleAsync();
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
        ///     Verify that the Community Search State Listing page displays search results for search term.
        /// </summary>
        /// <returns>
        ///     Boolean value indicating if the Community Search State Listing page search term returns a specific community.
        /// </returns>
        public async Task<bool> LoadCommunitySearchStateListingPage_VerifySearchResults()
        {
            logger.Information("    Verifying search term is applied on page...");

            // Tier 1: confirm the results list rendered (auto-wait + scroll handled by Playwright on assert/action).
            try
            {
                await Expect(CommunityResultsList_Container).ToBeVisibleAsync();
                await CommunityResultsList_Container.ScrollIntoViewIfNeededAsync();
                logger.Information("    Scrolled to search results.");

            }
            catch (Exception ex)
            {
                logger.Error(ex, "    Failed to scroll to search results on the page.");
                return false;
            }

            // TODO: Refine locator to be more intentional (less flakey) to the community result item instead of env/community specific data-id.
            try
            {
                await Expect(CommunityResultsList_ListItem).ToBeVisibleAsync();
                logger.Information("    Search term is applied on page and community found.");
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "    Failed to verify search term is applied and find specific community on the page.");
                return false;
            }


        }



    }
}

