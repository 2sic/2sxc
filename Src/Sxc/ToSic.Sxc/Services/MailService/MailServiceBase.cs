using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Services
{
    public abstract class MailServiceBase : HasLog, IMailService
    {
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

            if (from is MailAddress mailAddress)
                mailMessage.From = mailAddress;
            else if (from is string fromString)
                mailMessage.From = new MailAddress(fromString);





            //if (!string.IsNullOrEmpty(from)) mailMessage.From = new MailAddress(from);
            //AddMailAddresses(mailMessage.To, to);
            //AddMailAddresses(mailMessage.CC, cc);
            //AddMailAddresses(mailMessage.Bcc, bcc);
            //if (!string.IsNullOrEmpty(replyTo)) mailMessage.ReplyTo = new MailAddress(replyTo);
            //mailMessage.Subject = subject;
            //mailMessage.SubjectEncoding = encoding ?? Encoding.UTF8;
            //mailMessage.IsBodyHtml = isHtml ?? AutoDetectHtml(body);
            //mailMessage.BodyEncoding = encoding ?? Encoding.UTF8;
            //mailMessage.Body = body;

            //foreach (var attachment in attachments)
            //{
            //    mailMessage.Attachments.Add(attachment);
            //}

            return mailMessage;
        }

        // TODO: STV
        private bool AutoDetectHtml(string body)
        {
            throw new NotImplementedException();
        }

        public void Send(
            string noParamOrder =
                "Rule: all params must be named (https://r.2sxc.org/named-params), Example: \'enable: true, version: 10\'",
            string @from = null,
            string to = null,
            string cc = null,
            string bcc = null,
            string replyTo = null,
            string subject = null,
            bool? isHtml = null,
            Encoding encoding = null,
            string body = null,
            IEnumerable<Attachment> attachments = null)
        {
            // prevent incorrect use without named parameters
            Eav.Parameters.ProtectAgainstMissingParameterNames(noParamOrder, $"{nameof(Send)}", $"{nameof(to)},{nameof(body)}");

            var mailMessage = Create(
                @from: @from,
                to: to,
                cc: cc,
                bcc: bcc,
                replyTo: replyTo,
                subject: subject,
                isHtml: isHtml,
                encoding: encoding,
                body: body,
                attachments: attachments);

            //var mailMessage = new MailMessage();
            //if (!string.IsNullOrEmpty(mailFrom)) mailMessage.From = new MailAddress(mailFrom);
            //AddMailAddresses(mailMessage.To, mailTo);
            //AddMailAddresses(mailMessage.CC, cc);
            //AddMailAddresses(mailMessage.Bcc, bcc);
            //if (!string.IsNullOrEmpty(replyTo)) mailMessage.ReplyTo = new MailAddress(replyTo);
            //mailMessage.Priority = priority;
            //mailMessage.Subject = subject;
            //mailMessage.IsBodyHtml = isBodyHtml;
            //mailMessage.BodyEncoding = bodyEncoding;
            //mailMessage.Body = body;

            //foreach (var attachment in attachments)
            //{
            //    mailMessage.Attachments.Add(attachment);
            //}

            Send(mailMessage);
        }

        private void AddMailAddresses(MailAddressCollection mails, string emails)
        {
            if (string.IsNullOrEmpty(emails)) return;
            foreach (var email in emails.Split(",;".ToCharArray()))
            {
                mails.Add(new MailAddress(email));
            }
        }

        public MailAddress GetMailAddress(string input)
        {
            const string pattern = @"^(""?(.+)[,;\s]""?)?<?(([\w\.\-]+)@([\w\.\-]+))>?$";
            var match = Regex.Match(input.Trim(), pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            
            if (!match.Success) return null;

            var displayName = match.Groups[2].Value.Trim(" ,;".ToCharArray());
            var address = match.Groups[3].Value.Trim(" <>".ToCharArray());
            return new MailAddress(address, displayName);
        }
    }
}