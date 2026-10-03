using Microsoft.Playwright;
using Serilog;
using static Microsoft.Playwright.Assertions;

namespace Definition.Pages.CommunityPages.US_CommunityPages
{
    public class US_CommunityStateListingPage(IPage page, ILogger logger)
    {
        //-- Page Elements --//
        private ILocator PageH1Heading_Text => page.Locator("xpath=//h1[@class='with-divider' and text()='Community Locations']").First;



        //-- Page Methods --//


        /// <summary>
        ///     Verify that the Community State Listing page loads as expected.
        /// </summary>
        /// <returns>
        ///     Boolean value indicating if the Community State Listing page heading is visible.
        /// </returns>
        public async Task<bool> LoadCommunityStateListingPage_VerifyHeading()
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
    }
}

