using Microsoft.VisualStudio.TestTools.UnitTesting;
using ToSic.Sxc.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace ToSic.Sxc.Tests.ServicesTests
{
    [TestClass]
    public class MailServiceTests
    {
        [TestMethod]
        public void MailAddressTests()
        {
            MailAddressAreEqual("user@domain", "user@domain", "");
            MailAddressAreEqual("<user@domain>", "user@domain", "");
            MailAddressAreEqual("first last user@domain", "user@domain", "first last");
            MailAddressAreEqual("   first last    user@domain   ", "user@domain", "first last");
            MailAddressAreEqual("first last  user@domain", "user@domain", "first last");
            MailAddressAreEqual("first last <user@domain>", "user@domain", "first last");
            MailAddressAreEqual("first last   <user@domain>  ", "user@domain", "first last");
            MailAddressAreEqual(@"""first last"" user@domain", "user@domain", "first last");
            MailAddressAreEqual(@"  ""first last""  user@domain", "user@domain", "first last");
            MailAddressAreEqual(@"""first last"" <user@domain>", "user@domain", "first last");
            // MailAddressAreEqual(@"""first""<> last"" <user@domain>", "user@domain", @"first""<> last");
            MailAddressAreEqual("first@last user@domain", "user@domain", "first@last");
            // MailAddressAreEqual("<first@last> <user@domain>", "user@domain", "<first@last>");
            MailAddressAreEqual("first last<user@domain>", "user@domain", "first last");
            MailAddressAreEqual(@"""first last""user@domain", "user@domain", "first last");
        }

        //[TestMethod]
        //[Ignore]
        //public void MailAddressTestsThatAreStrange()
        //{
        //    MailAddressAreEqual(@"first last user@domain>", "user@domain", @"first last");
        //    MailAddressAreEqual(@"first last <user@domain", "user@domain", @"first last");
        //    MailAddressAreEqual(@"""first last user@domain", "user@domain", @"first last");
        //    MailAddressAreEqual(@"first last"" user@domain", "user@domain", @"first last");
        //    MailAddressAreEqual(@" "" "" first last "" "" user@domain", "user@domain", @"first last");
        //    MailAddressAreEqual(@"first"" last user@domain", "user@domain", @"first"" last");
        //    MailAddressAreEqual(@"<first@last> user@domain", "user@domain", @"<first@last>");
        //}

        [TestMethod]
        public void MailAddressesCommon1()
        {
            const string addresses = @"""John, Doe"" <user@host>, ""Bob, Smith"" <user2@host>";
            var mailAddressCollection = new MailAddressCollection { addresses };

            AreEqual(mailAddressCollection[0].Address, "user@host");
            AreEqual(mailAddressCollection[0].DisplayName, "John, Doe");
            AreEqual(mailAddressCollection[1].Address, "user2@host");
            AreEqual(mailAddressCollection[1].DisplayName, "Bob, Smith");
        }

        [TestMethod]
        public void MailAddressesCommon2()
        {
            const string addresses = @"info@a.com, info@b.com";
            var mailAddressCollection = new MailAddressCollection { addresses };

            AreEqual(mailAddressCollection[0].Address, "info@a.com");
            AreEqual(mailAddressCollection[0].DisplayName, "");
            AreEqual(mailAddressCollection[1].Address, "info@b.com");
            AreEqual(mailAddressCollection[1].DisplayName, "");
        }

        private void MailAddressAreEqual(string input, string address, string displayName)
        {
            var mailAddress = new MailAddress(input);
            AreEqual(mailAddress.Address, address);
            AreEqual(mailAddress.DisplayName, displayName);
        }

        // TODO: STV - unit tests for
        // - MailAddressCollection
        // - Attachments
    }
}