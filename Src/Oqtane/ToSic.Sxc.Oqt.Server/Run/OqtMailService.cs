using Oqtane.Repository;
using Oqtane.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using ToSic.Eav;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtMailService : MailService
    {
        private readonly Lazy<ISiteRepository> _siteRepositoryLazy;
        private readonly Lazy<ISettingRepository> _settingRepositoryLazy;

        public OqtMailService(Lazy<ISiteRepository> siteRepositoryLazy, Lazy<ISettingRepository> settingRepositoryLazy)
        {
            _siteRepositoryLazy = siteRepositoryLazy;
            _settingRepositoryLazy = settingRepositoryLazy;
        }

        public override string Send(MailMessage message)
        {
            try
            {
                var client = SmtpClient();
                if (client == null)
                    return "SMTP Not Configured Properly In Site Settings - Host, Port, SSL, And Sender Are All Required";

                client.Send(message);
                return "Message: Send Ok";
            }
            catch (Exception ex)
            {
                // error
                return $"Error: {ex.Message}.";
            }
        }

        public override string Send(
            string noParamOrder = Parameters.Protector,
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
            mailMessage.From = new MailAddress(mailFrom);
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

        private Dictionary<string, string> GetSettings()
        {
            var sitedId = base.App.Site.Id;

            var site = _siteRepositoryLazy.Value.GetSite(sitedId);

            // get site settings
            var settings = _settingRepositoryLazy.Value.GetSettings(EntityNames.Site, site.SiteId).ToList();

            return settings.OrderBy(item => item.SettingName).ToList().ToDictionary(setting => setting.SettingName, setting => setting.SettingValue);
        }

        private SmtpClient SmtpClient()
        {
            var settings = GetSettings();
            if (!settings.ContainsKey("SMTPHost") || settings["SMTPHost"] == "" 
                || !settings.ContainsKey("SMTPPort") || settings["SMTPPort"] == "" 
                || !settings.ContainsKey("SMTPSSL") || settings["SMTPSSL"] == "" 
                || !settings.ContainsKey("SMTPSender") || settings["SMTPSender"] == "") return null;

            // construct SMTP Client
            var client = new SmtpClient()
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Host = settings["SMTPHost"],
                Port = int.Parse(settings["SMTPPort"]),
                EnableSsl = bool.Parse(settings["SMTPSSL"])
            };

            if (settings["SMTPUsername"] != "" && settings["SMTPPassword"] != "")
            {
                client.Credentials = new NetworkCredential(settings["SMTPUsername"], settings["SMTPPassword"]);
            }

            return client;
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
