using ToSic.Sxc.Data.Internal.Factory;
using ToSic.Sxc.Data.Models;

namespace ToSic.Sxc.Data.Internal;

internal class DataModelAnalyzer
{

    /// <summary>
    /// Figure out the expected ContentTypeName of a DataWrapper type.
    /// If it is decorated with <see cref="ModelSourceAttribute"/> then use the information it provides, otherwise
    /// use the type name.
    /// </summary>
    /// <typeparam name="TCustom"></typeparam>
    /// <returns></returns>
    internal static string GetContentTypeNames<TCustom>() where TCustom : ICanWrapData =>
        ContentTypeNames.Get<TCustom, ModelSourceAttribute>(a =>
        {
            // If we have an attribute, use the value provided (unless not specified)
            if (a?.ContentTypes != null)
                return a.ContentTypes;

            // If no attribute, use name of type
            var type = typeof(TCustom);
            var typeName = type.Name;
            // If type is Interface: drop the "I" as this can't be a content-type
            // TODO: not sure if this is a good idea
            return typeName.StartsWith("I") && type.IsInterface
                ? typeName.Substring(1)
                : typeName;
        });
    private static readonly ClassAttributeLookup<string> ContentTypeNames = new();

    /// <summary>
    /// Get the stream names of the current type.
    /// </summary>
    /// <typeparam name="TCustom"></typeparam>
    /// <returns></returns>
    internal static string GetStreamName<TCustom>() where TCustom : ICanWrapData =>
        StreamNames.Get<TCustom, ModelSourceAttribute>(a =>
            // if we have the attribute, use that
            a?.Streams.Split(',').First().Trim()
            // If no attribute, use name of type
            ?? typeof(TCustom).Name);
    private static readonly ClassAttributeLookup<string> StreamNames = new();

    internal static Type GetTargetType<TCustom>()
    {
        var type = typeof(TCustom);

        if (TargetTypes.TryGetValue(type, out var cachedType))
            return cachedType;

        // Find attributes which describe conversion
        var attributes = type
            .GetCustomAttributes(typeof(ModelCreationAttribute), false)
            .Cast<ModelCreationAttribute>()
            .ToList();

        // 2025-01-21 temp
        var implementation = attributes.FirstOrDefault()?.Use;
        if (implementation != null)
        {
            TargetTypes[type] = implementation;
            return implementation;
        }


        if (type.IsInterface)
            throw new TypeInitializationException(type.FullName,
                new($"Can't determine type to create of {type.Name} as it's an interface and doesn't have the proper Attributes"));
        TargetTypes[type] = type;
        return type;
    }

    private static readonly Dictionary<Type, Type> TargetTypes = new();
}