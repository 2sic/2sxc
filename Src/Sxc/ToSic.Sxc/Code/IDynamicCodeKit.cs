using ToSic.Eav.Documentation;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code
{
    [PrivateApi("v14")]
    public interface IDynamicCodeKit<out TKit> where TKit : ServiceKit
    {
        TKit Kit { get; }
    }

    public static class IDynamicCodeKitExtensions
    {
        public static TKit GetKit<TKit>(this IDynamicCodeRoot codeRoot) where TKit : ServiceKit
        {
            if (codeRoot is IDynamicCodeKit<TKit> hasKit) return hasKit.Kit;
            return codeRoot.GetService<TKit>();
        }
    }

}
