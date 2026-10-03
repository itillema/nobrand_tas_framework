using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions;
using Serilog;
using System.Text.RegularExpressions;

namespace Utility
{
    /// <summary>
    ///     The lead forms' ZIP field fires a geolocation + Coveo "nearby" lookup on blur; the form's required-nearby validator silently blocks the POST until the hidden NearbyCommunities input is populated. The lookup's two external dependencies (the site's /api/geolocation and the Coveo community index) blip intermittently in UAT, so re-trigger the blur on failure before giving up.
    /// </summary>
    public static class ZipLookup
    {
        /// <summary>
        ///     Wait for the hidden NearbyCommunities input to be populated by the ZIP lookup, re-triggering the lookup (focus + blur) on failure. Throws with a diagnosis when the lookup never lands — in that state the form cannot submit for real users either.
        /// </summary>
        public static async Task EnsureNearbyCommunitiesPopulatedAsync(ILocator zipInput, ILocator nearbyCommunitiesInput, ILogger logger)
        {
            const int MaxAttempts = 3;
            for (int attempt = 1; attempt <= MaxAttempts; attempt++)
            {
                try
                {
                    await Expect(nearbyCommunitiesInput).ToHaveValueAsync(new Regex(".+"), new LocatorAssertionsToHaveValueOptions
                    {
                        Timeout = TestConstants.ShortWaitSeconds * 1000
                    });
                    logger.Information("    ZIP lookup succeeded: NearbyCommunities populated (attempt {Attempt}).", attempt);
                    return;
                }
                catch (Exception) when (attempt < MaxAttempts)
                {
                    logger.Warning("    ZIP nearby-communities lookup did not populate (attempt {Attempt}); re-triggering blur...", attempt);
                    await zipInput.FocusAsync();
                    await zipInput.BlurAsync();
                }
            }
            throw new Exception(
                "ZIP nearby-communities lookup failed: the hidden NearbyCommunities input never populated, so the form's required-nearby validation will block the POST (no submit will occur). The site's geolocation API (/api/geolocation) or Coveo community query is returning nothing for the test ZIP — a site/UAT environment defect, not a test issue.");
        }
    }
}


