using ToSic.Sxc.Apps.Sys;

namespace ToSic.Sxc.Sys.ExecutionContext;

[ShowApiWhenReleased(ShowApiMode.Never)]
public static class ExecutionContextExtensions
{
    ///// <summary>
    ///// Special helper to ensure that AppState is always a Sxc IApp.
    ///// There is a big risk that the code uses the Eav.Apps.IApp.
    ///// </summary>
    //public static IApp GetApp(this IExecutionContext exCtx)
    //    => exCtx.GetState<IApp>();

    /// <summary>
    /// Special helper to ensure that AppState is always a Sxc IApp.
    /// There is a big risk that the code uses the Eav.Apps.IApp.
    /// </summary>
    public static IAppForCode GetAppForCode(this IExecutionContext exCtx)
        => exCtx.GetState<IAppForCode>();
}
