using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public static class IHasKitExtensions
{
    public static TServiceKit GetKit<TServiceKit>(this ICodeApiService codeRoot) where TServiceKit : ServiceKit
    {
        if (codeRoot is IHasKit<TServiceKit> withKit && withKit.Kit != null)
            return withKit.Kit;
        return codeRoot.GetService<TServiceKit>();
    }

    internal static ICodeApiService SetCompatibility(this ICodeApiService codeRoot, int compatibility)
    {
        codeRoot._Cdf.SetCompatibilityLevel(compatibility);
        return codeRoot;
    }
}