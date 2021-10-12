using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code;

namespace ToSic.Sxc.Web
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

        public string Send(MailMessage message)
        {
            try
            {
                using (var client = SmtpClient())
                {
                    if (client == null)
                        return "SMTP Not Configured Properly In Site Settings - Host, Port, SSL, And Sender Are All Required";

                    client.Send(message);
                }

                return ""; // return "Message: Send Ok";
            }
            catch (SmtpFailedRecipientException ex)
            {
                return "Error Failed Recipient: " + ex.Message;
            }
            catch (SmtpException ex)
            {
                return "Error SMTP Configuration Problem: " + ex.Message;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    return "Error: " + ex.Message + Environment.NewLine + ex.InnerException.Message;

                }
                return "Error: " + ex.Message;

            }
            finally
            {
                message.Dispose();
            }
        }

        public string Send(
            string noParamOrder = Eav.Parameters.Protector,
            string mailFrom = null,
            string mailTo = null,
            string cc = null,
            string bcc = null,
            string replyTo = null,
            MailPriority priority = MailPriority.Normal,
            string subject = null,
            bool isBodyHtml = true,
            Encoding bodyEncoding = null,
            string body = null,
            List<Attachment> attachments = null
        )
        {
            // prevent incorrect use without named parameters
            Eav.Parameters.ProtectAgainstMissingParameterNames(noParamOrder, $"{nameof(Send)}", $"{nameof(mailTo)},{nameof(body)}");

            var mailMessage = new MailMessage();
            if (!string.IsNullOrEmpty(mailFrom)) mailMessage.From = new MailAddress(mailFrom);
            AddMailAddresses(mailMessage.To, mailTo);
            AddMailAddresses(mailMessage.CC, cc);
            AddMailAddresses(mailMessage.Bcc, bcc);
            if (!string.IsNullOrEmpty(replyTo)) mailMessage.ReplyTo = new MailAddress(replyTo);
            mailMessage.Priority = priority;
            mailMessage.Subject = subject;
            mailMessage.IsBodyHtml = isBodyHtml;
            mailMessage.BodyEncoding = bodyEncoding;
            mailMessage.Body = body;

            foreach (var attachment in attachments)
            {
                mailMessage.Attachments.Add(attachment);
            }

            return Send(mailMessage);
        }

        private void AddMailAddresses(MailAddressCollection mails, string emails)
        {
            if (string.IsNullOrEmpty(emails)) return;
            foreach (var email in emails.Split(",;".ToCharArray()))
            {
                mails.Add(new MailAddress(email));
            }
        }
    }
}