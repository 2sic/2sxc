using System.Net.Mail;
using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Internal;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.ServicesTests
{
    public class MailServiceTestsBase : TestBaseSxc
    {
        #region Helpers

        public IMailService MailService() => GetService<IMailService>();

        public const string Address = @"user@host";
        public const string DisplayName = @"Display Name";
        public const string Addresses = @"""John, Doe"" <user@host>, ""Bob, Smith"" <user2@host>";
        public const string Emails = @"a@a.com,b@b.com,c@c.com,d@d.com";

        public void MailAddressAreEqual(string input, string expectedAddress, string expectedDisplayName)
        {
            // Actually tests System.Net.Mail.MailAddress
            // that is responsible for parsing input to email Address and display name
            var actual = ((MailServiceBase)MailService()).MailAddress("test", input);
            AreEqual(expectedAddress, actual.Address);
            AreEqual(expectedDisplayName, actual.DisplayName);
        }

        public void MailAddressAreEqual(string input,
            string expected1Address, string expected1DisplayName,
            string expected2Address, string expected2DisplayName
        )
        {
            // Actually tests System.Net.Mail.MailAddressCollection
            // that is responsible for parsing string input to list of MailAddresses
            var actual = new MailAddressCollection();

            ((MailServiceBase)MailService()).AddMailAddresses("test", actual, input);
            AreEqual(expected1Address, actual[0].Address);
            AreEqual(expected1DisplayName, actual[0].DisplayName);
            AreEqual(expected2Address, actual[1].Address);
            AreEqual(expected2DisplayName, actual[1].DisplayName);
        }

        #endregion
    }
}
