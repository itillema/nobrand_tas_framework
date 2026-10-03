using Microsoft.Playwright;
using Serilog;
using static Microsoft.Playwright.Assertions;

namespace Definition.Pages.ExperienceNoBrandPages
{
    public class ExperienceNoBrandPage(IPage page, ILogger logger)
    {
        //-- Page Elements --//
        private ILocator PageH1Heading_Text => page.Locator("xpath=//h1[@class='masthead-common-content__heading']").First;

        /// <summary>
        ///     Verify that the Experience NoBrand page loads as expected.
        /// </summary>
        /// <returns>
        ///     Boolean value indicating if the Experience NoBrand page heading is visible.
        /// </returns>
        public async Task<bool> LoadExperienceNoBrandPage_VerifyHeading()
        {
            logger.Information("    Verifying heading is displayed on page...");

            try
            {
                await Expect(PageH1Heading_Text).ToBeVisibleAsync();
                logger.Information("    Webdriver successfully scrolled to and verified the page heading is displayed.");
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


