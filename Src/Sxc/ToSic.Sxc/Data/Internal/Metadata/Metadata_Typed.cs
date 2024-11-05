using System.Text.Json.Serialization;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Helpers;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Cms.Data;
using ToSic.Sxc.Data.Internal.Typed;
using ToSic.Sxc.Images;
using ToSic.Sxc.Services.Tweaks;
using static ToSic.Eav.Code.Infos.CodeInfoObsolete;
using static ToSic.Sxc.Data.Internal.Typed.TypedHelpers;

namespace ToSic.Sxc.Data.Internal.Metadata;

internal partial class Metadata: ITypedItem
{
    #region Keys

    [PrivateApi]
    public bool ContainsKey(string name) =>
        TypedHelpers.ContainsKey(name, Entity,
            (e, k) => e.Attributes.ContainsKey(k),
            (e, k) => e.Children(k)?.FirstOrDefault()
        );

    public bool IsEmpty(string name, NoParamOrder noParamOrder = default, string language = default)
        => ItemHelper.IsEmpty(name, noParamOrder, isBlank: default, language: language);

    public bool IsNotEmpty(string name, NoParamOrder noParamOrder = default, string language = default)
        => ItemHelper.IsNotEmpty(name, noParamOrder, isBlank: default, language: language);

    [PrivateApi]
    public IEnumerable<string> Keys(NoParamOrder noParamOrder = default, IEnumerable<string> only = default)
        => FilterKeysIfPossible(noParamOrder, only, Entity?.Attributes.Keys);

    #endregion

    #region ITyped

    object ITyped.Get(string name, NoParamOrder noParamOrder, bool? required, string language)
        => ItemHelper.Get(name: name, noParamOrder: noParamOrder, required: required, language: language);

    TValue ITyped.Get<TValue>(string name, NoParamOrder noParamOrder, TValue fallback, bool? required, string language)
        => ItemHelper.GetT(name, noParamOrder, fallback: fallback, required: required, language: language);

    IRawHtmlString ITyped.Attribute(string name, NoParamOrder noParamOrder, string fallback, bool? required)
        => ItemHelper.Attribute(name, noParamOrder, fallback, required);

    [PrivateApi]
    [JsonIgnore]
    dynamic ITypedItem.Dyn => this;


    DateTime ITyped.DateTime(string name, NoParamOrder noParamOrder, DateTime fallback, bool? required)
        => ItemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

    string ITyped.String(string name, NoParamOrder noParamOrder, string fallback, bool? required, object scrubHtml)
        => ItemHelper.String(name, noParamOrder, fallback, required, scrubHtml);

    int ITyped.Int(string name, NoParamOrder noParamOrder, int fallback, bool? required)
        => ItemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

    bool ITyped.Bool(string name, NoParamOrder noParamOrder, bool fallback, bool? required)
        => ItemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

    long ITyped.Long(string name, NoParamOrder noParamOrder, long fallback, bool? required)
        => ItemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

    float ITyped.Float(string name, NoParamOrder noParamOrder, float fallback, bool? required)
        => ItemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

    decimal ITyped.Decimal(string name, NoParamOrder noParamOrder, decimal fallback, bool? required)
        => ItemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

    double ITyped.Double(string name, NoParamOrder noParamOrder, double fallback, bool? required)
        => ItemHelper.G4T(name, noParamOrder: noParamOrder, fallback: fallback, required: required);

    string ITyped.Url(string name, NoParamOrder noParamOrder, string fallback, bool? required)
        => ItemHelper.Url(name, noParamOrder, fallback, required);

    string ITyped.ToString() => "test / debug: " + ToString();

    #endregion

    #region ADAM

    /// <inheritdoc />
    [PrivateApi]
    IFolder ITypedItem.Folder(string name, NoParamOrder noParamOrder, bool? required) 
        => TypedItem.Folder(name, noParamOrder, required);

    IFile ITypedItem.File(string name, NoParamOrder noParamOrder, bool? required) 
        => TypedItem.File(name, noParamOrder, required);

    #endregion

    #region Basic Props like Id, Guid, Title, Type

    [PrivateApi]
    int IMetadata.EntityId => Cdf.CodeInfo.GetAndWarn(V16To18("IMetadata.EntityId", message: $"Use {nameof(ITypedItem.Id)} instead of {nameof(EntityId)}"), EntityId);

    [PrivateApi]
    int ITypedItem.Id => EntityId;

    [PrivateApi]
    Guid ITypedItem.Guid => EntityGuid;

    [PrivateApi]
    string ITypedItem.Title => EntityTitle;

    [PrivateApi]
    IContentType ITypedItem.Type => Entity?.Type;

    #endregion


    #region Relationship properties Presentation, Metadata, Child, Children, Parents

    /// <inheritdoc />
    [PrivateApi]
    [JsonIgnore]
    ITypedItem ITypedItem.Presentation => throw new NotSupportedException($"You can't access the {nameof(Presentation)} of Metadata");

    /// <inheritdoc />
    [JsonIgnore] // prevent serialization as it's not a normal property
    IMetadata ITypedItem.Metadata => throw new NotSupportedException($"You can't access the Metadata of Metadata in ITypedItem");

    [PrivateApi]
    ITypedItem ITypedItem.Parent(NoParamOrder noParamOrder, bool? current, string type, string field) =>
        throw new NotSupportedException($"You can't access the {nameof(ITypedItem.Parent)}() of Metadata as it usually does not make sense to do this");

    /// <inheritdoc />
    [PrivateApi]
    IEnumerable<ITypedItem> ITypedItem.Parents(NoParamOrder noParamOrder, string type, string field)
    {
        // Protect & no Strict (as that's not really possible, since it's not a field)

        // Exit if no metadata items available to get parents from
        var mdEntities = _metadata.ToList();
        if (!mdEntities.Any()) return new List<ITypedItem>(0);

        // Get children from first metadata item which matches the criteria
        var parents = mdEntities
            .Select(e => e.Parents(type: type, field: field)?.ToList())
            .FirstOrDefault(l => l.SafeAny());
        if (parents == null) return new List<ITypedItem>(0);

        var list = Cdf.EntitiesToItems(parents).ToList();
        return list.Any() ? list : [];
    }

    IPublishing ITypedItem.Publishing => _publishing.Get(() => new Publishing(this, Cdf));
    private readonly GetOnce<IPublishing> _publishing = new();

    /// <inheritdoc />
    [PrivateApi]
    IEnumerable<ITypedItem> ITypedItem.Children(string field, NoParamOrder noParamOrder, string type, bool? required)
    {
        // Protect & Strict
        if (IsErrStrict(this, field, required, GetHelper.PropsRequired))
            throw ErrStrict(field);

        // Exit if no metadata items available to get children from
        var mdEntities = _metadata.ToList();
        if (!mdEntities.Any()) return [];

        // Get children from first metadata item which matches the criteria
        var children = mdEntities
            .Select(e => e.Children(field: field, type: type)?.ToList())
            .FirstOrDefault(l => l.SafeAny());
        if (children == null) return [];

        return Cdf.EntitiesToItems(children).ToList();
    }

    /// <inheritdoc />
    [PrivateApi]
    ITypedItem ITypedItem.Child(string name, NoParamOrder noParamOrder, bool? required) 
        => (this as ITypedItem).Children(name, noParamOrder: noParamOrder, required:required).FirstOrDefault();

    #endregion

    #region New Child<T> / Children<T> - disabled as ATM Kit is missing

    /// <inheritdoc />
    T ITypedItem.Child<T>(string name, NoParamOrder protector, bool? required)
        => Cdf.AsCustom<T>(
            source: (this as ITypedItem).Child(name, required: required), protector: protector, mock: false
        );

    /// <inheritdoc />
    IEnumerable<T> ITypedItem.Children<T>(string field, NoParamOrder protector, string type, bool? required)
        => Cdf.AsCustomList<T>(
            source: (this as ITypedItem).Children(field: field, noParamOrder: protector, type: type, required: required),
            protector: protector,
            nullIfNull: false
        );

    /// <inheritdoc />
    T ITypedItem.Parent<T>(NoParamOrder protector, bool? current, string type, string field)
        => Cdf.AsCustom<T>(
            source: (this as ITypedItem).Parent(noParamOrder: protector, current: current, type: type ?? typeof(T).Name, field: field), protector: protector, mock: false
        );

    /// <inheritdoc />
    IEnumerable<T> ITypedItem.Parents<T>(NoParamOrder protector, string type, string field)
        => Cdf.AsCustomList<T>(
            source: (this as ITypedItem).Parents(noParamOrder: protector, field: field, type: type ?? typeof(T).Name),
            protector: protector,
            nullIfNull: false
        );

    #endregion

    #region Fields, Html, Picture

    [PrivateApi]
    IField ITypedItem.Field(string name, NoParamOrder noParamOrder, bool? required) => Cdf.Field(this, name, GetHelper.PropsRequired, noParamOrder, required);

    IHtmlTag ITypedItem.Html(
        string name,
        NoParamOrder noParamOrder,
        object container,
        bool? toolbar,
        object imageSettings,
        bool? required,
        bool debug,
        Func<ITweakInput<string>, ITweakInput<string>> tweak
    ) => TypedItemHelpers.Html(Cdf, this, name: name, noParamOrder: noParamOrder, container: container,
        toolbar: toolbar, imageSettings: imageSettings, required: required, debug: debug, tweak: tweak);

    /// <inheritdoc/>
    IResponsivePicture ITypedItem.Picture(
        string name,
        NoParamOrder noParamOrder,
        Func<ITweakMedia, ITweakMedia> tweak,
        object settings,
        object factor,
        object width,
        string imgAlt,
        string imgAltFallback,
        string imgClass,
        object imgAttributes,
        string pictureClass,
        object pictureAttributes,
        object toolbar,
        object recipe
    ) => TypedItemHelpers.Picture(cdf: Cdf, item: this, name: name, noParamOrder: noParamOrder,
        tweak: tweak,
        settings: settings,
        factor: factor, width: width, imgAlt: imgAlt, imgAltFallback: imgAltFallback, 
        imgClass: imgClass, imgAttributes: imgAttributes, pictureClass: pictureClass, pictureAttributes: pictureAttributes, toolbar: toolbar, recipe: recipe);

    IResponsiveImage ITypedItem.Img(
        string name,
        NoParamOrder noParamOrder,
        Func<ITweakMedia, ITweakMedia> tweak,
        object settings,
        object factor,
        object width,
        string imgAlt,
        string imgAltFallback,
        string imgClass,
        object imgAttributes,
        object toolbar,
        object recipe
    ) => TypedItemHelpers.Img(cdf: Cdf, item: this, name: name, noParamOrder: noParamOrder, tweak: tweak, settings: settings,
        factor: factor, width: width, imgAlt: imgAlt, imgAltFallback: imgAltFallback,
        imgClass: imgClass, imgAttributes: imgAttributes,
        toolbar: toolbar, recipe: recipe);


    #endregion

    #region GPS

    GpsCoordinates ITypedItem.Gps(string name, NoParamOrder protector, bool? required)
        => GpsCoordinates.FromJson(((ITypedItem)this).String(name, required: required));

    #endregion

}