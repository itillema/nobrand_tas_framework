using Microsoft.Playwright;
using Serilog;
using static Microsoft.Playwright.Assertions;

namespace Definition.Pages.NoBrandactionersPages
{
    public class NoBrandactionersPage(IPage page, ILogger logger)
    {
        //-- Page Elements --//
        private ILocator PageH1Heading_Text => page.Locator("xpath=//h1[@class='masthead-common-content__heading' and text()='NoBrandactioners at NoBrand']").First;



        //-- Page Methods --//

        /// <summary>
        ///     Verify NoBrandactioners page loads.
        /// </summary>
        /// <returns>
        ///     Boolean value indicating if the NoBrandactioners page heading is visible.
        /// </returns>
        public async Task<bool> LoadNoBrandactionersPage_VerifyHeading()
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

