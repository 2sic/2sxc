using ToSic.Eav.Model;
using ToSic.Sxc.Data.Sys.Factory;

namespace ToSic.Sxc.Data.Models.Sys;

public class DataModelAnalyzer
{

    /// <summary>
    /// Figure out the expected ContentTypeName of a DataWrapper type.
    /// If it is decorated with <see cref="ModelSourceAttribute"/> then use the information it provides, otherwise
    /// use the type name.
    /// </summary>
    /// <typeparam name="TCustom"></typeparam>
    /// <returns></returns>
    internal static string GetContentTypeNameCsv<TCustom>() where TCustom : ICanWrapData =>
        ContentTypeNames.Get<TCustom, ModelSourceAttribute>(a =>
        {
            // If we have an attribute, use the value provided (unless not specified)
            if (a?.ContentType != null)
                return a.ContentType;

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
    /// Figure out the expected ContentTypeName of a DataWrapper type.
    /// If it is decorated with <see cref="ModelSourceAttribute"/> then use the information it provides, otherwise
    /// use the type name.
    /// </summary>
    /// <typeparam name="TCustom"></typeparam>
    /// <returns></returns>
    public static List<string> GetContentTypeNamesList<TCustom>() where TCustom : ICanWrapData
        => ContentTypeNamesList.Get<TCustom, ModelSourceAttribute>(a => UseSpecifiedNameOrDeriveFromType<TCustom>(a?.ContentType));
    private static readonly ClassAttributeLookup<List<string>> ContentTypeNamesList = new();

    /// <summary>
    /// Get the stream names of the current type.
    /// </summary>
    /// <typeparam name="TCustom"></typeparam>
    /// <returns></returns>
    public static List<string> GetStreamNameList<TCustom>() where TCustom : ICanWrapData
        => StreamNames.Get<TCustom, ModelSourceAttribute>(attribute => UseSpecifiedNameOrDeriveFromType<TCustom>(attribute?.Stream));

    private static List<string> UseSpecifiedNameOrDeriveFromType<TCustom>(string? names)
        where TCustom : ICanWrapData
    {
        var list = !string.IsNullOrWhiteSpace(names)
            ? names!.Split(',').Select(n => n.Trim()).ToList()
            : CreateListOfNameVariants(typeof(TCustom).Name, typeof(TCustom).IsInterface);
        return list;
    }

    private static readonly ClassAttributeLookup<List<string>> StreamNames = new();

    /// <summary>
    /// Take a class/interface name and create a list
    /// which also checks for the same name without leading "I" or without trailing "Model".
    /// </summary>
    private static List<string> CreateListOfNameVariants(string name, bool isInterface)
    {
        // Start list with initial name
        List<string> result = [name];
        // Check if it ends with Model
        var nameWithoutModelSuffix = name.EndsWith("Model")
            ? name.Substring(0, name.Length - 5)
            : null;
        if (nameWithoutModelSuffix != null)
            result.Add(nameWithoutModelSuffix);

        if (isInterface && name.Length > 1 && name.StartsWith("I") && char.IsUpper(name, 1))
        {
            result.Add(name.Substring(1));
            if (nameWithoutModelSuffix != null)
                result.Add(nameWithoutModelSuffix.Substring(1));
        }

        return result;
    }


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