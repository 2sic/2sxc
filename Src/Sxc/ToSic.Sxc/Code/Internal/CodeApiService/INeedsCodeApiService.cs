namespace ToSic.Sxc.Code.Internal;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface INeedsCodeApiService
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)] 
    void ConnectToRoot(ICodeApiService codeRoot);
}