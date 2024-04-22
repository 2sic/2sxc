using ToSic.Eav.Code.InfoSystem;
using ToSic.Sxc.Dnn.Services;
using static ToSic.Eav.Code.Infos.CodeInfoObsolete;


// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Services;

/// <summary>
/// Obsolete, use <see cref="ISystemLogService"/> instead
/// </summary>
internal class LogServiceUsingOldInterface: DnnSystemLogService
{
    public LogServiceUsingOldInterface(CodeInfoService codeInfos)
    {
        codeInfos.Warn(V16To18("ToSic.Sxc.Services.ILogService", message: "Use ToSic.Sxc.Services.ISystemLogService instead"));
    }
}