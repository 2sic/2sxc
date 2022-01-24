using System;
using System.Net.Mail;
using ToSic.Eav.Context;
using ToSic.Eav.Run.Unknown;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Services
{
    public class MailServiceUnknown : MailServiceBase
    {
        public MailServiceUnknown(WarnUseOfUnknown<MailServiceUnknown> warn, Lazy<IUser> userLazy) : base(userLazy)
        { }

        protected override SmtpClient SmtpClient()
        {
            throw new System.NotImplementedException();
        }
    }
}