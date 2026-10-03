using Microsoft.Playwright;
using Serilog;
using static Microsoft.Playwright.Assertions;

namespace Definition.Pages.LandingPages
{
    public class ThankYouPage(IPage page, ILogger logger)
    {
        //-- Page Elements --//
        private ILocator PageH1Heading =>
            page.GetByRole(AriaRole.Heading, new PageGetByRoleOptions { Name = "Thank you for your interest in NoBrand." })
                .Filter(new LocatorFilterOptions { Visible = true });

        //-- Page Methods --//

        /// <summary>
        ///     Verify the user can navigate to the Thank You page and that it loads as expected.
        /// </summary>
        /// <returns>True if the Thank You page heading is visible.</returns>
        public async Task<bool> LoadThankYouPage_VerifyHeading()
        {
            logger.Information("    Verifying heading is displayed on page...");
            try
            {
                await Expect(PageH1Heading.First).ToBeVisibleAsync();
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


