using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Code
{
    // Todo: Merge with ICoupledDynamicCode as it does the same thing
    [PrivateApi]
    public interface INeedsCodeRoot
    {
        void AddBlockContext(IDynamicCodeRoot codeRoot);
    }
}
