using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Net.Mail;
using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Internal;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.ServicesTests
{
    [TestClass]
    public class AddMailAddressesInputTypeTests : MailServiceTestsBase
    {
        [TestMethod]
        public void MailAddressCollectionType()
        {
            var expected = new MailAddressCollection { Addresses };
            var actual = new MailAddressCollection();
            ((MailServiceBase)MailService()).AddMailAddresses("test", actual, expected);
            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestMethod]
        public void MailAddressArrayType()
        {
            var mailAddressCollection = new MailAddressCollection { Addresses };
            var expected = mailAddressCollection.ToArray();
            var actual = new MailAddressCollection();
            ((MailServiceBase)MailService()).AddMailAddresses("test", actual, expected);
            CollectionAssert.AreEquivalent(mailAddressCollection, actual);
        }

        [TestMethod]
        public void StringArrayType()
        {
            var expected = Emails.Split(',');
            var actual = new MailAddressCollection();
            ((MailServiceBase)MailService()).AddMailAddresses("test", actual, expected);
            AreEqual(expected.Length, actual.Count);
        }

        [TestMethod]
        public void StringArrayTypeWithTwoEmptyItems()
        {
            const string addresses = @"a@a.com,,,c@c.com,d@d.com";
            var expected = addresses.Split(',');
            var actual = new MailAddressCollection();
            ((MailServiceBase)MailService()).AddMailAddresses("test", actual, expected);
            AreEqual(expected.Length, actual.Count+2);
        }

        [TestMethod]
        public void StringType()
        {
            const int expected = 4; // number of emails in Emails string
            var actual = new MailAddressCollection();
            ((MailServiceBase)MailService()).AddMailAddresses("test", actual, Emails);
            AreEqual(expected, actual.Count);
        }

        [TestMethod]
        public void StringTypeWithNonStandardSeparator()
        {
            const string emailsWithNonStandardSeparator = @"a@a.com;b@b.com;c@c.com;d@d.com";
            const int expected = 4; // number of emails
            var actual = new MailAddressCollection();
            ((MailServiceBase)MailService()).AddMailAddresses("test", actual, emailsWithNonStandardSeparator);
            AreEqual(expected, actual.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void InvalidString()
        {
            // An invalid character was found in the mail header.
            ((MailServiceBase)MailService()).MailAddress("test", @";;;ffff@@@@@@gggggg");
        }

        [TestMethod]
        public void EmptyString()
        {
            // The parameter 'mailAddresses' can be empty string.
            const int expected = 0; // number of emails
            var actual = new MailAddressCollection();
            ((MailServiceBase)MailService()).AddMailAddresses("test", actual, string.Empty);
            AreEqual(expected, actual.Count);
        }

        [TestMethod]
        public void Null()
        {
            // The parameter 'mailAddresses' can be null.
            const int expected = 0; // number of emails
            var actual = new MailAddressCollection();
            ((MailServiceBase)MailService()).AddMailAddresses("test", actual, null);
            AreEqual(expected, actual.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UnknownType()
        {
            var mailAddresses = new { a = "test" }; // unknown type
            // Trying to parse e-mails for test but got unknown type for mailAddresses.
            ((MailServiceBase)MailService()).AddMailAddresses("test", new(), mailAddresses);
        }
    }
}