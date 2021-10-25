using System.Net.Mail;
using ToSic.Eav.Run.Unknown;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Services
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