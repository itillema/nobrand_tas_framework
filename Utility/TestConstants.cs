namespace Utility
{
    /// <summary>
    ///     Centralized constants for test configuration. Reference these instead of hardcoding magic numbers (e.g., timeouts) throughout the codebase.
    /// </summary>
    public static class TestConstants
    {
        /// <summary>Standard wait timeout for most element interactions (30 seconds).</summary>
        public const int DefaultWaitSeconds = 30;

        /// <summary>Extended wait for slow AJAX/network operations such as form submissions (60 seconds).</summary>
        public const int SubmitWaitSeconds = 60;

        /// <summary>Short wait for quick visibility checks or new tab/window detection (10 seconds).</summary>
        public const int ShortWaitSeconds = 10;
    }
}
