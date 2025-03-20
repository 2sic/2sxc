using ToSic.Sxc.Services.Internal;

namespace ToSic.Sxc.ServicesTests.Mail;

public class MailServiceBaseTests
{
    [Fact]
    public void AutoDetectHtmlTest()
    {
        False(MailServiceBase.AutoDetectHtml(null));
        False(MailServiceBase.AutoDetectHtml(string.Empty));
        False(MailServiceBase.AutoDetectHtml("text"));
        True(MailServiceBase.AutoDetectHtml("<b>html</b>"));
    }

    [Fact]
    public void NormalizeEmailSeparatorsTest()
    {
        Null(MailServiceBase.NormalizeEmailSeparators(null));
        Null(MailServiceBase.NormalizeEmailSeparators(string.Empty));
        // all standard separators
        Equal(",,,,", MailServiceBase.NormalizeEmailSeparators(",,,,"));
        // all non standard separators
        Equal(",,,,", MailServiceBase.NormalizeEmailSeparators(";;;;"));
        // some non standard separators
        Equal(",,,,", MailServiceBase.NormalizeEmailSeparators(",,;;"));
    }
}