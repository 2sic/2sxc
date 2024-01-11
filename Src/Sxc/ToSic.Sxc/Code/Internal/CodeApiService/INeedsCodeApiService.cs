using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Code.Internal;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface INeedsDynamicCodeRoot
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)] 
    void ConnectToRoot(IDynamicCodeRoot codeRoot);
}