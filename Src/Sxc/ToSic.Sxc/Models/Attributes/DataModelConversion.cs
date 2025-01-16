namespace ToSic.Sxc.Models.Attributes;

/// <summary>
/// Attribute to decorate interfaces which should be used to retrieve a data model.
/// </summary>
/// <remarks>
/// It's primary property is the Map, which is an array of types that should be used to map the data model to.
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
public class DataModelConversion: Attribute
{
    public Type[] Map { get; init; }
}

/// <summary>
/// Conversion map from one type - typically <see cref="IEntity"/> to a DataModel.
/// </summary>
/// <typeparam name="TFrom"></typeparam>
/// <typeparam name="TImplements"></typeparam>
/// <typeparam name="TTo"></typeparam>
public class DataModelFrom<TFrom, TImplements, TTo>
    where TTo : class, TImplements, IDataModelOf<TFrom>, new()
    where TImplements : class
    where TFrom : ICanBeEntity;