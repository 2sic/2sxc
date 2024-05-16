using System.IO;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using ToSic.Eav.Apps.Assets;
using ToSic.Eav.Context;
using ToSic.Lib.DI;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code.Internal;
using MailMessage = System.Net.Mail.MailMessage;

namespace ToSic.Sxc.Services.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract class MailServiceBase(LazySvc<IUser> userLazy)
    : ServiceForDynamicCode($"{SxcLogName}.MailSrv", connect: [userLazy]), IMailService
{
    private static readonly Regex HtmlDetectionRegex = new("<(.*\\s*)>", RegexOptions.Compiled);

    [PrivateApi] protected IApp App;

    /// <inheritdoc />
    public override void ConnectToRoot(ICodeApiService codeRoot)
    {
        base.ConnectToRoot(codeRoot);
        App = codeRoot.App;
    }

    protected abstract SmtpClient SmtpClient();

    /// <inheritdoc />
    public void Send(MailMessage message)
    {
        var l = Log.Fn();
        try
        {
            using var client = SmtpClient();
            client.Send(message);
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            if (userLazy.Value.IsSystemAdmin)
                throw;
            throw new("SMTP configuration problem.");
        }

        l.Done();
    }

    /// <inheritdoc />
    public MailMessage Create(
        NoParamOrder noParamOrder = default,
        object from = null,
        object to = null,
        object cc = null,
        object bcc = null,
        object replyTo = null,
        string subject = null,
        string body = null,
        bool? isHtml = null,
        Encoding encoding = null,
        object attachments = null)
    {
        var l = Log.Fn<MailMessage>(
            parameters: $"{nameof(from)}: {from}, {nameof(to)}: {to}, {nameof(cc)}: {cc}, {nameof(bcc)}: {bcc}, {nameof(replyTo)}: {replyTo}, " +
                        $"{nameof(subject)}: {subject}, {nameof(body)}: {body}, {nameof(isHtml)}: {isHtml}, {nameof(encoding)}: {encoding}, " +
                        $"{nameof(attachments)}: {attachments}");

        // prevent incorrect use without named parameters

        var mailMessage = new MailMessage();
            
        if (from != null) mailMessage.From = MailAddress(nameof(from), from);
        AddMailAddresses(nameof(to), mailMessage.To, to);
        AddMailAddresses(nameof(cc), mailMessage.CC, cc);
        AddMailAddresses(nameof(bcc), mailMessage.Bcc, bcc);
        AddMailAddresses(nameof(replyTo), mailMessage.ReplyToList, replyTo);

        mailMessage.Subject = subject;
        mailMessage.SubjectEncoding = encoding ?? Encoding.UTF8;
        mailMessage.IsBodyHtml = isHtml ?? AutoDetectHtml(body);
        mailMessage.BodyEncoding = encoding ?? Encoding.UTF8;
        mailMessage.Body = body;

        AddAttachments(mailMessage.Attachments, attachments);

        return l.Return(mailMessage, "done");
    }

    [PrivateApi] 
    public static bool AutoDetectHtml(string body)
    {
        return !string.IsNullOrEmpty(body) && HtmlDetectionRegex.IsMatch(body);
    }

    /// <inheritdoc />
    public void Send(
        NoParamOrder noParamOrder = default,
        object from = null,
        object to = null,
        object cc = null,
        object bcc = null,
        object replyTo = null,
        string subject = null,
        string body = null,
        bool? isHtml = null,
        Encoding encoding = null,
        object attachments = null)
    {
        var l = Log.Fn();
        // Note: don't log all the parameters here, because we'll do it again on the Create-call
        var mailMessage = Create(
            from: from,
            to: to,
            cc: cc,
            bcc: bcc,
            replyTo: replyTo,
            subject: subject,
            body: body,
            isHtml: isHtml, encoding: encoding, attachments: attachments);

        Send(mailMessage);
        l.Done();
    }

    // 2024-01-10 2dm internalized - doesn't seem in use, and also not clear why we would have this
    // was probably an experiment from STV during dev, but we shouldn't keep it in the interface
    internal MailAddress MailAddress(string addressType, object mailAddress)
    {
        switch (mailAddress)
        {
            case MailAddress fromMailAddress:
                return fromMailAddress;
            case string fromString:
                return new(fromString);
            default:
                throw new ArgumentException($"Trying to parse e-mails for {addressType} but got unknown type for {nameof(mailAddress)}");
        }
    }

    // 2024-01-10 2dm internalized - doesn't seem in use, and also not clear why we would have this
    // was probably an experiment from STV during dev, but we shouldn't keep it in the interface
    internal bool AddMailAddresses(string addressType, MailAddressCollection targetMails, object mailAddresses)
    {
        var l = Log.Fn<bool>(); // return a bool just to make return-statements easier later on

        switch (mailAddresses)
        {
            case MailAddressCollection inputMailAddressCollection:
                foreach (var mailAddress in inputMailAddressCollection) 
                    targetMails.Add(mailAddress);
                return l.ReturnTrue(nameof(MailAddressCollection));

            case IEnumerable<MailAddress> inputMailAddressesArray:
                foreach (var mailAddress in inputMailAddressesArray) 
                    targetMails.Add(mailAddress);
                return l.ReturnTrue(nameof(IEnumerable<MailAddress>));

            case IEnumerable<string> inputStringArray:
                foreach (var emailAddress in inputStringArray)
                    if (!string.IsNullOrEmpty(emailAddress))
                        targetMails.Add(emailAddress);
                return l.ReturnTrue(nameof(IEnumerable<string>));

            case string inputString: 
                if (!string.IsNullOrEmpty(inputString)) 
                    targetMails.Add(NormalizeEmailSeparators(inputString));
                return l.ReturnTrue("string");

            case null:
                return l.ReturnTrue("null");

            default:
                throw new ArgumentException($"Trying to parse e-mails for {addressType} but got unknown type for {nameof(mailAddresses)}");
        }
    }

    [PrivateApi]
    public static string NormalizeEmailSeparators(string input)
    {
        return string.IsNullOrEmpty(input) ? null : input.Replace(";", ",");
    }

    public bool AddAttachments(AttachmentCollection targetAttachments, object attachments)
    {
        var l = Log.Fn<bool>(); // return a bool just to make return-statements easier later on
        switch (attachments)
        {
            case Attachment inputAttachment:
                targetAttachments.Add(inputAttachment);
                return l.ReturnTrue(nameof(Attachment));

            case AttachmentCollection attachmentCollection:
                foreach (var attachment in attachmentCollection) 
                    targetAttachments.Add(attachment);
                return l.ReturnTrue(nameof(AttachmentCollection));

            case IEnumerable<Attachment> inputAttachmentsArray:
                foreach (var attachment in inputAttachmentsArray) 
                    targetAttachments.Add(attachment);
                return l.ReturnTrue(nameof(IEnumerable<Attachment>));

            case IFile inputFile:
                targetAttachments.Add(new(
                    new FileStream(inputFile.PhysicalPath, FileMode.Open, FileAccess.Read, FileShare.Read),
                    inputFile.FullName));
                return l.ReturnTrue(nameof(IFile));

            case IEnumerable<IFile> inputFiles:
                foreach (var file in inputFiles)
                    targetAttachments.Add(new(
                        new FileStream(file.PhysicalPath, FileMode.Open, FileAccess.Read, FileShare.Read),
                        file.FullName));
                return l.ReturnTrue(nameof(IEnumerable<IFile>));

            default:
                throw new ArgumentException($"Unknown type for {nameof(attachments)}");
        }
    }
}