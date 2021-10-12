using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code;

namespace ToSic.Sxc.Web
{
    public abstract class MailService : HasLog, IMailService
    {
        [PrivateApi] protected IApp App;

        protected MailService() : base($"{Constants.SxcLogName}.MailSrv")
        { }

        public virtual void AddBlockContext(IDynamicCodeRoot codeRoot)
        {
            Log.LinkTo(codeRoot.Log);
            App = codeRoot.App;
        }

        public abstract string Send(MailMessage message);

        public abstract string Send(
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
            List<Attachment> attachments = null);
    }
}