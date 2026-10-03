namespace Definition.TestData.FormData
{
    /// <summary>
    ///     Test data for the NoBrandAp NoBrandlux community page inline "GET IN TOUCH WITH THE NoBrandAp" lead form.
    /// </summary>
    public class CP_TheNoBrandAp_FormData
    {
        public string FirstName { get; set; } = "NoBrand";
        public string LastName { get; set; } = "Auto-Test";

        // Per project sensitive data rules: use placeholder domain, never a real address
        public string Email { get; set; } = "autotest@nobrand.com";
        public string Phone { get; set; } = "1234567891";

        // Upper West Side NYC ZIP — matches the community's location
        public string Zip { get; set; } = "10024";

        // "Request More Information" avoids the conditional Preferred Date/Time pickers that "Schedule a Tour" triggers
        public string InterestedIn { get; set; } = "Request More Information";
    }
}
