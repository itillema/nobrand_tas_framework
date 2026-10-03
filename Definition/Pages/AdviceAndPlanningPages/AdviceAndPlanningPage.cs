using Microsoft.Playwright;
using Serilog;
using static Microsoft.Playwright.Assertions;

namespace Definition.Pages.AdviceAndPlanningPages
{
    public class AdviceAndPlanningPage(IPage page, ILogger logger)
    {

        //-- Page Elements --//
        private ILocator PageH1Heading_Text => page.Locator("xpath=/html/body/main/div/div/div/div/h1").First;


        // -- Page Methods --//


        /// <summary>
        ///     Verify that the Advice and Planning page loads as expected.
        /// </summary>
        /// <returns>
        ///     Boolean value indicating if the Advice and Planning page heading is visible.
        /// </returns>
        public async Task<bool> LoadAdviceAndPlanningPage_VerifyHeading()
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
