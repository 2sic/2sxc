using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public static class IDynamicCodeKitExtensions
{
    public static TServiceKit GetKit<TServiceKit>(this IDynamicCodeRoot codeRoot) where TServiceKit : ServiceKit
    {
        if (codeRoot is IDynamicCodeKit<TServiceKit> withKit && withKit.Kit != null)
            return withKit.Kit;
        return codeRoot.GetService<TServiceKit>();
    }

    internal static IDynamicCodeRoot SetCompatibility(this IDynamicCodeRoot codeRoot, int compatibility)
    {
        codeRoot.Cdf.SetCompatibilityLevel(compatibility);
        return codeRoot;
    }
}