using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Code
{
    [PrivateApi]
    public interface INeedsCodeRoot
    {
        void AddBlockContext(IDynamicCodeRoot codeRoot);
    }
}
