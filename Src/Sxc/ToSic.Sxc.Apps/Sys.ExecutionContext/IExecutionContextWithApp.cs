using ToSic.Sxc.Apps.Sys;

namespace ToSic.Sxc.Sys.ExecutionContext;

public interface IExecutionContextWithApp
{
    IAppForCode? App { get; }
}