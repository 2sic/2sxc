using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ToSic.Sxc.Tests.ServicesTests
{
    [TestClass]
    public class MailAddressCollectionTests : MailServiceTestsBase
    {
        [TestMethod]
        public void ParseInputString()
        {
            // Actually tests System.Net.Mail.MailAddressCollection
            // that is responsible for parsing string input to list of MailAddresses

            MailAddressAreEqual(@"a@a.com,b@b.com,c@c.com",
                "a@a.com", "",
                "b@b.com", "");

            MailAddressAreEqual(@"a@a.com, b@b.com, c@c.com",
                "a@a.com", "",
                "b@b.com", "");

            MailAddressAreEqual(@"  a@a.com  ,  b@b.com  ,  c@c.com  ",
                "a@a.com", "",
                "b@b.com", "");

            // use ; as address separator
            MailAddressAreEqual(@"a@a.com;b@b.com;c@c.com",
                "a@a.com", "",
                "b@b.com", "");

            MailAddressAreEqual(@"<a@a.com>,<b@b.com>,<c@c.com>",
                "a@a.com", "",
                "b@b.com", "");

            // sometimes use ; as address separator
            MailAddressAreEqual(@"<a@a.com>,b@b.com;<c@c.com>",
                "a@a.com", "",
                "b@b.com", "");

            MailAddressAreEqual(@"John Doe user@host, Bob Smith user2@host",
                "user@host", "John Doe",
                "user2@host", "Bob Smith");

            // have , as part of displayName when displayName is quoted
            MailAddressAreEqual(@"""John, Doe"" <user@host>, ""Bob, Smith"" <user2@host>",
                "user@host", "John, Doe",
                "user2@host", "Bob, Smith");

            MailAddressAreEqual(@"""John, Doe"" <user@host>, Bob Smith user2@host",
                "user@host", "John, Doe",
                "user2@host", "Bob Smith");

            // use ; as address separator
            MailAddressAreEqual(@"""John, Doe"" <user@host>; ""Bob, Smith"" <user2@host>",
                "user@host", "John, Doe",
                "user2@host", "Bob, Smith");
        }

        [TestMethod]
        [Ignore]
        public void ParseInputStringThatFails()
        {
            // FormatException: An invalid character was found in the mail header: ','.
            MailAddressAreEqual(@"a@a.com,,c@c.com",
                "a@a.com", "",
                "c@c.com", "");

            // failed. Expected:<a@a.com>. Actual:<c@c.com>.
            MailAddressAreEqual(@"a@a.com b@b.com c@c.com",
                "a@a.com", "",
                "b@b.com", "");

            // !!! have ; as part of displayName
            // failed. Expected:<John; Doe>. Actual:<John, Doe>.
            MailAddressAreEqual(@"""John; Doe"" <user@host>, ""Bob; Smith"" <user2@host>",
                "user@host", "John; Doe",
                "user2@host", "Bob; Smith");

            // have , as part of displayName when displayName is not quoted
            // FormatException: The specified string is not in the form required for an e-mail address.
            MailAddressAreEqual(@"John, Doe user@host, Bob, Smith user2@host",
                "user@host", "John, Doe",
                "user2@host", "Bob, Smith");
        }
    }
}