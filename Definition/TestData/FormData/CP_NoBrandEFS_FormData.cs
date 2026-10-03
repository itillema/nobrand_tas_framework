namespace Definition.TestData.FormData
{
    /// <summary>
    ///     Test data for the NoBrandEFSth NoBrandlux community page inline "Get In Touch with NoBrand at NoBrandEFSth" lead form.
    /// </summary>
    public class CP_NoBrandEFS_FormData
    {
        public string FirstName { get; set; } = "NoBrand";
        public string LastName { get; set; } = "Auto-Test";

        // Per project sensitive data rules: use placeholder domain, never a real address
        public string Email { get; set; } = "autotest@nobrand.com";
        public string Phone { get; set; } = "1234567891";
        public string Zip { get; set; } = "10022";

        // "Request More Information" avoids the conditional Preferred Date/Time pickers that "Schedule a Tour" triggers
        public string InterestedIn { get; set; } = "Request More Information";
    }
}

