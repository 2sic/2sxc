using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using ToSic.Sxc.Code;

namespace ToSic.Sxc.Web
{
    public interface IMailService: INeedsCodeRoot
    {
        string Send(MailMessage message);

        string Send(
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
        );
    }
}