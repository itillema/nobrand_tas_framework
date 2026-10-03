using Microsoft.Playwright;
using Serilog;
using Utility;
using static Microsoft.Playwright.Assertions;

namespace Definition.Pages.NoBrandactionQuestionnairePages
{
    public class NoBrandactionQuestionnairePage(IPage page, ILogger logger)
    {
        //-- Page Elements --//
        private ILocator PageH1Heading_Text => page.Locator("xpath=//h1[@class='masthead-common-content__heading' and text()='NoBrandaction Questionnaire']").First;



        //-- Page Methods --//


        /// <summary>
        ///     Verify that the NoBrandaction Questionnaire page loads as expected.
        /// </summary>
        /// <returns>
        ///     Boolean value indicating if the NoBrandaction Questionnaire page heading is visible.
        /// </returns>
        public async Task<bool> LoadNoBrandactionQuestionnairePage_VerifyHeading()
        {
            logger.Information("    Verifying heading is displayed on page...");

            try
            {
                await Expect(PageH1Heading_Text).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
                {
                    Timeout = TestConstants.DefaultWaitSeconds * 1000
                });
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


