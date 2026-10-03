using Microsoft.Playwright;
using Serilog;

namespace Utility
{
    /// <summary>
    ///     Suite-wide navigation policy. NoBrand pages embed third-party content (consent manager, invisible reCAPTCHA, social embeds, hero media) whose requests can intermittently hang and keep the window load event from ever firing. Navigations therefore wait for DOMContentLoaded only; element readiness is enforced by the auto-waiting action or Expect() assertion that follows every navigation in this suite.
    /// </summary>
    public static class PageNavigation
    {
        /// <summary>
        ///     Navigate and wait for DOMContentLoaded (the suite's navigation-complete signal).
        ///     Always follow with an auto-waiting action or web-first assertion.
        /// </summary>
        public static async Task<IResponse?> GotoDomReadyAsync(this IPage page, string url)
        {
            var options = new PageGotoOptions { WaitUntil = WaitUntilState.DOMContentLoaded };
            try
            {
                return await page.GotoAsync(url, options);
            }
            catch (TimeoutException)
            {
                // A parser-blocking third-party script in <head> (sync reCAPTCHA api.js) can stall a single navigation past the timeout; a fresh request clears it. One retry only; a second consecutive stall means the environment is genuinely down and should fail.
                Log.Warning("Navigation to {Url} timed out waiting for DOMContentLoaded; retrying once.", url);
                return await page.GotoAsync(url, options);
            }
        }

        /// <summary>
        ///     Wait until the frame URL satisfies <paramref name="predicate"/>, resolving on DOMContentLoaded (the suite's navigation-complete signal) rather than the window load event, which third-party subresources can keep from ever firing. Mirrors GotoDomReadyAsync for URL waits. Callable on any IPage (main page or popup). Always follow with an auto-waiting action or Expect() assertion. <paramref name="timeoutMs"/> is milliseconds (null = Playwright's default navigation timeout).
        /// </summary>
        public static Task WaitForUrlDomReadyAsync(
            this IPage page, Func<string, bool> predicate, float? timeoutMs = null)
            => page.WaitForURLAsync(predicate, BuildUrlWaitOptions(timeoutMs));

        /// <summary>Glob / URL-string overload of <see cref="WaitForUrlDomReadyAsync(IPage, Func{string, bool}, float?)"/>.</summary>
        public static Task WaitForUrlDomReadyAsync(
            this IPage page, string urlOrGlob, float? timeoutMs = null)
            => page.WaitForURLAsync(urlOrGlob, BuildUrlWaitOptions(timeoutMs));

        private static PageWaitForURLOptions BuildUrlWaitOptions(float? timeoutMs) => new()
        {
            WaitUntil = WaitUntilState.DOMContentLoaded,
            Timeout = timeoutMs
        };
    }
}


