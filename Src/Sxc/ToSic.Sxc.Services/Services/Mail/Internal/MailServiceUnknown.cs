using System.Net.Mail;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sys.Users;

namespace ToSic.Sxc.Services.Internal;

[ShowApiWhenReleased(ShowApiMode.Never)]
internal class MailServiceUnknown : MailServiceBase
{
    public MailServiceUnknown(WarnUseOfUnknown<MailServiceUnknown> _, LazySvc<IUser> userLazy) : base(userLazy)
    { }

    protected override SmtpClient SmtpClient()
    {
        throw new System.NotImplementedException();
    }
}