using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Mail.Sys;

namespace ToSic.Sxc.ServicesTests.Mail;

public class MailServiceBaseTests(IMailService mailSvc)
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

    [Fact]
    public void Create_NoAttachments_CreatesMessageWithoutAttachments()
    {
        using var message = mailSvc.Create(
            from: "from@example.com",
            to: "to@example.com",
            subject: "Test",
            body: "Body");

        Empty(message.Attachments);
    }
}
