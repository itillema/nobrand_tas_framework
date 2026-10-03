using Microsoft.Playwright;
using Serilog;
using static Microsoft.Playwright.Assertions;

namespace Definition.Pages.AboutPages
{
    public class LeadershipPage(IPage page, ILogger logger)
    {
        //-- Page Elements --//
        private ILocator PageH1Heading_Text => page.Locator("xpath=//h1[@class='masthead-common-content__heading' and text()='Our Leadership']").First;


        //-- Page Methods --//


        /// <summary>
        ///     Verify Leadership page loads.
        /// </summary>
        /// <returns>
        ///     Boolean value indicating whether the Leadership page heading is visible.
        /// </returns>
        public async Task<bool> LoadLeadershipPage_VerifyHeading()
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


