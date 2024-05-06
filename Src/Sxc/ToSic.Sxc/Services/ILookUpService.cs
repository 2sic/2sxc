using ToSic.Eav.LookUp;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Services;

public interface ILookUpService
{
    IEnumerable<ILookUp> Sources { get; }

    ILookUp CreateSource(string name, IDictionary<string, string> values);

    ILookUp CreateSource(string name, ILookUp original);

    ILookUp CreateSource(string name, IEntity entity);

    ILookUp CreateSource(string name, ITypedItem item);
}