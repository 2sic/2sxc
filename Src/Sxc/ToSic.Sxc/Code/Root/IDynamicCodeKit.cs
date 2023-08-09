using ToSic.Lib.Documentation;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code
{
    [PrivateApi("v14")]
    public interface IDynamicCodeKit<out TServiceKit> where TServiceKit : ServiceKit
    {
        /// <summary>
        /// The Service Kit containing all kinds of services which are commonly used.
        /// The services on the Kit are context-aware, so they know what App is currently being used etc.
        /// </summary>
        TServiceKit Kit { get; }
    }

    public static class IDynamicCodeKitExtensions
    {
        public static TServiceKit GetKit<TServiceKit>(this IDynamicCodeRoot codeRoot) where TServiceKit : ServiceKit
        {
            if (codeRoot is IDynamicCodeKit<TServiceKit> withKit && withKit.Kit != null)
                return withKit.Kit;
            return codeRoot.GetService<TServiceKit>();
        }

        public static IDynamicCodeRoot SetCompatibility(this IDynamicCodeRoot codeRoot, int compatibility)
        {
            codeRoot.Cdf.SetCompatibilityLevel(compatibility);
            return codeRoot;
        }
    }

}
