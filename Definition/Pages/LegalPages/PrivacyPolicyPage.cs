using Microsoft.Playwright;
using Serilog;
using static Microsoft.Playwright.Assertions;

namespace Definition.Pages.LegalPages
{
    public class PrivacyPolicyPage(IPage page, ILogger logger)
    {
        //-- Page Elements --//
        private ILocator PageH2Heading_Text => page.Locator("xpath=/html/body/main/section[1]/h2").First;



        //-- Page Methods --//


        /// <summary>
        ///     Verify Privacy Policy page loads.
        /// </summary>
        /// <returns>
        ///     Boolean value indicating if the sub-header on the Privacy Policy page is visible.
        /// </returns>
        public async Task<bool> LoadPrivacyPolicyPage_VerifyHeading()
        {
            logger.Information("    Verifying sub-heading is displayed on page...");

            try
            {
                await Expect(PageH2Heading_Text.First).ToBeVisibleAsync();
                logger.Information("    Sub-heading is displayed on page.");
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "    Failed to verify headin is displayed on the page.");
                return false;
            }


        }


    }
}

