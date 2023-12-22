using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Code;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface INeedsDynamicCodeRoot
{
    void ConnectToRoot(IDynamicCodeRoot codeRoot);
}