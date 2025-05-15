using ToSic.Sxc.Apps;

namespace ToSic.Sxc.Sys.ExecutionContext;
public static class IExecutionContextGetApp
{
    /// <summary>
    /// Special helper to ensure that AppState is always a Sxc IApp.
    /// There is a big risk that the code uses the Eav.Apps.IApp.
    /// </summary>
    public static IApp GetApp(this IExecutionContext exCtx)
        => exCtx.GetState<IApp>();
}
