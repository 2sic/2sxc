using ToSic.Sxc.Data;

namespace ToSic.Sxc.Code.Generate.Internal.CSharpModels;
internal class ModelGeneratorConstants
{
    /// <summary>
    /// These names exist in the base class, so we need to override them with `new`.
    /// It would work without this override, but would add warnings that can confuse users.
    /// </summary>
    public static readonly string[] OverridePropertyNames =
    [
        nameof(ITypedItem.Id),
        nameof(ITypedItem.Guid),
        nameof(ITypedItem.Title),
        nameof(ITypedItem.Type),
        nameof(ITypedItem.Metadata),
        nameof(ITypedItem.Presentation),
        nameof(ITypedItem.IsPublished),
        nameof(ITypedItem.Publishing),
        // nameof(ITypedItem.Dyn), - this one is explicitly implemented, so it's not available
        nameof(ITypedItem.IsDemoItem),
        nameof(ITypedItem.Entity),
        nameof(ITypedItem.Type),
        nameof(ITypedItem.Field),
        /*nameof(ITypedItem.Html)*/ "Html",
        nameof(ITypedItem.Picture),
        nameof(ITypedItem.Url),
        nameof(ITypedItem.Img),
        nameof(ITypedItem.File),
        nameof(ITypedItem.Folder),
        nameof(ITypedItem.Gps),
        /*nameof(ITypedItem.Attribute)*/ "Attribute",

        nameof(ITypedItem.IsEmpty),
        nameof(ITypedItem.IsNotEmpty),
        nameof(ITypedItem.Keys),
        nameof(ITypedItem.ContainsKey),
        nameof(ITypedItem.Get),

        nameof(ITypedItem.Bool),
        nameof(ITypedItem.DateTime),
        nameof(ITypedItem.String),
        nameof(ITypedItem.Int),
        nameof(ITypedItem.Double),
        nameof(ITypedItem.Decimal),
        nameof(ITypedItem.Long),
        nameof(ITypedItem.Float),

        nameof(ITypedItem.Child),
        nameof(ITypedItem.Children),
        nameof(ITypedItem.Parent),
        nameof(ITypedItem.Parents),

    ];

    public static readonly string[] OverrideMethods =
    [
        nameof(ITypedItem.Field),

        nameof(ITypedItem.Get),
        nameof(ITypedItem.Bool),
        nameof(ITypedItem.DateTime),
        nameof(ITypedItem.String),
        nameof(ITypedItem.Int),
        nameof(ITypedItem.Double),
        nameof(ITypedItem.Decimal),
        nameof(ITypedItem.Long),
        nameof(ITypedItem.Float),

        nameof(ITypedItem.Child),
        nameof(ITypedItem.Children),
        nameof(ITypedItem.Parent),
        nameof(ITypedItem.Parents),

        nameof(ITypedItem.Attribute),
        nameof(ITypedItem.Url),
        nameof(ITypedItem.Img),
        nameof(ITypedItem.Picture),
        nameof(ITypedItem.Html),

        nameof(ITypedItem.File),
        nameof(ITypedItem.Folder),
        nameof(ITypedItem.Gps),

        // Key / value handling methods
        nameof(ITypedItem.ContainsKey),
        nameof(ITypedItem.IsEmpty),
        nameof(ITypedItem.IsNotEmpty),
        nameof(ITypedItem.Keys),
    ];
}
