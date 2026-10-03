
namespace Definition.TestData.FormData
{
    /// <summary>
    ///     Shared test data for Lead Form A across landing pages (e.g., NoBrand Every Day, Family).
    /// </summary>
    public class LP_LeadFormA_FormData
    {
        public string FirstName { get; set; } = "NoBrand";
        public string LastName { get; set; } = "Auto-Test";
        public string Email { get; set; } = "autotest@nobrand.com";
        public string Phone { get; set; } = "1234567891";
        public string Zip { get; set; } = "60614";

        // Use "Pricing & Availability" instead of "Book a Tour" to avoid date/time picker fields
        public string InquiryType { get; set; } = "Pricing & Availability";

        // How did you hear about us
        public string ReferralSource { get; set; } = "Search Engine";

        // Comments field - appears when InquiryType is NOT "Book a Tour"
        public string Comments { get; set; } = "This is an automated test submission.";
    }
}

