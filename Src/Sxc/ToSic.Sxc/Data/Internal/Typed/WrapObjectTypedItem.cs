using System.Collections;
using System.Text.Json.Serialization;
using ToSic.Eav.Data.Shared;
using ToSic.Eav.Metadata;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Lib.Helpers;
using ToSic.Razor.Blade;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Adam.Internal;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Cms.Data;
using ToSic.Sxc.Data.Internal.Convert;
using ToSic.Sxc.Data.Internal.Wrapper;
using ToSic.Sxc.Images;
using ToSic.Sxc.Services.Internal;
using ToSic.Sxc.Services.Tweaks;
using static ToSic.Sxc.Data.Internal.Typed.TypedHelpers;

namespace ToSic.Sxc.Data.Internal.Typed;

[JsonConverter(typeof(DynamicJsonConverter))]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class WrapObjectTypedItem(LazySvc<IScrub> scrubSvc, LazySvc<ConvertForCodeService> forCodeConverter)
    : WrapObjectTyped(scrubSvc, forCodeConverter), ITypedItem
{
    internal WrapObjectTypedItem Setup(ILazyLike<CodeDataFactory> cdf, CodeDataWrapper wrapper, PreWrapObject preWrap)
    {
        Setup(preWrap);
        Wrapper = wrapper;
        _cdf = cdf;
        return this;
    }

    private CodeDataFactory Cdf => _cdf.Value;
    private ILazyLike<CodeDataFactory> _cdf;
    private CodeDataWrapper Wrapper { get; set; }


    [PrivateApi]
    [JsonIgnore]
    dynamic ITypedItem.Dyn => throw new NotSupportedException($"{nameof(ITypedItem.Dyn)} is not supported on the {nameof(ITypedStack)} by design");

    public TValue Get<TValue>(string name, NoParamOrder noParamOrder = default, TValue fallback = default,
        bool? required = default, string language = default)
        => PreWrap.TryGetTyped(name, noParamOrder, fallback, required: required);

    bool ITypedItem.IsDemoItem => PreWrap.TryGetTyped(nameof(ITypedItem.IsDemoItem), noParamOrder: default, fallback: false, required: false);

    IHtmlTag ITypedItem.Html(string name, NoParamOrder noParamOrder, object container, bool? toolbar,
        object imageSettings, bool? required, bool debug, Func<ITweakInput<string>, ITweakInput<string>> tweak
    ) => TypedItemHelpers.Html(Cdf, this, name: name, noParamOrder: noParamOrder, container: container,
        toolbar: toolbar, imageSettings: imageSettings, required: required, debug: debug, tweak: tweak);

    IResponsivePicture ITypedItem.Picture(string name, NoParamOrder noParamOrder, Func<ITweakMedia, ITweakMedia> tweak, object settings,
        object factor, object width, string imgAlt, string imgAltFallback,
        string imgClass, object imgAttributes, string pictureClass,
        object pictureAttributes, object toolbar, object recipe
    ) => TypedItemHelpers.Picture(cdf: Cdf, item: this, name: name, noParamOrder: noParamOrder,
        tweak: tweak, settings: settings, factor: factor, width: width, imgAlt: imgAlt,
        imgAltFallback: imgAltFallback, imgClass: imgClass, imgAttributes: imgAttributes, pictureClass: pictureClass, pictureAttributes: pictureAttributes, toolbar: toolbar, recipe: recipe);

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


    public int Id => PreWrap.TryGetTyped(nameof(Id), noParamOrder: default, fallback: 0, required: false);

    public Guid Guid => PreWrap.TryGetTyped(nameof(Guid), noParamOrder: default, fallback: Guid.Empty, required: false);

    public string Title => _title.Get(() => PreWrap.TryGetTyped<string>(nameof(ITypedItem.Title), noParamOrder: default, fallback: null, required: false));
    private readonly GetOnce<string> _title = new();

    #region Properties which return null or empty

    public IEntity Entity => null;
    public IContentType Type => null;

    #region Relationships - Child, Children, Parents, Presentation

    public ITypedItem Child(string name, NoParamOrder noParamOrder, bool? required) => CreateItemFromProperty(name);

    public IEnumerable<ITypedItem> Children(string field, NoParamOrder noParamOrder, string type, bool? required)
    {
        var blank = Enumerable.Empty<ITypedItem>();
        var r = PreWrap.TryGetWrap(field);
        if (!r.Found || r.Raw == null || r.Raw.GetType().IsValueType) return blank;
        if (r.Raw is not IEnumerable re)
        {
            var rawWrapped = Wrapper.TypedItemFromObject(r.Raw, PreWrap.Settings);
            return rawWrapped == null ? null : new[] { rawWrapped };
        }

        var list = re.Cast<object>()
            .Where(o => o != null && !o.GetType().IsValueType)
            .ToList();
            
        var items = list.Select(l => Wrapper.TypedItemFromObject(l, PreWrap.Settings));

        if (type.HasValue())
            items = items.Where(i => i.String(nameof(ITypedItem.Type), required: false).EqualsInsensitive(type)).ToList();
            
        return items;
    }

    public ITypedItem Parent(NoParamOrder noParamOrder = default, bool? current = default, string type = default, string field = default) => 
        throw new NotSupportedException($"You can't access the {nameof(Parent)}() here");


    /// <summary>
    /// The parents are "fake" so they behave just like children... but under the node "Parents".
    /// If "field" is specified, then it will assume another child-level under the node parents
    /// </summary>
    public IEnumerable<ITypedItem> Parents(NoParamOrder noParamOrder, string type, string field)
    {
        var blank = Enumerable.Empty<ITypedItem>();
        var typed = this as ITypedItem;
        var items = typed.Children(nameof(ITypedItem.Parents), type: type)?.ToList();

        if (items == null || !items.Any() && !field.HasValue())
            return items ?? blank;

        if (field.HasValue())
            items = items.Where(i => i.String("Field", required: false).EqualsInsensitive(field)).ToList();
        return items;
    }

    bool ITypedItem.IsPublished => true;

    IPublishing ITypedItem.Publishing => _publishing.Get(() => new PublishingUnsupported(this));
    private readonly GetOnce<IPublishing> _publishing = new();

    [JsonIgnore]
    public ITypedItem Presentation => _presentation.Get(() => CreateItemFromProperty(nameof(Presentation)));
    private readonly GetOnce<ITypedItem> _presentation = new();

    private ITypedItem CreateItemFromProperty(string name)
    {
        var result = PreWrap.TryGetWrap(name);
        if (!result.Found || result.Raw == null || result.Raw.GetType().IsValueType)
            return null;
        var first = result.Raw is IEnumerable re ? re.Cast<object>().FirstOrDefault() : result.Raw;
        if (first == null || first.GetType().IsValueType)
            return null;
        return Wrapper.TypedItemFromObject(first, PreWrap.Settings);
    }

    #endregion




    public IFolder Folder(string name, NoParamOrder noParamOrder, bool? required)
    {
        return IsErrStrict(this, name, required, PreWrap.Settings.PropsRequired)
            ? throw ErrStrictForTyped(this, name)
            : Cdf.AdamManager.Folder(Guid, name, Field(name: name, noParamOrder: default, required: required));
    }

    public IFile File(string name, NoParamOrder noParamOrder, bool? required)
    {
        if (IsErrStrict(this, name, required, PreWrap.Settings.PropsRequired))
            throw ErrStrictForTyped(this, name);
        var typed = this as ITypedItem;
        // Check if it's a direct string, or an object with a sub-property with a Value
        var idString = typed.String(name) ?? typed.Child(name)?.String("Value");

        // TODO: SEE if we can also provide optional metadata

        var fileId = AdamManager.CheckIdStringForId(idString);
        return fileId == null ? null : Cdf.AdamManager.File(fileId.Value);
    }

    #endregion

    #region New Child<T> / Children<T> - disabled as ATM Kit is missing

    T ITypedItem.Child<T>(string name, NoParamOrder protector, bool? required)
        => Cdf.AsCustom<T>(
            source: (this as ITypedItem).Child(name, required: required), protector: protector, mock: false
        );

    IEnumerable<T> ITypedItem.Children<T>(string field, NoParamOrder protector, string type, bool? required)
        => Cdf.AsCustomList<T>(
            source: (this as ITypedItem).Children(field: field, noParamOrder: protector, type: type, required: required),
            protector: protector,
            nullIfNull: false
        );

    T ITypedItem.Parent<T>(NoParamOrder protector, bool? current, string type, string field)
        => Cdf.AsCustom<T>(
            source: (this as ITypedItem).Parent(noParamOrder: protector, current: current, type: type ?? typeof(T).Name, field: field), protector: protector, mock: false
        );

    IEnumerable<T> ITypedItem.Parents<T>(NoParamOrder protector, string type, string field)
        => Cdf.AsCustomList<T>(
            source: (this as ITypedItem).Parents(noParamOrder: protector, field: field, type: type ?? typeof(T).Name), protector: protector, nullIfNull: false
        );

    #endregion

    #region Not Supported Properties such as Entity, Type, Child, Folder, Presentation, Metadata

    [JsonIgnore] // prevent serialization as it's not a normal property
    IMetadata ITypedItem.Metadata => _metadata ??= BuildMetadata(PreWrap.TryGetWrap(nameof(Metadata.Metadata)).Raw);
    private IMetadata _metadata;

    private IMetadata BuildMetadata(object raw)
    {
        var objList = raw != null
            ? raw is IEnumerable rawEnum
                ? rawEnum.Cast<object>().ToList()
                : [raw]
            : [];

        var df = Cdf.Services.DataFactory.New(
            options: new(appId: Cdf.BlockOrNull?.AppId, autoId: false));
        var mdEntities = objList
            .Where(o => o != null)
            .Select(o =>
            {
                var values = o.ToDicInvariantInsensitive();
                // Note: id/guid don't really work, but it's never used in metadata context
                //var id = values.TryGetValue(nameof(Id), out var maybeId) ? maybeId.ConvertOrFallback(0) : 0;
                //var guid = values.TryGetValue(nameof(Guid), out var maybeGuid) ? maybeGuid.ConvertOrFallback(Guid.Empty) : Guid.Empty;
                return df.Create(values);
            })
            .ToList();

        var mdOf = new MetadataOf<int>(0, 0, "virtual", mdEntities);
        // TODO: @2dm - this probably won't work yet, without an entity (null) #todoTyped
        var metadata = Cdf.Metadata(mdOf);
        return metadata;
    }


    public IField Field(string name, NoParamOrder noParamOrder, bool? required) 
        => new Field(this, name, Cdf);

    /// <summary>
    /// Override the URL, to also support checks for "file:72"
    /// </summary>
    string ITyped.Url(string name, NoParamOrder noParamOrder, string fallback, bool? required)
    {
        var url = PreWrap.TryGetTyped(name, noParamOrder: noParamOrder, fallback, required: required);
        if (url == null) return null;

        // ReSharper disable once ConvertTypeCheckPatternToNullCheck
        if (ValueConverterBase.CouldBeReference(url))
            url = Cdf.Services.ValueConverterOrNull?.ToValue(url, Guid.Empty) ?? url;

        return Tags.SafeUrl(url).ToString();
    }
    #endregion

    #region GPS

    GpsCoordinates ITypedItem.Gps(string name, NoParamOrder protector, bool? required)
        => GpsCoordinates.FromJson(((ITypedItem)this).String(name, required: required));

    #endregion


    IBlock ICanBeItem.TryGetBlockContext() => Cdf?.BlockOrNull;

    public ITypedItem Item => this;

    /// <summary>
    /// Get by name should never throw an error, as it's used to get null if not found.
    /// </summary>
    object ICanGetByName.Get(string name) => (this as ITypedItem).Get(name, required: false);

    #region Equals

    /// <summary>
    /// This is used by various equality comparison. 
    /// Since we define two object to be equal when they host the same contents, this determines the hash based on the contents
    /// </summary>
    [PrivateApi]
    // ReSharper disable once NonReadonlyMemberInGetHashCode
    public override int GetHashCode() => WrapperEquality.GetWrappedHashCode(this.PreWrap);

    public override bool Equals(object b)
    {
        if (b is null) return false;
        if (ReferenceEquals(this, b)) return true;
        if (b.GetType() != GetType()) return false;
        if (b is not WrapObjectTypedItem bTyped) return false;
        if (bTyped.PreWrap.GetType() != PreWrap.GetType()) return false;

        // TODO: ATM not clear how to best do this
        // probably need to check what's inside the PreWrap...
        return WrapperEquality.EqualsWrapper(this.PreWrap, bTyped.PreWrap);
    }

    bool IEquatable<ITypedItem>.Equals(ITypedItem other) => Equals(other);

    #endregion
}