using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Definition.TestData.FormData
{
    /// <summary>
    ///     Test data for The NoBrandCap NoBrandlux community page lead form (Form A).
    /// </summary>
    public class CP_TheNoBrandCap_FormData
    {
        public string FirstName { get; set; } = "NoBrand";
        public string LastName { get; set; } = "Auto-Test";

        // Per project sensitive data rules: use placeholder domain, never a real address
        public string Email { get; set; } = "autotest@nobrand.com";
        public string Phone { get; set; } = "1234567891";

        // "Pricing & Availability" avoids any date/time picker that "Booking a Tour" may trigger
        public string InterestedIn { get; set; } = "Pricing & Availability";

        // Preferred Method of Contact radio button value (options: Call, Text, Email)
        public string PreferredContact { get; set; } = "Email";
    }
}


