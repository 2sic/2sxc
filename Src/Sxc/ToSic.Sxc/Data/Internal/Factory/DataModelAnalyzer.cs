using System.Reflection;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Data.Internal.Factory;
using ToSic.Sxc.Models;
using ToSic.Sxc.Models.Attributes;

namespace ToSic.Sxc.Data.Internal;

internal class DataModelAnalyzer
{

    /// <summary>
    /// Figure out the expected ContentTypeName of a DataWrapper type.
    /// If it is decorated with <see cref="DataModelAttribute"/> then use the information it provides, otherwise
    /// use the type name.
    /// </summary>
    /// <typeparam name="TCustom"></typeparam>
    /// <returns></returns>
    internal static string GetContentTypeNames<TCustom>() where TCustom : IDataModel =>
        ContentTypeNames.Get<TCustom, DataModelAttribute>(a =>
        {
            // If we have an attribute, use the value provided (unless not specified)
            if (a?.ForContentTypes != null)
                return a.ForContentTypes;

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
    internal static string GetStreamName<TCustom>() where TCustom : IDataModel =>
        StreamNames.Get<TCustom, DataModelAttribute>(a =>
            // if we have the attribute, use that
            a?.StreamNames.Split(',').First().Trim()
            // If no attribute, use name of type
            ?? typeof(TCustom).Name);
    private static readonly ClassAttributeLookup<string> StreamNames = new();

    internal static Type GetTargetType<TCustom>()
    {
        var type = typeof(TCustom);

        // Find attributes which describe conversion
        var attributes = type
            .GetCustomAttributes(typeof(DataModelConversion), false)
            .Cast<DataModelConversion>()
            .ToList();


        var map = attributes
            .FirstOrDefault()?.Map?
            .Select(m =>
            {
                if (!m.IsGenericType) return null;
                if (m.GetGenericTypeDefinition() != typeof(DataModelFrom<,,>))
                    return null;

                var args = m.GetGenericArguments();
                return new { From = args[0], To = args[2] };
            })
            .Where(m => m != null)
            .ToList();

        if (map == null || map.Count == 0)
            return !type.IsInterface 
                ? type
                : throw new TypeInitializationException(type.FullName, new($"Can't determine type to create of {type.Name} as it's an interface and doesn't have the proper Attributes"));

        return map.First().To;
    }

}