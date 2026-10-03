using Microsoft.Playwright;
using Serilog;
using static Microsoft.Playwright.Assertions;

namespace Definition.Pages.CommunityPages.CA_CommunityPages
{
    public class CommunityPage_NoBrandOfOakville(IPage page, ILogger logger)
    {
        //-- Page Elements --//
        private ILocator Subheading => page.Locator("xpath=//h2[contains(text(),'NoBrand of Ok')]").First;

        //-- Page Methods --//

        /// <summary>
        ///     Verify that the NoBrand of Ok community page loads as expected.
        /// </summary>
        /// <returns>True if the NoBrand of Ok subheading is visible.</returns>
        public async Task<bool> LoadNoBrandofOkCommunityPage_VerifySubheading()
        {
            logger.Information("    Verifying subheading is displayed on page...");
            try
            {
                await Expect(Subheading.First).ToBeVisibleAsync();
                logger.Information("    Subheading is displayed on page.");
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "    Failed to verify subheading is displayed on the page.");
                return false;
            }
        }
    }
}

