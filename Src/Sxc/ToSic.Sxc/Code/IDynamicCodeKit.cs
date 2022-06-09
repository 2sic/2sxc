using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Code
{
    [PrivateApi("v14")]
    public interface IDynamicCodeKit<out TKit>
    {
        TKit Kit { get; }
    }

    public static class IDynamicCodeKitExtensions
    {
        public static TKit GetKit<TKit>(this IDynamicCodeRoot codeRoot)
        {
            if (codeRoot is IDynamicCodeKit<TKit> hasKit) return hasKit.Kit;
            return codeRoot.GetService<TKit>();
        }
    }

}
