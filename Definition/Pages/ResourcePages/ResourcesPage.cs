using Microsoft.Playwright;
using Serilog;
using Utility;
using static Microsoft.Playwright.Assertions;


namespace Definition.Pages.ResourcePages
{
    public class ResourcesPage(IPage page, ILogger logger)
    {

        private readonly string SearchTermInput_String = "test";
        private readonly string UrlSnippet_SearchResults = "resources/search-results?q=test";

        //-- Header and Navigation Menu Elements --//
        private ILocator SearchField_Input => page.Locator("xpath=//input[@class='resource-hub-header__search-input']").First;
        private ILocator SearchButton_Button => page.Locator("xpath=//input[@class='button' and @value='Search']").First;
        private ILocator PageH1Heading_Text => page.Locator("xpath=//h1[normalize-space(.)='NoBrand Blog']").First;



        // -- Page Methods --//

        /// <summary>
        ///     Verify that the Resources / NoBrand Blog page loads as expected.
        /// </summary>
        /// <returns>
        ///     Boolean value indicating if the Resources page heading is visible.
        /// </returns>
        public async Task<bool> LoadResourcesPage_VerifyHeading()
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

        /// <summary>
        ///     Verify users can search for resources via the search field on the Resources page.
        /// </summary>
        /// <returns>
        ///     A new ResourcesPage_SearchResults object.
        /// </returns>
        public async Task<ResourcesPage_SearchResults> SearchForResource_NavigateToSearchResultsPage()
        {
            logger.Information("Verifying user can search for resources via the search field on the Resources page...");

            try
            {
                // Scroll to the search form section
                await Expect(SearchField_Input).ToBeVisibleAsync(new LocatorAssertionsToBeVisibleOptions
                {
                    Timeout = TestConstants.DefaultWaitSeconds * 1000
                });
                logger.Information("    Search field is visible.");

                await SearchField_Input.FillAsync(SearchTermInput_String);
                logger.Information("    Search term input into field.");

                await SearchButton_Button.ClickAsync();
                logger.Information("    Search button clicked.");

                // Wait for search results page to load
                await page.WaitForUrlDomReadyAsync($"**{UrlSnippet_SearchResults}**", TestConstants.DefaultWaitSeconds * 1000);
                logger.Information("    Successfully navigated to Resource Search Results page.");

                return new ResourcesPage_SearchResults(page, logger);
            }
            catch (Exception ex)
            {
                logger.Warning(ex, "    Cannot input search term or click search button.");
                throw;
            }
        }



    }
}

