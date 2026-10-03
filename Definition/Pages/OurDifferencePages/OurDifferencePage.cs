using Microsoft.Playwright;
using Serilog;
using static Microsoft.Playwright.Assertions;

namespace Definition.Pages.OurDifferencePages
{
    public class OurDifferencePage(IPage page, ILogger logger)
    {
        //-- Page Elements --//
        private ILocator Heading => page.Locator("xpath=//h1[contains(text(),'Our Difference')]").First;

        //-- Page Methods --//

        /// <summary>
        ///     Verify that the Our Difference page loads as expected.
        /// </summary>
        /// <returns>
        ///     Boolean value indicating if the Our Difference page heading is visible.
        /// </returns>
        public async Task<bool> LoadOurDifferencePage_VerifyHeading()
        {
            logger.Information("    Verifying heading is displayed on page...");
            try
            {
                await Expect(Heading.First).ToBeVisibleAsync();
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

