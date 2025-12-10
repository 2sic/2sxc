using System.Text.Json.Serialization;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Cms.Data;
using ToSic.Sxc.Data.Options;
using ToSic.Sxc.Data.Sys.Typed;
using ToSic.Sxc.Images;
using ToSic.Sxc.Services.Tweaks;
using ToSic.Sys.Performance;
using static ToSic.Sxc.Data.Sys.Typed.TypedHelpers;

namespace ToSic.Sxc.Data.Sys.Metadata;

internal partial class Metadata: ITypedItem
{
    #region Keys

    [PrivateApi]
    public bool ContainsKey(string name) =>
        TypedHelpers.ContainsKey(name, Entity,
            (e, k) => e.Attributes.ContainsKey(k),
            (e, k) => e.Children(k)?.FirstOrDefault()
        );

    public bool IsEmpty(string name, NoParamOrder npo = default, string? language = default)
        => ItemHelper.IsEmpty(name, npo, isBlank: default, language: language);

    public bool IsNotEmpty(string name, NoParamOrder npo = default, string? language = default)
        => ItemHelper.IsNotEmpty(name, npo, isBlank: default, language: language);

    [PrivateApi]
    public IEnumerable<string> Keys(NoParamOrder npo = default, IEnumerable<string>? only = default)
        => FilterKeysIfPossible(npo, only, Entity?.Attributes.Keys);

    #endregion

    #region ITyped

    object? ITyped.Get(string name, NoParamOrder npo, bool? required, string? language)
        => ItemHelper.Get(name: name, npo: npo, required: required, language: language);

    TValue? ITyped.Get<TValue>(string name, NoParamOrder npo, TValue? fallback, bool? required, string? language)
        where TValue : default
        => ItemHelper.GetT(name, npo, fallback: fallback, required: required, language: language);

    IRawHtmlString? ITyped.Attribute(string name, NoParamOrder npo, string? fallback, bool? required)
        => ItemHelper.Attribute(name, npo, fallback, required);

    DateTime ITyped.DateTime(string name, NoParamOrder npo, DateTime fallback, bool? required)
        => ItemHelper.G4T(name, npo: npo, fallback: fallback, required: required);

    string? ITyped.String(string name, NoParamOrder npo, string? fallback, bool? required, object? scrubHtml)
        => ItemHelper.String(name, npo, fallback, required, scrubHtml);

    int ITyped.Int(string name, NoParamOrder npo, int fallback, bool? required)
        => ItemHelper.G4T(name, npo: npo, fallback: fallback, required: required);

    bool ITyped.Bool(string name, NoParamOrder npo, bool fallback, bool? required)
        => ItemHelper.G4T(name, npo: npo, fallback: fallback, required: required);

    long ITyped.Long(string name, NoParamOrder npo, long fallback, bool? required)
        => ItemHelper.G4T(name, npo: npo, fallback: fallback, required: required);

    float ITyped.Float(string name, NoParamOrder npo, float fallback, bool? required)
        => ItemHelper.G4T(name, npo: npo, fallback: fallback, required: required);

    decimal ITyped.Decimal(string name, NoParamOrder npo, decimal fallback, bool? required)
        => ItemHelper.G4T(name, npo: npo, fallback: fallback, required: required);

    double ITyped.Double(string name, NoParamOrder npo, double fallback, bool? required)
        => ItemHelper.G4T(name, npo: npo, fallback: fallback, required: required);

    string? ITyped.Url(string name, NoParamOrder npo, string? fallback, bool? required)
        => ItemHelper.Url(name, npo, fallback, required);

    string ITyped.ToString() => "test / debug: " + ToString();

    #endregion

    #region ADAM

    /// <inheritdoc />
    [PrivateApi]
    IFolder? ITypedItem.Folder(string name, NoParamOrder npo, bool? required) 
        => TypedItem.Folder(name, npo, required);

    IFile? ITypedItem.File(string name, NoParamOrder npo, bool? required) 
        => TypedItem.File(name, npo, required);

    #endregion

    #region Basic Props like Id, Guid, Title, Type

    //[PrivateApi]
    //int ITypedMetadata.EntityId => Cdf.CodeInfo.GetAndWarn(V16To18("IMetadata.EntityId", message: $"Use {nameof(ITypedItem.Id)} instead of {nameof(EntityId)}"), EntityId);

    [PrivateApi]
    int ITypedItem.Id => Entity.EntityId;
    //int ITypedItem.Id => EntityId;

    [PrivateApi]
    Guid ITypedItem.Guid => Entity.EntityGuid;
    //Guid ITypedItem.Guid => EntityGuid;

    [PrivateApi]
    string? ITypedItem.Title => Entity?.GetBestTitle(Cdf.Dimensions);
    //string? ITypedItem.Title => EntityTitle;

    [PrivateApi]
    IContentType ITypedItem.Type => Entity?.Type!;

    #endregion


    #region Relationship properties Presentation, Metadata, Child, Children, Parents

    /// <inheritdoc />
    [PrivateApi]
    [JsonIgnore]
    ITypedItem ITypedItem.Presentation
        => throw new NotSupportedException($"You can't access the {nameof(ITypedItem.Presentation)} of Metadata");

    /// <inheritdoc />
    [JsonIgnore] // prevent serialization as it's not a normal property
    ITypedMetadata ITypedItem.Metadata
        => throw new NotSupportedException($"You can't access the {nameof(ITypedItem.Metadata)} of Metadata");

    [PrivateApi]
    ITypedItem ITypedItem.Parent(NoParamOrder npo, bool? current, string? type, string? field, GetRelatedOptions? options)
        => throw new NotSupportedException($"You can't access the {nameof(ITypedItem.Parent)}() of Metadata as it usually does not make sense to do this");

    /// <inheritdoc />
    [PrivateApi]
    IEnumerable<ITypedItem> ITypedItem.Parents(NoParamOrder npo, string? type, string? field, GetRelatedOptions? options)
    {
        // Protect & no Strict (as that's not really possible, since it's not a field)

        // Exit if no metadata items available to get parents from
        var mdEntities = metadata.ToList();
        if (!mdEntities.Any())
            return [];

        // Get children from first metadata item which matches the criteria
        var parents = mdEntities
            .Select(e => e.Parents(type: type, field: field).ToList())
            .FirstOrDefault(l => l.SafeAny());
        if (parents == null)
            return [];

        var list = Cdf.EntitiesToItems(parents, new() { ItemIsStrict = false, DropNullItems = true })
            .ToListOpt();
        return list.Any()
            ? list
            : [];
    }

    IPublishing ITypedItem.Publishing => _publishing.Get(() => new Publishing.Publishing(this, Cdf))!;
    private readonly GetOnce<IPublishing> _publishing = new();

    /// <inheritdoc />
    [PrivateApi]
    IEnumerable<ITypedItem> ITypedItem.Children(string? field, NoParamOrder npo, string? type, bool? required, GetRelatedOptions? options)
    {
        // Protect & Strict
        if (IsErrStrictNameOptional(this, field, required, GetHelper.PropsRequired))
            throw ErrStrict(field);

        // Exit if no metadata items available to get children from
        var mdEntities = metadata.ToList();
        if (!mdEntities.Any())
            return [];

        // Get children from first metadata item which matches the criteria
        var children = mdEntities
            .Select(e => e.Children(field: field, type: type).ToList())
            .FirstOrDefault(l => l.SafeAny());
        if (children == null)
            return [];

        // WIP - sometimes children can contain null items, but Razor won't expect this.
        // in future, we may add an option to preserve them, for now filter them out
        var filtered = children
            .Where(c => c != null)
            .Cast<IEntity>()
            .ToListOpt();

        return Cdf.EntitiesToItems(filtered, new() { ItemIsStrict = false, DropNullItems = true })
            .ToListOpt();
    }

    /// <inheritdoc />
    [PrivateApi]
    ITypedItem? ITypedItem.Child(string name, NoParamOrder npo, bool? required, GetRelatedOptions? options)
        => (this as ITypedItem)
            .Children(name, npo: npo, required: required, options: options)
            .FirstOrDefault();

    #endregion

    #region New Child<T> / Children<T> - disabled as ATM Kit is missing

    /// <inheritdoc />
    T? ITypedItem.Child<T>(string name, NoParamOrder npo, bool? required, GetRelatedOptions? options)
        where T : class
        => Cdf.AsCustom<T>(source: (this as ITypedItem).Child(name, required: required, options: options), npo: npo, mock: false);

    /// <inheritdoc />
    IEnumerable<T> ITypedItem.Children<T>(string? field, NoParamOrder npo, string? type, bool? required, GetRelatedOptions? options)
        => Cdf.AsCustomList<T>(
            source: (this as ITypedItem).Children(field: field, npo: npo, type: type, required: required, options: options),
            npo: npo,
            nullIfNull: false
        );

    /// <inheritdoc />
    T? ITypedItem.Parent<T>(NoParamOrder npo, bool? current, string? type, string? field, GetRelatedOptions? options)
        where T : class
        => Cdf.AsCustom<T>(
            source: (this as ITypedItem).Parent(npo: npo, current: current, type: type ?? typeof(T).Name, field: field, options: options), npo: npo, mock: false
        );

    /// <inheritdoc />
    IEnumerable<T> ITypedItem.Parents<T>(NoParamOrder npo, string? type, string? field, GetRelatedOptions? options)
        => Cdf.AsCustomList<T>(
            source: (this as ITypedItem).Parents(npo: npo, field: field, type: type ?? typeof(T).Name, options: options),
            npo: npo,
            nullIfNull: false
        );

    #endregion

    #region Fields, Html, Picture

    [PrivateApi]
    IField? ITypedItem.Field(string name, NoParamOrder npo, bool? required)
        => Cdf.Field(this, supportOldMetadata: false, name, new() { EntryPropIsRequired = required ?? true, ItemIsStrict = GetHelper.PropsRequired });

    IHtmlTag? ITypedItem.Html(
        string name,
        NoParamOrder npo,
        object? container,
        bool? toolbar,
        object? imageSettings,
        bool? required,
        bool debug,
        Func<ITweakInput<string>, ITweakInput<string>>? tweak
    ) => TypedItemHelpers.Html(Cdf, this, name: name, npo: npo, container: container,
        toolbar: toolbar, imageSettings: imageSettings, required: required, debug: debug, tweak: tweak);

    /// <inheritdoc/>
    IResponsivePicture? ITypedItem.Picture(
        string name,
        NoParamOrder npo,
        Func<ITweakMedia, ITweakMedia>? tweak,
        object? settings,
        object? factor,
        object? width,
        string? imgAlt,
        string? imgAltFallback,
        string? imgClass,
        object? imgAttributes,
        string? pictureClass,
        object? pictureAttributes,
        object? toolbar,
        object? recipe
    ) => TypedItemHelpers.Picture(cdf: Cdf, item: this, name: name, npo: npo,
        tweak: tweak,
        settings: settings,
        factor: factor, width: width, imgAlt: imgAlt, imgAltFallback: imgAltFallback, 
        imgClass: imgClass, imgAttributes: imgAttributes, pictureClass: pictureClass, pictureAttributes: pictureAttributes, toolbar: toolbar, recipe: recipe);

    IResponsiveImage? ITypedItem.Img(
        string name,
        NoParamOrder npo,
        Func<ITweakMedia, ITweakMedia>? tweak,
        object? settings,
        object? factor,
        object? width,
        string? imgAlt,
        string? imgAltFallback,
        string? imgClass,
        object? imgAttributes,
        object? toolbar,
        object? recipe
    ) => TypedItemHelpers.Img(cdf: Cdf, item: this, name: name, npo: npo, tweak: tweak, settings: settings,
        factor: factor, width: width, imgAlt: imgAlt, imgAltFallback: imgAltFallback,
        imgClass: imgClass, imgAttributes: imgAttributes,
        toolbar: toolbar, recipe: recipe);


    #endregion

    #region GPS

    GpsCoordinates ITypedItem.Gps(string name, NoParamOrder npo, bool? required)
        => GpsCoordinates.FromJson(((ITypedItem)this).String(name, required: required));

    #endregion

}