using System.Runtime.CompilerServices;
using ToSic.Eav.Data;
using ToSic.Lib.Coding;

namespace ToSic.Sxc.Data.Experimental;

public abstract class TypedItem(ITypedItem item): ICanBeEntity
{

    public ITypedItem Item => item;

#pragma warning disable IDE0060
    protected TValue GetThis<TValue>(NoParamOrder protector = default, TValue fallback = default, [CallerMemberName] string name = default)
#pragma warning restore IDE0060
    {
        return item.Get(name, fallback: fallback);
    }

    /// <summary>
    /// This is necessary so the object can be used in places where an IEntity is expected,
    /// like toolbars.
    /// </summary>
    IEntity ICanBeEntity.Entity => Item.Entity;
}