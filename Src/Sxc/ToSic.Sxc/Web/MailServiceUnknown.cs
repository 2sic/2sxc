using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using ToSic.Eav.Run.Unknown;

namespace ToSic.Sxc.Web
{
    public class MailServiceUnknown : MailService
    {
        public MailServiceUnknown(WarnUseOfUnknown<MailServiceUnknown> warn)
        {
            
        }
        
        public override string Send(MailMessage message)
        {
            throw new System.NotImplementedException();
        }

        public override string Send(string noParamOrder = Eav.Parameters.Protector, string mailFrom = null, string mailTo = null, string cc = null,
            string bcc = null, string replyTo = null, MailPriority priority = MailPriority.Normal, string subject = null,
            bool isBodyHtml = true, Encoding bodyEncoding = null, string body = null, List<Attachment> attachments = null)
        {
            throw new System.NotImplementedException();
        }
    }
}