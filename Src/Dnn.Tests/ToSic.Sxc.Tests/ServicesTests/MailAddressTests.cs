using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ToSic.Sxc.Tests.ServicesTests
{
    [TestClass]
    public class MailAddressTests : MailServiceTestsBase
    {
        [TestMethod]
        public void ParseInputString()
        {
            // Actually tests System.Net.Mail.MailAddress
            // that is responsible for parsing input to email Address and display name

            MailAddressAreEqual(Address, Address, "");
            MailAddressAreEqual($"<{Address}>", Address, "");
            MailAddressAreEqual($"{DisplayName} {Address}", Address, DisplayName);
            MailAddressAreEqual($"{DisplayName}  {Address}", Address, DisplayName);
            MailAddressAreEqual($"   {DisplayName}    {Address}   ", Address, DisplayName);
            MailAddressAreEqual($"{DisplayName} <{Address}>", Address, DisplayName);
            MailAddressAreEqual($" {DisplayName}   <{Address}>  ", Address, DisplayName);
            MailAddressAreEqual($@"""{DisplayName}"" {Address}", Address, DisplayName);
            MailAddressAreEqual($@"  ""{DisplayName}""  {Address}    ", Address, DisplayName);
            MailAddressAreEqual($@"""{DisplayName}"" <{Address}>", Address, DisplayName);
            MailAddressAreEqual($"first@last {Address}", Address, "first@last");
            MailAddressAreEqual($"   <first@last>    <{Address}>   ", Address, "<first@last>");
            MailAddressAreEqual($"{DisplayName}<{Address}>", Address, DisplayName);
            MailAddressAreEqual($@"""{DisplayName}""{Address}", Address, DisplayName);
        }

        [TestMethod]
        [Ignore]
        public void ParseInputStringThatFails()
        {
            // FormatException: An invalid character was found in the mail header: 't'.
            MailAddressAreEqual($@"""first"" last"" <{Address}>", Address, @"first"" last");

            // FormatException: The specified string is not in the form required for an e-mail address.
            MailAddressAreEqual(@"{DisplayName} {Address}>", Address, DisplayName);

            // FormatException: The specified string is not in the form required for an e-mail address.
            MailAddressAreEqual(@"{DisplayName} <{Address}", Address, DisplayName);

            // FormatException: The specified string is not in the form required for an e-mail address.
            MailAddressAreEqual(@"""{DisplayName} {Address}", Address, DisplayName);

            // FormatException: The specified string is not in the form required for an e-mail address.
            MailAddressAreEqual(@"{DisplayName}"" {Address}", Address, DisplayName);

            // FormatException: The specified string is not in the form required for an e-mail address.
            MailAddressAreEqual(@" "" "" {DisplayName} "" "" {Address}", Address, DisplayName);

            // FormatException: The specified string is not in the form required for an e-mail address.
            MailAddressAreEqual(@"first"" last {Address}", Address, @"first"" last");

            // FormatException: The specified string is not in the form required for an e-mail address.
            MailAddressAreEqual(@"<first@last> {Address}", Address, @"<first@last>");
        }
    }
}