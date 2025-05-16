using ToSic.Sxc.Services;

namespace ToSic.Sxc.ServicesTests.Mail;


public class MailAddressCollectionTests(IMailService mailSvc, MailServiceTestsHelper helper)
{
    [Theory]

    [InlineData(@"a@a.com,b@b.com,c@c.com", "a@a.com", "", "b@b.com", "")]
    [InlineData(@"a@a.com, b@b.com, c@c.com", "a@a.com", "", "b@b.com", "")]
    [InlineData(@"  a@a.com  ,  b@b.com  ,  c@c.com  ", "a@a.com", "", "b@b.com", "")]

    // use ; as address separator
    [InlineData(@"a@a.com;b@b.com;c@c.com", "a@a.com", "", "b@b.com", "")]
    [InlineData(@"<a@a.com>,<b@b.com>,<c@c.com>", "a@a.com", "", "b@b.com", "")]

    // sometimes use ; as address separator
    [InlineData(@"<a@a.com>,b@b.com;<c@c.com>", "a@a.com", "", "b@b.com", "")]
    [InlineData(@"John Doe user@host, Bob Smith user2@host", "user@host", "John Doe", "user2@host", "Bob Smith")]

    // have , as part of displayName when displayName is quoted
    [InlineData(@"""John, Doe"" <user@host>, ""Bob, Smith"" <user2@host>", "user@host", "John, Doe", "user2@host", "Bob, Smith")]
    [InlineData(@"""John, Doe"" <user@host>, Bob Smith user2@host", "user@host", "John, Doe", "user2@host", "Bob Smith")]

    // use ; as address separator
    [InlineData(@"""John, Doe"" <user@host>; ""Bob, Smith"" <user2@host>", "user@host", "John, Doe", "user2@host", "Bob, Smith")]
    public void ParseInputStringTheory(string input, string expMail1, string expName1, string expMail2, string expName2) =>
        helper.MailAddressEqual(input, expMail1, expName1, expMail2, expName2);



    [Theory(Skip = "unclear why disabled, but they fail, must ask @STV")]
    // failed. Expected:<a@a.com>. Actual:<c@c.com>.
    [InlineData(@"a@a.com b@b.com c@c.com", "a@a.com", "", "b@b.com", "")]

    // !!! have ; as part of displayName
    // failed. Expected:<John; Doe>. Actual:<John, Doe>.
    [InlineData(@"""John; Doe"" <user@host>, ""Bob; Smith"" <user2@host>", "user@host", "John; Doe", "user2@host", "Bob; Smith")]

    public void ParseInputStringThatFails(string input, string expMail1, string expName1, string expMail2, string expName2) =>
        helper.MailAddressEqual(input, expMail1, expName1, expMail2, expName2);

    [Theory]
    // FormatException: An invalid character was found in the mail header: ','.
    [InlineData(@"a@a.com,,c@c.com", "a@a.com", "", "c@c.com", "")]
    // have , as part of displayName when displayName is not quoted
    // FormatException: The specified string is not in the form required for an e-mail address.
    [InlineData(@"John, Doe user@host, Bob, Smith user2@host", "user@host", "John, Doe", "user2@host", "Bob, Smith")]
    public void ParseInputStringExpectFormatExceptions(string input, string expMail1, string expName1, string expMail2, string expName2) =>
        Throws<FormatException>(() => helper.MailAddressEqual(input, expMail1, expName1, expMail2, expName2));
}