using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Data;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IFromField
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    IField Field { get; set; }
}