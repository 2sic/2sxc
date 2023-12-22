using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Data;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IFromField
{
    IField Field { get; set; }
}