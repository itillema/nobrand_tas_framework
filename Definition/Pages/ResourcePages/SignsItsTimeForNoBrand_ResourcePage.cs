using Microsoft.Playwright;
using Serilog;
using static Microsoft.Playwright.Assertions;

namespace Definition.Pages.ResourcePages
{
    public class SignsItsTimeForNoBrand_ResourcePage(IPage page, ILogger logger)
    {
        //-- General Header and Navigation Menu Elements --//
        private ILocator PageH1Heading_Text => page.Locator("xpath=//h1[text()='When Is It Time for NoBrand type 1? Key Signs to Look For']").First;


        //-- Page Methods --//

        /// <summary>
        ///     Verifies this blog page loads.
        /// </summary>
        /// <returns>
        ///     Boolean value indicating if the page heading is visible.
        /// </returns>
        public async Task<bool> LoadSignsItsTimeForNoBrand_ResourcePage_VerifyTitle()
        {
            logger.Information("    Verifying title is displayed on page...");

            try
            {
                await Expect(PageH1Heading_Text.First).ToBeVisibleAsync();
                logger.Information("    Title is displayed on page.");
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "    Failed to verify title is displayed on the page.");
                return false;
            }


        }


    }
}

