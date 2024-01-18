using System.Net.Mail;
using ToSic.Eav.Context;
using ToSic.Eav.Internal.Unknown;
using ToSic.Lib.DI;

namespace ToSic.Sxc.Services.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class MailServiceUnknown : MailServiceBase
{
    public MailServiceUnknown(WarnUseOfUnknown<MailServiceUnknown> _, LazySvc<IUser> userLazy) : base(userLazy)
    { }

    protected override SmtpClient SmtpClient()
    {
        throw new System.NotImplementedException();
    }
}