namespace Definition.TestData.FormData
{
    /// <summary>
    ///     Test data for the "Get in Touch to Learn More" lead form on the Home page. Uses "Book a Tour" as the inquiry type, which exposes the Preferred Date and Preferred Tour Time fields and hides the Comments textarea.
    /// </summary>
    public class Home_LeadFormA_FormData
    {
        public string FirstName { get; set; } = "NoBrand";
        public string LastName { get; set; } = "Auto-Test";
        public string Email { get; set; } = "autotest@nobrand.com";
        public string Phone { get; set; } = "1234567891";
        public string Zip { get; set; } = "60614";

        public string InquiryType { get; set; } = "Booking a Tour";

        // Relative date so the value doesn't age into a "select a future date" validation error.
        public string PreferredTourDate { get; set; } = DateTime.Today.AddDays(7).ToString("MM/dd/yyyy");

        // Option text from the live <select name="preferredTime"> on LP lead forms.
        public string PreferredTourTime { get; set; } = "10:00 am - 11:00 am";

        public string ReferralSource { get; set; } = "Search Engine";

        // Comments textarea is hidden when InquiryType is "Book a Tour" on sibling LP forms; kept here in case the Home variant still renders it, in which case FillLeadForm will fill it.
        public string Comments { get; set; } = "This is an automated test submission.";
    }
}

