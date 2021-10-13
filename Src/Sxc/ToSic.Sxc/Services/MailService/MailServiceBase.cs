using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
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

        protected MailServiceBase() : base($"{Constants.SxcLogName}.MailSrv")
        { }

        public virtual void AddBlockContext(IDynamicCodeRoot codeRoot)
        {
            Log.LinkTo(codeRoot.Log);
            App = codeRoot.App;
        }

        protected abstract SmtpClient SmtpClient();

        public void Send(MailMessage message)
        {
            var wrapLog = Log.Call();
            try
            {
                using (var client = SmtpClient()) 
                    client.Send(message);
                wrapLog("ok");
            }
            catch (Exception ex)
            {
                Log.Exception(ex);
                wrapLog("error");
                throw;
            }
        }


        public MailMessage Create(string noParamOrder = Eav.Parameters.Protector,
            object from = null,
            object to = null,
            object cc = null,
            object bcc = null,
            object replyTo = null,
            string subject = null,
            bool? isHtml = null,
            Encoding encoding = null,
            string body = null,
            object attachments = null)
        {
            // prevent incorrect use without named parameters
            Eav.Parameters.ProtectAgainstMissingParameterNames(noParamOrder, $"{nameof(Send)}", $"{nameof(to)},{nameof(body)}");

            var mailMessage = new MailMessage();
            
            if (from != null) mailMessage.From = MailAddress(from);
            AddMailAddresses(mailMessage.To, to);
            AddMailAddresses(mailMessage.CC, cc);
            AddMailAddresses(mailMessage.Bcc, bcc);
            AddMailAddresses(mailMessage.ReplyToList, replyTo);

            mailMessage.Subject = subject;
            mailMessage.SubjectEncoding = encoding ?? Encoding.UTF8;
            mailMessage.IsBodyHtml = isHtml ?? AutoDetectHtml(body);
            mailMessage.BodyEncoding = encoding ?? Encoding.UTF8;
            mailMessage.Body = body;

            AddAttachments(mailMessage.Attachments, attachments);

            return mailMessage;
        }

        private static bool AutoDetectHtml(string body)
        {
            return !string.IsNullOrEmpty(body) && HtmlDetectionRegex.IsMatch(body);
        }

        public void Send(
            string noParamOrder =
                "Rule: all params must be named (https://r.2sxc.org/named-params), Example: \'enable: true, version: 10\'",
            object from = null,
            object to = null,
            object cc = null,
            object bcc = null,
            object replyTo = null,
            string subject = null,
            bool? isHtml = null,
            Encoding encoding = null,
            string body = null,
            object attachments = null)
        {
            // prevent incorrect use without named parameters
            Eav.Parameters.ProtectAgainstMissingParameterNames(noParamOrder, $"{nameof(Send)}", $"{nameof(to)},{nameof(body)}");

            var mailMessage = Create(
                from: from,
                to: to,
                cc: cc,
                bcc: bcc,
                replyTo: replyTo,
                subject: subject,
                isHtml: isHtml,
                encoding: encoding,
                body: body,
                attachments: attachments);

            Send(mailMessage);
        }

        public static MailAddress MailAddress(object input)
        {
            switch (input)
            {
                case MailAddress mailAddress:
                    return mailAddress;
                case string fromString:
                    return new MailAddress(fromString);
                default:
                    throw new ArgumentException("Unknown type for MailAddress");
            }
        }

        public static void AddMailAddresses(MailAddressCollection mails, object input)
        {
            switch (input)
            {
                case MailAddressCollection inputMailAddressCollection:
                    foreach (var mailAddress in inputMailAddressCollection)
                    {
                        mails.Add(mailAddress);
                    }
                    break;

                case IEnumerable<MailAddress> inputMailAddressesArray:
                    foreach (var mailAddress in inputMailAddressesArray)
                    {
                        mails.Add(mailAddress);
                    }
                    break;

                case IEnumerable<string> inputStringArray:
                    foreach (var emailAddress in inputStringArray)
                    {
                        if (!string.IsNullOrEmpty(emailAddress)) 
                            mails.Add(NormalizeEmailSeparators(emailAddress));
                    }
                    break;

                case string inputString: 
                    if (!string.IsNullOrEmpty(inputString)) 
                        mails.Add(NormalizeEmailSeparators(inputString));
                    break;
                    
                default:
                    throw new ArgumentException("Unknown type for MailAddressCollection");
            }
        }

        public static string NormalizeEmailSeparators(string input)
        {
            return string.IsNullOrEmpty(input) ? null : input.Replace(";", ",");
        }

        public static void AddAttachments(AttachmentCollection attachments, object input)
        {
            switch (input)
            {
                case Attachment inputAttachment:
                    attachments.Add(inputAttachment);
                    break;

                case AttachmentCollection attachmentCollection:
                    foreach (var attachment in attachmentCollection)
                    {
                        attachments.Add(attachment);
                    }
                    break;

                case IEnumerable<Attachment> inputAttachmentsArray:
                    foreach (var attachment in inputAttachmentsArray)
                    {
                        attachments.Add(attachment);
                    }
                    break;

                case Adam.IFile inputFile:
                    attachments.Add(new Attachment(
                        new FileStream(inputFile.PhysicalPath, FileMode.Open, FileAccess.Read, FileShare.Read),
                        inputFile.FullName));
                    break;

                case IEnumerable<Adam.IFile> inputFiles:
                    foreach (var file in inputFiles)
                    {
                        attachments.Add(new Attachment(
                            new FileStream(file.PhysicalPath, FileMode.Open, FileAccess.Read, FileShare.Read),
                            file.FullName));
                    }
                    break;

                default:
                    throw new ArgumentException("Unknown type for AttachmentCollection");
            }
        }
    }
}