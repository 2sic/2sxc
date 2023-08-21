using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Data
{
    [PrivateApi]
    public interface IFromField
    {
        IField Field { get; set; }
    }
}
