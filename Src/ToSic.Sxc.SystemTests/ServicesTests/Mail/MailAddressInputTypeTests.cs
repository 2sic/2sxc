using System.Net.Mail;
using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Internal;

namespace ToSic.Sxc.ServicesTests.Mail;

public class MailAddressInputTypeTests(IMailService mailSvc, MailServiceTestsHelper helper)
{
    [Fact]
    public void MailAddressType()
    {
        var expected = new MailAddress(MailServiceTestsHelper.Address, MailServiceTestsHelper.DisplayName);
        var actual = ((MailServiceBase)mailSvc).MailAddress("test", expected);
        Equal(expected, actual);
    }

    [Fact]
    public void StringType()
    {
        Equal(MailServiceTestsHelper.Address, ((MailServiceBase)mailSvc).MailAddress("test", MailServiceTestsHelper.Address).Address);
    }

    // An invalid character was found in the mail header.
    [Fact]
    //[ExpectedException(typeof(FormatException))]
    public void InvalidString() =>
        Throws<FormatException>(() =>
            ((MailServiceBase)mailSvc).MailAddress("test", @";;;ffff@@@@@@gggggg"));

    // The parameter 'address' cannot be an empty string.
    [Fact]
    //[ExpectedException(typeof(ArgumentException))]
    public void EmptyString() =>
        Throws<ArgumentException>(() => ((MailServiceBase)mailSvc).MailAddress("test", string.Empty));

        // Unknown type for MailAddress
    [Fact]
    //[ExpectedException(typeof(ArgumentException))]
    public void Null() => Throws<ArgumentException>(() => ((MailServiceBase)mailSvc).MailAddress("test", null));

    [Fact]
    //[ExpectedException(typeof(ArgumentException))]
    public void UnknownType()
    {
        var mailAddress = new { a = "test" }; // unknown type
        // Trying to parse e-mails for test but got unknown type for mailAddress
        Throws<ArgumentException>(() => ((MailServiceBase)mailSvc).MailAddress("test", mailAddress));
    }
}