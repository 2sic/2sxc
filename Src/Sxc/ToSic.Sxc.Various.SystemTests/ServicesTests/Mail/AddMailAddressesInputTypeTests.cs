using System.Net.Mail;
using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Internal;
using ToSic.Sxc.Services.Mail.Sys;

namespace ToSic.Sxc.ServicesTests.Mail;

public class AddMailAddressesInputTypeTests(IMailService mailSvc, MailServiceTestsHelper helper)
{
    [Fact]
    public void MailAddressCollectionType()
    {
        var expected = new MailAddressCollection { MailServiceTestsHelper.Addresses };
        var actual = new MailAddressCollection();
        ((MailServiceBase)mailSvc).AddMailAddresses("test", actual, expected);
        Equivalent(expected, actual);
    }

    [Fact]
    public void MailAddressArrayType()
    {
        var mailAddressCollection = new MailAddressCollection { MailServiceTestsHelper.Addresses };
        var expected = mailAddressCollection.ToArray();
        var actual = new MailAddressCollection();
        ((MailServiceBase)mailSvc).AddMailAddresses("test", actual, expected);
        Equivalent(mailAddressCollection, actual);
    }

    [Fact]
    public void StringArrayType()
    {
        var expected = MailServiceTestsHelper.Emails.Split(',');
        var actual = new MailAddressCollection();
        ((MailServiceBase)mailSvc).AddMailAddresses("test", actual, expected);
        Equal(expected.Length, actual.Count);
    }

    [Fact]
    public void StringArrayTypeWithTwoEmptyItems()
    {
        const string addresses = @"a@a.com,,,c@c.com,d@d.com";
        var expected = addresses.Split(',');
        var actual = new MailAddressCollection();
        ((MailServiceBase)mailSvc).AddMailAddresses("test", actual, expected);
        Equal(expected.Length, actual.Count+2);
    }

    [Fact]
    public void StringType()
    {
        const int expected = 4; // number of emails in Emails string
        var actual = new MailAddressCollection();
        ((MailServiceBase)mailSvc).AddMailAddresses("test", actual, MailServiceTestsHelper.Emails);
        Equal(expected, actual.Count);
    }

    [Fact]
    public void StringTypeWithNonStandardSeparator()
    {
        const string emailsWithNonStandardSeparator = @"a@a.com;b@b.com;c@c.com;d@d.com";
        const int expected = 4; // number of emails
        var actual = new MailAddressCollection();
        ((MailServiceBase)mailSvc).AddMailAddresses("test", actual, emailsWithNonStandardSeparator);
        Equal(expected, actual.Count);
    }

    [Fact]
    //[ExpectedException(typeof(FormatException))]
    public void InvalidString()
    {
        // An invalid character was found in the mail header.
        Throws<FormatException>(() => ((MailServiceBase)mailSvc).MailAddress("test", @";;;ffff@@@@@@gggggg"));
    }

    [Fact]
    public void EmptyString()
    {
        // The parameter 'mailAddresses' can be empty string.
        const int expected = 0; // number of emails
        var actual = new MailAddressCollection();
        ((MailServiceBase)mailSvc).AddMailAddresses("test", actual, string.Empty);
        Equal(expected, actual.Count);
    }

    [Fact]
    public void Null()
    {
        // The parameter 'mailAddresses' can be null.
        const int expected = 0; // number of emails
        var actual = new MailAddressCollection();
        ((MailServiceBase)mailSvc).AddMailAddresses("test", actual, null);
        Equal(expected, actual.Count);
    }

    [Fact]
    //[ExpectedException(typeof(ArgumentException))]
    public void UnknownType() =>
        Throws<ArgumentException>(() =>
        {
            var mailAddresses = new { a = "test" }; // unknown type
            // Trying to parse e-mails for test but got unknown type for mailAddresses.
            ((MailServiceBase)mailSvc).AddMailAddresses("test", new(), mailAddresses);
        });
}