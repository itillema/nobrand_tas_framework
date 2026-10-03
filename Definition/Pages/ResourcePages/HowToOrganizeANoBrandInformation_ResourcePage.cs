using Microsoft.Playwright;
using Serilog;
using static Microsoft.Playwright.Assertions;

namespace Definition.Pages.ResourcePages
{
    public class HowToOrganizeANoBrandusersMedicalInformation_ResourcePage(IPage page, ILogger logger)
    {
        //-- General Header and Navigation Menu Elements --//
        private ILocator PageH1Heading_Text => page.Locator("xpath=/html/body/div[2]/main/section[1]/div[1]/div/h1").First;


        //-- Page Methods --//

        /// <summary>
        ///     Verifies this blog page loads.
        /// </summary>
        /// <returns>
        ///     Boolean value indicating if the page heading is visible.
        /// </returns>
        public async Task<bool> LoadHowToOrganizeANoBrandusersMedicalInformation_ResourcePage_VerifyTitle()
        {
            logger.Information("    Verifying title is displayed on page...");

            try
            {
                await Expect(PageH1Heading_Text).ToBeVisibleAsync();
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

