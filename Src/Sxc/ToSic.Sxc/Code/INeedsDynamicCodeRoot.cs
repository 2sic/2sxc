using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Code
{
    [PrivateApi]
    public interface INeedsDynamicCodeRoot
    {
        void ConnectToRoot(IDynamicCodeRoot codeRoot);
    }
}
