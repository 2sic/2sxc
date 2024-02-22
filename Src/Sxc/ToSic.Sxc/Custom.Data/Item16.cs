using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Cms.Data;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Internal;
using ToSic.Sxc.Data.Internal.Convert;
using ToSic.Sxc.Images;
using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Tweaks;

// ReSharper disable once CheckNamespace
namespace Custom.Data;

/// <summary>
/// Base class for custom data objects, which are typed and can be used in Razor Components
/// </summary>
[PrivateApi("WIP, don't publish yet")]
[JsonConverter(typeof(DynamicJsonConverter))]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
// ReSharper disable once UnusedMember.Global
public abstract class Item16: ITypedItem, ITypedItemWrapper16, IHasJsonSource, IHasPropLookup
{

    void ITypedItemWrapper16.Setup(ITypedItem baseItem, ServiceKit16 addKit)
    {
        _item = baseItem;
        Kit = addKit;
    }

    /// <summary>
    /// The actual item which is being wrapped, in rare cases where you must access it.
    ///
    /// It's only on the explicit interface, so it is not available from outside or inside, unless you cast to it.
    /// Goal is that inheriting classes don't access it to keep API surface small.
    /// </summary>
    ITypedItem ICanBeItem.Item => _item;
    private ITypedItem _item;

    IBlock ICanBeItem.TryGetBlockContext() => _item.TryGetBlockContext();

    /// <summary>
    /// Kit - private, so not available to inheriting classes for now; keep API surface small. 
    /// </summary>
    private ServiceKit16 Kit { get; set; }

    /// <summary>
    /// Override ToString to give more information about the current object
    /// </summary>
    public override string ToString()
    {
        var msg = $"Custom Data Model {GetType().FullName}";
        return _item == null ? $"{msg} without backing data (null)" : msg + $" for id:{Id} ({_item})";
    }

    /// <summary>
    /// Get a value from the current item, which has the same name as the calling property.
    /// Usage:
    ///
    /// * `public string Something => GetThis&lt;string&gt;();` (without fallback you must specify the type)
    /// * `public string Something => GetThis(fallback: "default value");` (with fallback the type is auto-detected)
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="protector"></param>
    /// <param name="fallback"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    protected TValue GetThis<TValue>(NoParamOrder protector = default, TValue fallback = default, [CallerMemberName] string name = default)
        => _item.Get(name, protector, fallback: fallback);

    #region Auto-Replay Properties


    /// <summary>
    /// This is necessary so the object can be used in places where an IEntity is expected,
    /// like toolbars.
    ///
    /// It's an explicit interface implementation, so that the object itself doesn't broadcast this.
    /// </summary>
    [PrivateApi]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    IEntity ICanBeEntity.Entity => _item.Entity;

    /// <inheritdoc />
    public bool ContainsKey(string name) => ((IHasKeys)_item).ContainsKey(name);

    /// <inheritdoc />
    public IEnumerable<string> Keys(NoParamOrder noParamOrder = default, IEnumerable<string> only = default) => _item.Keys(noParamOrder, only);

    /// <inheritdoc />
    public object Get([CallerMemberName] string name = default, NoParamOrder noParamOrder = default, bool? required = default) => _item.Get(name, noParamOrder, required);

    /// <inheritdoc />
    public TValue Get<TValue>([CallerMemberName] string name = default, NoParamOrder noParamOrder = default, TValue fallback = default,
        bool? required = default) =>
        _item.Get(name, noParamOrder, fallback, required);

    /// <inheritdoc />
    public bool Bool([CallerMemberName] string name = default, NoParamOrder noParamOrder = default, bool fallback = default, bool? required = default) => _item.Bool(name, noParamOrder, fallback, required);

    /// <inheritdoc />
    public DateTime DateTime([CallerMemberName] string name = default, NoParamOrder noParamOrder = default, DateTime fallback = default,
        bool? required = default) =>
        _item.DateTime(name, noParamOrder, fallback, required);

    /// <inheritdoc />
    public string String([CallerMemberName] string name = default, NoParamOrder noParamOrder = default, string fallback = default, bool? required = default,
        object scrubHtml = default) =>
        _item.String(name, noParamOrder, fallback, required, scrubHtml);

    /// <inheritdoc />
    public int Int([CallerMemberName] string name = default, NoParamOrder noParamOrder = default, int fallback = default, bool? required = default) => _item.Int(name, noParamOrder, fallback, required);

    /// <inheritdoc />
    public long Long([CallerMemberName] string name = default, NoParamOrder noParamOrder = default, long fallback = default, bool? required = default) => _item.Long(name, noParamOrder, fallback, required);

    /// <inheritdoc />
    public float Float([CallerMemberName] string name = default, NoParamOrder noParamOrder = default, float fallback = default, bool? required = default) => _item.Float(name, noParamOrder, fallback, required);

    /// <inheritdoc />
    public decimal Decimal([CallerMemberName] string name = default, NoParamOrder noParamOrder = default, decimal fallback = default, bool? required = default) => _item.Decimal(name, noParamOrder, fallback, required);

    /// <inheritdoc />
    public double Double([CallerMemberName] string name = default, NoParamOrder noParamOrder = default, double fallback = default, bool? required = default) => _item.Double(name, noParamOrder, fallback, required);

    /// <inheritdoc />
    public string Url([CallerMemberName] string name = default, NoParamOrder noParamOrder = default, string fallback = default, bool? required = default) => _item.Url(name, noParamOrder, fallback, required);

    /// <inheritdoc />
    public IRawHtmlString Attribute([CallerMemberName] string name = default, NoParamOrder noParamOrder = default, string fallback = default,
        bool? required = default) =>
        _item.Attribute(name, noParamOrder, fallback, required);

    /// <inheritdoc />
    public bool IsEmpty(string name, NoParamOrder noParamOrder = default) => _item.IsEmpty(name, noParamOrder);

    /// <inheritdoc />
    public bool IsNotEmpty(string name, NoParamOrder noParamOrder = default) => _item.IsNotEmpty(name, noParamOrder);


    /// <inheritdoc />
    public bool Equals(ITypedItem other) => _item.Equals(other);

    /// <inheritdoc />
    public bool IsDemoItem => _item.IsDemoItem;

    /// <inheritdoc />
    public IHtmlTag Html(string name, NoParamOrder noParamOrder = default, object container = default, bool? toolbar = default,
        object imageSettings = default, bool? required = default, bool debug = default, Func<ITweakInput<string>, ITweakInput<string>> tweak = default) =>
        _item.Html(name, noParamOrder, container, toolbar, imageSettings, required, debug, tweak);

    /// <inheritdoc />
    public IResponsivePicture Picture(string name, NoParamOrder noParamOrder = default, object settings = default,
        object factor = default, object width = default, string imgAlt = default, string imgAltFallback = default,
        string imgClass = default, object imgAttributes = default, string pictureClass = default,
        object pictureAttributes = default, object toolbar = default, object recipe = default) =>
        _item.Picture(name, noParamOrder, settings, factor, width, imgAlt, imgAltFallback, imgClass, imgAttributes, pictureClass, pictureAttributes, toolbar, recipe);

    /// <inheritdoc />
    public IFolder Folder([CallerMemberName] string name = default, NoParamOrder noParamOrder = default, bool? required = default) => _item.Folder(name, noParamOrder, required);

    /// <inheritdoc />
    public IFile File([CallerMemberName] string name = default, NoParamOrder noParamOrder = default, bool? required = default) => _item.File(name, noParamOrder, required);

    /// <inheritdoc />
    public ITypedItem Child([CallerMemberName] string name = default, NoParamOrder noParamOrder = default, bool? required = default) => _item.Child(name, noParamOrder, required);

    /// <inheritdoc />
    public IEnumerable<ITypedItem> Children([CallerMemberName] string field = default, NoParamOrder noParamOrder = default, string type = default,
        bool? required = default) =>
        _item.Children(field, noParamOrder, type, required);

    /// <inheritdoc />
    public ITypedItem Parent(NoParamOrder noParamOrder = default, bool? current = default, string type = default,
        string field = default) =>
        _item.Parent(noParamOrder, current, type, field);

    /// <inheritdoc />
    public IEnumerable<ITypedItem> Parents(NoParamOrder noParamOrder = default, string type = default, string field = default) => _item.Parents(noParamOrder, type, field);

    /// <inheritdoc />
    public bool IsPublished => _item.IsPublished;

    /// <inheritdoc />
    public IPublishing Publishing => _item.Publishing;

    /// <summary>
    /// Explicit, obsolete `Dyn` implementation, not to be used.
    /// </summary>
    [PrivateApi]
    [Obsolete("Not available on Custom objects, use Get(...) to access any property.")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    dynamic ITypedItem.Dyn => throw new NotSupportedException($"{nameof(ITypedItem.Dyn)} is not supported on the {nameof(ITypedStack)} by design");

    /// <inheritdoc />
    public ITypedItem Presentation => _item.Presentation;

    /// <inheritdoc />
    public IMetadata Metadata => _item.Metadata;

    /// <inheritdoc />
    public IField Field(string name, NoParamOrder noParamOrder = default, bool? required = default) => _item.Field(name, noParamOrder, required);

    /// <inheritdoc />
    public int Id => _item.Id;

    /// <inheritdoc />
    public Guid Guid => _item.Guid;

    /// <inheritdoc />
    public string Title => _item.Title;

    /// <inheritdoc />
    public IContentType Type => _item.Type;

    #endregion

    object IHasJsonSource.JsonSource() => _item?.JsonSource();

    #region New Child<T> / Children<T>

    /// <inheritdoc />
    public T Child<T>([CallerMemberName] string name = default, NoParamOrder protector = default,
        bool? required = default)
        where T : class, ITypedItemWrapper16, ITypedItem, new()
        => _item.Child<T>(name, protector: protector, required: required);

    /// <inheritdoc />
    public IEnumerable<T> Children<T>([CallerMemberName] string field = default, NoParamOrder protector = default,
        string type = default, bool? required = default)
        where T : class, ITypedItemWrapper16, ITypedItem, new()
        => _item.Children<T>(field: field, protector: protector, type: type, required: required);

    /// <inheritdoc />
    public T Parent<T>(NoParamOrder protector = default, bool? current = default, string type = default,
        string field = default)
        where T : class, ITypedItemWrapper16, ITypedItem, new()
        => _item.Parent<T>(protector: protector, current: current, type: type, field: field);

    /// <inheritdoc />
    public IEnumerable<T> Parents<T>(NoParamOrder protector = default,
        string type = default, string field = default)
        where T : class, ITypedItemWrapper16, ITypedItem, new()
        => _item.Parents<T>(protector: protector, type: type, field: field);

    protected GpsCoordinates Gps(string name, NoParamOrder protector = default, bool? required = default)
        => Kit.Json.To<GpsCoordinates>(String(name, required: required, fallback: "{}"));

    #endregion

    IPropertyLookup IHasPropLookup.PropertyLookup => _propLookup ??= ((IHasPropLookup)((ICanBeItem)this).Item).PropertyLookup;
    private IPropertyLookup _propLookup;

}