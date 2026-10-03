
namespace Definition.TestData.FormData
{
    /// <summary>
    ///     Test data for the site-header 'Book a Tour' dropdown lead form. Not community-specific — the dropdown is a global header component shared across community pages.
    /// </summary>
    public class BookATour_FormData
    {
        public string FirstName { get; set; } = "NoBrand";
        public string LastName { get; set; } = "Auto-Test";

        // Per project sensitive data rules: use placeholder domain, never a real address
        public string Email { get; set; } = "autotest@nobrand.com";
        public string Phone { get; set; } = "1234567891";

        // Relative date so the test doesn't age into a "select a future date" validation error
        public string PreferredTourDate { get; set; } = DateTime.Today.AddDays(7).ToString("MM/dd/yyyy");

        // Option text from the live <select name="preferredTime"> on the community page's Book a Tour dropdown.
        public string PreferredTourTime { get; set; } = "10:00 am - 11:00 am";

        // "How did you hear about us?" dropdown value
        public string HearAboutUs { get; set; } = "Search Engine";
    }
}

