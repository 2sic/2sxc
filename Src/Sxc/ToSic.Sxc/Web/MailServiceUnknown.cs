using System.Net.Mail;
using ToSic.Eav.Run.Unknown;

namespace ToSic.Sxc.Web
{
    public class MailServiceUnknown : MailServiceBase
    {
        public MailServiceUnknown(WarnUseOfUnknown<MailServiceUnknown> warn)
        { }

        protected override SmtpClient SmtpClient()
        {
            throw new System.NotImplementedException();
        }
    }
}