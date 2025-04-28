using static ToSic.Sxc.ServicesTests.Mail.MailServiceTestsHelper;

namespace ToSic.Sxc.ServicesTests.Mail;

public class MailAddressTests(MailServiceTestsHelper helper)
{
    // Actually tests System.Net.Mail.MailAddress
    // that is responsible for parsing input to email Address and display name
    [Theory]
    [InlineData(Address, Address, "")]
    [InlineData($"<{Address}>", Address, "")]
    [InlineData($"{DisplayName} {Address}", Address, DisplayName)]
    [InlineData($"{DisplayName}  {Address}", Address, DisplayName)]
    [InlineData($"   {DisplayName}    {Address}   ", Address, DisplayName)]
    [InlineData($"{DisplayName} <{Address}>", Address, DisplayName)]
    [InlineData($" {DisplayName}   <{Address}>  ", Address, DisplayName)]
    [InlineData($@"""{DisplayName}"" {Address}", Address, DisplayName)]
    [InlineData($@"  ""{DisplayName}""  {Address}    ", Address, DisplayName)]
    [InlineData($@"""{DisplayName}"" <{Address}>", Address, DisplayName)]
    [InlineData($"first@last {Address}", Address, "first@last")]
    [InlineData($"   <first@last>    <{Address}>   ", Address, "<first@last>")]
    [InlineData($"{DisplayName}<{Address}>", Address, DisplayName)]
    [InlineData($@"""{DisplayName}""{Address}", Address, DisplayName)]

    public void ParseInputString(string input, string expMail, string expName) =>
        helper.MailAddressEqual(input, expMail, expName);

    [Theory]
    // FormatException: An invalid character was found in the mail header: 't'.
    [InlineData($@"""first"" last"" <{Address}>", Address, @"first"" last")]

    // FormatException: The specified string is not in the form required for an e-mail address.
    [InlineData(@"{DisplayName} {Address}>", Address, DisplayName)]

    // FormatException: The specified string is not in the form required for an e-mail address.
    [InlineData(@"{DisplayName} <{Address}", Address, DisplayName)]

    // FormatException: The specified string is not in the form required for an e-mail address.
    [InlineData(@"""{DisplayName} {Address}", Address, DisplayName)]

    // FormatException: The specified string is not in the form required for an e-mail address.
    [InlineData(@"{DisplayName}"" {Address}", Address, DisplayName)]

    // FormatException: The specified string is not in the form required for an e-mail address.
    [InlineData(@" "" "" {DisplayName} "" "" {Address}", Address, DisplayName)]

    // FormatException: The specified string is not in the form required for an e-mail address.
    [InlineData(@"first"" last {Address}", Address, @"first"" last")]

    // FormatException: The specified string is not in the form required for an e-mail address.
    [InlineData(@"<first@last> {Address}", Address, @"<first@last>")]
    public void ParseInputStringThatFails(string input, string expMail, string expName) =>
        Throws<FormatException>(() => helper.MailAddressEqual(input, expMail, expName));
}