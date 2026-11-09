using ToSic.Sxc.Apps.Sys;

namespace ToSic.Sxc.Sys.ExecutionContext;

[ShowApiWhenReleased(ShowApiMode.Never)]
public static class ExecutionContextExtensions
{
    /// <summary>
    /// Special helper to ensure that AppState is always a Sxc IApp.
    /// There is a big risk that the code uses the Eav.Apps.IApp.
    /// </summary>
    public static IAppForCode GetApp(this IExecutionContext exCtx)
        => ((IExecutionContextWithApp)exCtx).App!;
}