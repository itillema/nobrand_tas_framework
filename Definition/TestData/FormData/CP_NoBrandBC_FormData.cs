namespace Definition.TestData.FormData
{
    /// <summary>
    ///     Test data for the NoBrandBC NoBrandBC community page "Want to know more about NoBrandBC?" lead form.
    /// </summary>
    public class CP_NoBrandBCCourt_FormData
    {
        public string FirstName { get; set; } = "NoBrand";
        public string LastName { get; set; } = "Auto-Test";

        // Per project sensitive data rules: use placeholder domain, never a real address
        public string Email { get; set; } = "autotest@nobrand.com";
        public string Phone { get; set; } = "1234567891";

        // "Pricing & Availability" avoids the conditional tour-date/time pickers that "Booking a Tour" triggers
        public string InterestedIn { get; set; } = "Pricing & Availability";

        // "How did you hear about us?" dropdown value
        public string ReferralSource { get; set; } = "Search Engine";
    }
}


