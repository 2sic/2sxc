using ToSic.Sxc.Services;

namespace ToSic.Sxc.Data;

[PrivateApi("WIP, don't publish yet")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface ITypedItemWrapper16
{
    string ForContentType { get; }
    void Setup(ITypedItem baseItem, ServiceKit16 addKit);
}