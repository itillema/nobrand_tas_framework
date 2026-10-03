using Definition.Pages.CommunityPages.US_CommunityPages;
using Definition.TestData.FormData;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace Execution.RegressionTests.LeadFormsTests
{
    [TestFixture]
    [Category("Regression")]
    [Category("LeadForm")]
    public class NoBrandEFS_NoBrandlux_LeadFormB_RegressionTest : UtilityBase
    {
        [SetUp]
        public void Setup()
        {
            // Configure test environment for UAT, US locale
            GetEnvironmentConfig("uat", "us");
        }

        [Test, Description("Verify the NoBrandEFS Inquire NoBrandlux community page lead form submits successfully and a Thank You confirmation replaces the form in-page.")]
        public async Task NoBrandEFSNoBrandluxLeadFormB_Submit_ReplacedWithThankYou()
        {
            // Create form data and page object (DTO is shared between Form A and Form B)
            var formData = new CP_NoBrandEFS_FormData();
            CommunityPage_NoBrandEFS NoBrandEFSPage = new(await GetPageAsync(), testLogger);

            // Navigate to Inquire sub-page
            await NoBrandEFSPage.NavigateToNoBrandEFSInquirePage();
            testLogger.Information("STEP: Navigated to the NoBrand at NoBrandEFSth Inquire page.");

            // Accept cookie consent banner
            await NoBrandEFSPage.AcceptCookies();
            testLogger.Information("STEP: Accepted cookies.");

            // Scroll to the Inquire lead form ('Take a Tour. Reserve a Residence.' heading)
            await NoBrandEFSPage.ScrollToInquireLeadForm();
            testLogger.Information("STEP: Scrolled to the Inquire lead form.");

            // Fill all lead form fields and check both checkbNoBrandes (more-information + marketing consent)
            await NoBrandEFSPage.FillInquireLeadForm(formData);
            testLogger.Information("STEP: Filled Inquire lead form with test data.");

            // Submit form (60s wait for in-page confirmation)
            await NoBrandEFSPage.SubmitInquireLeadForm();
            testLogger.Information("STEP: Submitted Inquire lead form.");

            // Verify in-page thank-you confirmation is visible
            Assert.That(await NoBrandEFSPage.VerifyInquireThankYouConfirmation(), Is.True,
                "The in-page 'Thank you!' confirmation did not appear after Inquire form submission.");
            testLogger.Information("PASSED: In-page Inquire thank-you confirmation verified successfully.");
        }

        [TearDown]
        public async Task TearDown()
        {
            await TestCleanupAsync();
        }

    }
}

