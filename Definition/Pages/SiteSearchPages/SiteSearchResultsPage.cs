using Definition.Pages.ResourcePages;
using Microsoft.Playwright;
using Serilog;
using static Microsoft.Playwright.Assertions;


namespace Definition.Pages.SiteSearchPages
{
    public class SiteSearchResultsPage(IPage page, ILogger logger)
    {
        //-- Page Elements --//
        private ILocator PageH1Heading_Text => page.Locator("xpath=//h1[@class='masthead-common-content__heading' and text()='NoBrand Search']").First;
        private ILocator SiteSearchInput_TextField => page.Locator("xpath=//input[@class='field__input' and @value='NoBrand']").First;
        private ILocator SiteSearchInput_SearchResult_TextField => page.Locator("xpath=//input[@class='field__input' and @value='test']").First;
        private ILocator SiteSearchResults_List => page.Locator("xpath=//li//div[@class='listing-row-result']");
        private ILocator SiteSearchFilter_MedicalProfessionals_TextField => page.Locator("xpath=//label[@class='listing-row-facet__label' and @title='Medical Professionals']").First;
        private ILocator SiteSearchResult_Link => page.Locator("xpath=//h3[@class='listing-row-result__title ']//a[@href='https://uat.NoBrand.com']").First;
        private ILocator SiteSearchResult_Container => page.Locator("xpath=//div[@class='listing-row__facets-and-results-container']").First;


        //-- Page Methods --//


        /// <summary>
        ///     Verify that the Site Search Results page loads as expected.
        /// </summary>
        /// <returns>
        ///     Boolean value indicating if the Site Search Results page heading is visible.
        /// </returns>
        public async Task<bool> LoadSiteSearchResultsPage_VerifyHeading()
        {
            logger.Information("    Verifying heading is displayed on page...");

            try
            {
                await Expect(PageH1Heading_Text).ToBeVisibleAsync();
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
        ///     Verify that the Site Search Results page displays search results for search term.
        /// </summary>
        /// <returns>
        ///     Boolean value indicating if the Site Search Results page search term is applied.
        /// </returns>
        public async Task<bool> LoadSiteSearchResultsPage_VerifySearchTerm()
        {
            logger.Information("    Verifying search term is applied on page...");

            try
            {
                await Expect(SiteSearchInput_TextField).ToBeVisibleAsync();
                logger.Information("    Search term is applied on page.");
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "    Failed to verify search term is applied on the page.");
                return false;
            }


        }

        /// <summary>
        ///     Verify site search results.  This method requires navigation from a header site search.
        /// </summary>
        /// <returns>
        ///     Boolean value indicating if results are returned for a site search.
        /// </returns>
        public async Task<bool> LoadSiteSearchResultsPage_VerifySearchResults()
        {
            logger.Information("    Verifying search results are displayed on page...");

            try
            {
                await Expect(PageH1Heading_Text).ToBeVisibleAsync();
                logger.Information("    Header is displayed on page.");

                await Expect(SiteSearchInput_SearchResult_TextField).ToBeVisibleAsync();
                logger.Information("    Search term is displayed on page.");

            }
            catch (Exception ex)
            {
                logger.Error(ex, "    Failed to verify search results are displayed on the page.");
                return false;
            }

            try
            {
                await Expect(SiteSearchResult_Container).ToBeVisibleAsync();
                logger.Information("    Search results are displayed on page.");

                int resultCount = await SiteSearchResults_List.CountAsync();
                if (resultCount == 10)
                {
                    logger.Information($"    Check Passed: Found {resultCount} site search results.");
                    return true;
                }
                else
                {
                    // Page loaded but no results - this is still a valid page state for the test
                    logger.Information("    Page loaded successfully but no site search results found for the search criteria.");
                    return false;
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex, "    Failed to verify search results are displayed on the page.");
                return false;
            }




        }

        /// <summary>
        ///     Filter and sort site search results.  This method requires navigation from a header site search.
        /// </summary>
        /// <returns>
        ///     Boolean value indicating if results are filtered and sorted for a site search.
        /// </returns>
        public async Task<bool> LoadSiteSearchResultsPage_FilterAndSortSearchResults()
        {
            logger.Information("    Verifying search results are displayed on page...");

            try
            {
                await Expect(PageH1Heading_Text).ToBeVisibleAsync();
                logger.Information("    Header is displayed on page.");

                await Expect(SiteSearchInput_SearchResult_TextField).ToBeVisibleAsync();
                logger.Information("    Search term is displayed on page.");

            }
            catch (Exception ex)
            {
                logger.Error(ex, "    Failed to verify search results are displayed on the page.");
                return false;
            }

            // Capture the pre-filter result count so we can wait for the AJAX filter to apply
            int preFilterCount = await SiteSearchResults_List.CountAsync();
            logger.Information($"    Pre-filter result count: {preFilterCount}");

            try
            {
                await Expect(SiteSearchFilter_MedicalProfessionals_TextField).ToBeVisibleAsync();
                logger.Information("    Filter facet is displayed on page.");

                // Plain click — Playwright auto-scrolls the facet into view and auto-waits for it to be actionable, replacing the original scrollIntoView + ElementToBeClickable + JS click dance.
                await SiteSearchFilter_MedicalProfessionals_TextField.ClickAsync();
                logger.Information("    Filter facet is clicked.");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "    Failed to verify filter facets are displayed on the page.");
                return false;
            }

            try
            {
                // Wait for the filter AJAX to complete: the result count must change from the pre-filter value.
                // (No loading spinner / staleness signal exists in this UI to wait on instead.)
                await Expect(SiteSearchResults_List).Not.ToHaveCountAsync(preFilterCount);

                int postFilterCount = await SiteSearchResults_List.CountAsync();
                logger.Information($"    Post-filter result count: {postFilterCount}");

                // Resilient check: filter must produce a non-zero result set that is smaller than the unfiltered set.
                // Avoids brittleness against UAT data changes while still proving the facet actually filtered.
                if (postFilterCount > 0 && postFilterCount < preFilterCount)
                {
                    logger.Information($"    Check Passed: Filter reduced results from {preFilterCount} to {postFilterCount}.");
                    return true;
                }
                else
                {
                    logger.Error($"    Filter did not produce a valid reduced result set. Pre-filter: {preFilterCount}, Post-filter: {postFilterCount}.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "    Filter did not change result count from pre-filter value of " + preFilterCount + " within timeout, or failed to verify filtered search results.");
                return false;
            }




        }

        /// <summary>
        ///     Click a search result and navigate to the blog page.  This method requires navigation from a header site search.
        /// </summary>
        /// <returns>
        ///     New page object instance of HowToOrganizeANoBrandusersMedicalInformation_ResourcePage.
        /// </returns>
        public async Task<HowToOrganizeANoBrandusersMedicalInformation_ResourcePage> LoadSiteSearchResultsPage_ClickSearchResultAndNavigateToBlogPage()
        {
            logger.Information("    Verifying search results are displayed on page...");

            try
            {
                await Expect(PageH1Heading_Text).ToBeVisibleAsync();
                logger.Information("    Header is displayed on page.");

                await Expect(SiteSearchInput_SearchResult_TextField).ToBeVisibleAsync();
                logger.Information("    Search term is displayed on page.");

            }
            catch (Exception ex)
            {
                logger.Error(ex, "    Failed to verify search results are displayed on the page.");
                throw;
            }

            try
            {
                await Expect(SiteSearchResult_Link).ToBeVisibleAsync();
                logger.Information("    Search result link is displayed on page.");

                // Plain click — Playwright auto-scrolls the link into view and auto-waits for it to be actionable, replacing the original scrollIntoView + ElementToBeClickable dance.
                await SiteSearchResult_Link.ClickAsync();
                logger.Information("    Search result link is clicked.");

                return new HowToOrganizeANoBrandusersMedicalInformation_ResourcePage(page, logger);

            }
            catch (Exception ex)
            {
                logger.Error(ex, "    Failed to find or click search result link.");
                throw;
            }

        }
    }
}

