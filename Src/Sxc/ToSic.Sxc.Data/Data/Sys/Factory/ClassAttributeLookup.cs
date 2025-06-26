namespace ToSic.Sxc.Data.Sys.Factory;

internal class ClassAttributeLookup<TValue>
{
    internal TValue Get<TCustom, TAttribute>(Func<TAttribute?, TValue> func)
        where TCustom : ICanWrapData
        where TAttribute : Attribute
    {
        // Check cache
        var type = typeof(TCustom);
        if (_cache.TryGetValue(type, out var typeName))
            return typeName;

        // Try to get attribute
        var attribute = type.GetDirectlyAttachedAttribute<TAttribute>();

        typeName = func(attribute);

        _cache[type] = typeName;
        return typeName;
    }
    private readonly Dictionary<Type, TValue> _cache = new();

}