using Microsoft.Playwright;
using Serilog;
using static Microsoft.Playwright.Assertions;

namespace Definition.Pages.NoBrandactionAndLiving
{
    public class NoBrandactionAndLivingPage(IPage page, ILogger logger)
    {
        //-- Page Elements --//
        private ILocator PageH1Heading_Text => page.Locator("xpath=//h1[@class='masthead-common-content__heading' and normalize-space(.)='Say Hello to Living Your Way']").First;



        //-- Page Methods --//

        /// <summary>
        ///     Verify that the NoBrandaction and Living page loads as expected.
        /// </summary>
        /// <returns>
        ///     Boolean value indicating if the NoBrandaction and Living page heading is visible.
        /// </returns>
        public async Task<bool> LoadNoBrandactionAndLivingPage_VerifyHeading()
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
