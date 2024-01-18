using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Mail;
using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Internal;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.ServicesTests
{
    [TestClass]
    public class MailAddressInputTypeTests : MailServiceTestsBase
    {
        [TestMethod]
        public void MailAddressType()
        {
            var expected = new MailAddress(Address, DisplayName);
            var actual = ((MailServiceBase)MailService()).MailAddress("test", expected);
            AreSame(expected, actual);
        }

        [TestMethod]
        public void StringType()
        {
            AreEqual(Address, ((MailServiceBase)MailService()).MailAddress("test", Address).Address);
        }

        [TestMethod]
        [ExpectedException(typeof(FormatException))]
        public void InvalidString()
        {
            // An invalid character was found in the mail header.
            ((MailServiceBase)MailService()).MailAddress("test", @";;;ffff@@@@@@gggggg");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void EmptyString()
        {
            // The parameter 'address' cannot be an empty string.
            ((MailServiceBase)MailService()).MailAddress("test", string.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Null()
        {
            // Unknown type for MailAddress
            ((MailServiceBase)MailService()).MailAddress("test", null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UnknownType()
        {
            var mailAddress = new { a = "test" }; // unknown type
            // Trying to parse e-mails for test but got unknown type for mailAddress
            ((MailServiceBase)MailService()).MailAddress("test", mailAddress);
        }
    }
}