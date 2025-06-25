using ToSic.Sxc.Dnn.Services;
using ToSic.Sys.Code.InfoSystem;


// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Services;

/// <summary>
/// Obsolete, use <see cref="ISystemLogService"/> instead
/// </summary>
internal class LogServiceUsingOldInterface: DnnSystemLogService
{
    public LogServiceUsingOldInterface(CodeInfoService codeInfos)
    {
        throw new("The ToSic.Sxc.Services.ILogServices is deprecated / removed. Please use ToSic.Sxc.Services.ISystemLogService instead.");
        //codeInfos.Warn(V16To18("ToSic.Sxc.Services.ILogService", message: "Use ToSic.Sxc.Services.ISystemLogService instead"));
    }
}