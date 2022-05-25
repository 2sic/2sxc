using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using ToSic.Eav.Apps.Assets;
using ToSic.Eav.Context;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code;
using MailMessage = System.Net.Mail.MailMessage;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Services
{
    public abstract class MailServiceBase : HasLog, IMailService
    {
        private static readonly Regex HtmlDetectionRegex = new Regex("<(.*\\s*)>", RegexOptions.Compiled);

        [PrivateApi] protected IApp App;

        private readonly Lazy<IUser> _userLazy;

        protected MailServiceBase(Lazy<IUser> userLazy) : base($"{Constants.SxcLogName}.MailSrv")
        { 
            _userLazy = userLazy;
        }

        /// <inheritdoc />
        public virtual void ConnectToRoot(IDynamicCodeRoot codeRoot)
        {
            Log.LinkTo(codeRoot.Log);
            App = codeRoot.App;
        }

        protected abstract SmtpClient SmtpClient();

        /// <inheritdoc />
        public void Send(MailMessage message)
        {
            var wrapLog = Log.Fn();
            try
            {
                using (var client = SmtpClient()) 
                    client.Send(message);
                wrapLog.Done("ok");
            }
            catch (Exception ex)
            {
                Log.Ex(ex);
                wrapLog.Done("error");
                if (_userLazy.Value.IsSuperUser)
                    throw;
                else
                    throw new Exception("SMTP configuration problem."); 
            }
        }

        /// <inheritdoc />
        public MailMessage Create(
            string noParamOrder = Eav.Parameters.Protector,
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
            var wrapLog = Log.Fn<MailMessage>(
                parameters: $"{nameof(from)}: {from}, {nameof(to)}: {to}, {nameof(cc)}: {cc}, {nameof(bcc)}: {bcc}, {nameof(replyTo)}: {replyTo}, " +
                            $"{nameof(subject)}: {subject}, {nameof(body)}: {body}, {nameof(isHtml)}: {isHtml}, {nameof(encoding)}: {encoding}, " +
                            $"{nameof(attachments)}: {attachments}");

            // prevent incorrect use without named parameters
            Eav.Parameters.ProtectAgainstMissingParameterNames(noParamOrder, $"{nameof(Create)}", 
                $"{nameof(from)}, {nameof(to)}, {nameof(cc)}, {nameof(bcc)}, {nameof(replyTo)}, " +
                $"{nameof(subject)}, {nameof(body)}, {nameof(isHtml)}, {nameof(encoding)}, {nameof(attachments)}");

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

            return wrapLog.Return(mailMessage, "done");
        }

        [PrivateApi] 
        public static bool AutoDetectHtml(string body)
        {
            return !string.IsNullOrEmpty(body) && HtmlDetectionRegex.IsMatch(body);
        }

        /// <inheritdoc />
        public void Send(
            string noParamOrder = Eav.Parameters.Protector,
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
            // Note: don't log all the parameters here, because we'll do it again on the Create-call
            var wrapLog = Log.Fn();
            // prevent incorrect use without named parameters
            Eav.Parameters.ProtectAgainstMissingParameterNames(noParamOrder, $"{nameof(Send)}",
                $"{nameof(from)}, {nameof(to)}, {nameof(cc)}, {nameof(bcc)}, {nameof(replyTo)}, " +
                $"{nameof(subject)}, {nameof(body)}, {nameof(isHtml)}, {nameof(encoding)}, {nameof(attachments)}");

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
            wrapLog.Done("done");
        }

        public MailAddress MailAddress(string addressType, object mailAddress)
        {
            switch (mailAddress)
            {
                case MailAddress fromMailAddress:
                    return fromMailAddress;
                case string fromString:
                    return new MailAddress(fromString);
                default:
                    throw new ArgumentException($"Trying to parse e-mails for {addressType} but got unknown type for {nameof(mailAddress)}");
            }
        }

        public bool AddMailAddresses(string addressType, MailAddressCollection targetMails, object mailAddresses)
        {
            var wrapLog = Log.Fn<bool>(); // return a bool just to make return-statements easier later on

            switch (mailAddresses)
            {
                case MailAddressCollection inputMailAddressCollection:
                    foreach (var mailAddress in inputMailAddressCollection) 
                        targetMails.Add(mailAddress);
                    return wrapLog.ReturnTrue(nameof(MailAddressCollection));

                case IEnumerable<MailAddress> inputMailAddressesArray:
                    foreach (var mailAddress in inputMailAddressesArray) 
                        targetMails.Add(mailAddress);
                    return wrapLog.ReturnTrue(nameof(IEnumerable<MailAddress>));

                case IEnumerable<string> inputStringArray:
                    foreach (var emailAddress in inputStringArray)
                        if (!string.IsNullOrEmpty(emailAddress))
                            targetMails.Add(emailAddress);
                    return wrapLog.ReturnTrue(nameof(IEnumerable<string>));

                case string inputString: 
                    if (!string.IsNullOrEmpty(inputString)) 
                        targetMails.Add(NormalizeEmailSeparators(inputString));
                    return wrapLog.ReturnTrue("string");

                case null:
                    return wrapLog.ReturnTrue("null");

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
            var wrapLog = Log.Fn<bool>(); // return a bool just to make return-statements easier later on
            switch (attachments)
            {
                case Attachment inputAttachment:
                    targetAttachments.Add(inputAttachment);
                    return wrapLog.ReturnTrue(nameof(Attachment));

                case AttachmentCollection attachmentCollection:
                    foreach (var attachment in attachmentCollection) 
                        targetAttachments.Add(attachment);
                    return wrapLog.ReturnTrue(nameof(AttachmentCollection));

                case IEnumerable<Attachment> inputAttachmentsArray:
                    foreach (var attachment in inputAttachmentsArray) 
                        targetAttachments.Add(attachment);
                    return wrapLog.ReturnTrue(nameof(IEnumerable<Attachment>));

                case IFile inputFile:
                    targetAttachments.Add(new Attachment(
                        new FileStream(inputFile.PhysicalPath, FileMode.Open, FileAccess.Read, FileShare.Read),
                        inputFile.FullName));
                    return wrapLog.ReturnTrue(nameof(IFile));

                case IEnumerable<IFile> inputFiles:
                    foreach (var file in inputFiles)
                        targetAttachments.Add(new Attachment(
                            new FileStream(file.PhysicalPath, FileMode.Open, FileAccess.Read, FileShare.Read),
                            file.FullName));
                    return wrapLog.ReturnTrue(nameof(IEnumerable<IFile>));

                default:
                    throw new ArgumentException($"Unknown type for {nameof(attachments)}");
            }
        }
    }
}