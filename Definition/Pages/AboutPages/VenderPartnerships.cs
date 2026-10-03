using Microsoft.Playwright;
using Serilog;
using static Microsoft.Playwright.Assertions;

namespace Definition.Pages.AboutPages
{
    public class VenderPartnerships(IPage page, ILogger logger)
    {

        //-- Page Elements --//
        private ILocator PageH1Heading_Text => page.Locator("xpath=//h1[@class='masthead-common-content__heading' and text()='Vendor Partnerships']").First;



        //-- Page Methods --//

        /// <summary>
        ///     Verify Home page loads, the cookies banner is visible, and that the user can accept cookies.
        /// </summary>
        /// <returns>
        ///     Boolean value indicating if the cookie banner is visible.
        /// </returns>
        public async Task<bool> LoadVenderPartnershipsPage_VerifyHeading()
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


