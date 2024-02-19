using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
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
public abstract class Item16: ICanBeEntity, ITypedItem, ITypedItemWrapper16, IHasJsonSource
{
    private ITypedItem item;
    private ServiceKit16 kit;

    void ITypedItemWrapper16.Setup(ITypedItem baseItem, ServiceKit16 addKit)
    {
        item = baseItem;
        kit = addKit;
    }

    IBlock ICanBeItem.TryGetBlockContext() => item.TryGetBlockContext();


    /// <summary>
    /// The actual item which is being wrapped, in rare cases where you must access it.
    /// </summary>
    ITypedItem ICanBeItem.Item => item;

    protected ServiceKit16 Kit => kit;

    public override string ToString()
    {
        var msg = $"Custom Data Model {GetType().FullName}";
        return item == null ? $"{msg} without backing data (null)" : msg + $" for id:{Id} ({item})";
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
        => item.Get(name, protector, fallback: fallback);


    #region Auto-Replay Properties


    /// <summary>
    /// This is necessary so the object can be used in places where an IEntity is expected,
    /// like toolbars.
    /// </summary>
    public IEntity Entity => item.Entity;

    /// <inheritdoc />
    public bool ContainsKey(string name) => ((IHasKeys)item).ContainsKey(name);

    /// <inheritdoc />
    public IEnumerable<string> Keys(NoParamOrder noParamOrder = default, IEnumerable<string> only = default) => item.Keys(noParamOrder, only);

    /// <inheritdoc />
    public object Get([CallerMemberName] string name = default, NoParamOrder noParamOrder = default, bool? required = default) => item.Get(name, noParamOrder, required);

    /// <inheritdoc />
    public TValue Get<TValue>([CallerMemberName] string name = default, NoParamOrder noParamOrder = default, TValue fallback = default,
        bool? required = default) =>
        item.Get(name, noParamOrder, fallback, required);

    /// <inheritdoc />
    public bool Bool([CallerMemberName] string name = default, NoParamOrder noParamOrder = default, bool fallback = default, bool? required = default) => item.Bool(name, noParamOrder, fallback, required);

    /// <inheritdoc />
    public DateTime DateTime([CallerMemberName] string name = default, NoParamOrder noParamOrder = default, DateTime fallback = default,
        bool? required = default) =>
        item.DateTime(name, noParamOrder, fallback, required);

    /// <inheritdoc />
    public string String([CallerMemberName] string name = default, NoParamOrder noParamOrder = default, string fallback = default, bool? required = default,
        object scrubHtml = default) =>
        item.String(name, noParamOrder, fallback, required, scrubHtml);

    /// <inheritdoc />
    public int Int([CallerMemberName] string name = default, NoParamOrder noParamOrder = default, int fallback = default, bool? required = default) => item.Int(name, noParamOrder, fallback, required);

    /// <inheritdoc />
    public long Long([CallerMemberName] string name = default, NoParamOrder noParamOrder = default, long fallback = default, bool? required = default) => item.Long(name, noParamOrder, fallback, required);

    /// <inheritdoc />
    public float Float([CallerMemberName] string name = default, NoParamOrder noParamOrder = default, float fallback = default, bool? required = default) => item.Float(name, noParamOrder, fallback, required);

    /// <inheritdoc />
    public decimal Decimal([CallerMemberName] string name = default, NoParamOrder noParamOrder = default, decimal fallback = default, bool? required = default) => item.Decimal(name, noParamOrder, fallback, required);

    /// <inheritdoc />
    public double Double([CallerMemberName] string name = default, NoParamOrder noParamOrder = default, double fallback = default, bool? required = default) => item.Double(name, noParamOrder, fallback, required);

    /// <inheritdoc />
    public string Url([CallerMemberName] string name = default, NoParamOrder noParamOrder = default, string fallback = default, bool? required = default) => item.Url(name, noParamOrder, fallback, required);

    /// <inheritdoc />
    public IRawHtmlString Attribute([CallerMemberName] string name = default, NoParamOrder noParamOrder = default, string fallback = default,
        bool? required = default) =>
        item.Attribute(name, noParamOrder, fallback, required);

    /// <inheritdoc />
    public bool IsEmpty(string name, NoParamOrder noParamOrder = default) => item.IsEmpty(name, noParamOrder);

    /// <inheritdoc />
    public bool IsNotEmpty(string name, NoParamOrder noParamOrder = default) => item.IsNotEmpty(name, noParamOrder);


    /// <inheritdoc />
    public bool Equals(ITypedItem other) => item.Equals(other);

    /// <inheritdoc />
    public bool IsDemoItem => item.IsDemoItem;

    /// <inheritdoc />
    public IHtmlTag Html(string name, NoParamOrder noParamOrder = default, object container = default, bool? toolbar = default,
        object imageSettings = default, bool? required = default, bool debug = default, Func<ITweakInput<string>, ITweakInput<string>> tweak = default) =>
        item.Html(name, noParamOrder, container, toolbar, imageSettings, required, debug, tweak);

    /// <inheritdoc />
    public IResponsivePicture Picture(string name, NoParamOrder noParamOrder = default, object settings = default,
        object factor = default, object width = default, string imgAlt = default, string imgAltFallback = default,
        string imgClass = default, object imgAttributes = default, string pictureClass = default,
        object pictureAttributes = default, object toolbar = default, object recipe = default) =>
        item.Picture(name, noParamOrder, settings, factor, width, imgAlt, imgAltFallback, imgClass, imgAttributes, pictureClass, pictureAttributes, toolbar, recipe);

    /// <inheritdoc />
    public IFolder Folder([CallerMemberName] string name = default, NoParamOrder noParamOrder = default, bool? required = default) => item.Folder(name, noParamOrder, required);

    /// <inheritdoc />
    public IFile File([CallerMemberName] string name = default, NoParamOrder noParamOrder = default, bool? required = default) => item.File(name, noParamOrder, required);

    /// <inheritdoc />
    public ITypedItem Child([CallerMemberName] string name = default, NoParamOrder noParamOrder = default, bool? required = default) => item.Child(name, noParamOrder, required);

    /// <inheritdoc />
    public IEnumerable<ITypedItem> Children([CallerMemberName] string field = default, NoParamOrder noParamOrder = default, string type = default,
        bool? required = default) =>
        item.Children(field, noParamOrder, type, required);

    /// <inheritdoc />
    public ITypedItem Parent(NoParamOrder noParamOrder = default, bool? current = default, string type = default,
        string field = default) =>
        item.Parent(noParamOrder, current, type, field);

    /// <inheritdoc />
    public IEnumerable<ITypedItem> Parents(NoParamOrder noParamOrder = default, string type = default, string field = default) => item.Parents(noParamOrder, type, field);

    /// <inheritdoc />
    public bool IsPublished => item.IsPublished;

    /// <inheritdoc />
    public IPublishing Publishing => item.Publishing;

    [PrivateApi]
    [Obsolete("Not available on Custom objects, use Get(...) to access any property")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public dynamic Dyn => throw new NotSupportedException($"{nameof(ITypedItem.Dyn)} is not supported on the {nameof(ITypedStack)} by design");

    /// <inheritdoc />
    public ITypedItem Presentation => item.Presentation;

    /// <inheritdoc />
    public IMetadata Metadata => item.Metadata;

    /// <inheritdoc />
    public IField Field(string name, NoParamOrder noParamOrder = default, bool? required = default) => item.Field(name, noParamOrder, required);

    /// <inheritdoc />
    public int Id => item.Id;

    /// <inheritdoc />
    public Guid Guid => item.Guid;

    /// <inheritdoc />
    public string Title => item.Title;

    /// <inheritdoc />
    public IContentType Type => item.Type;

    #endregion

    object IHasJsonSource.JsonSource() => item?.JsonSource();

    #region New Child<T> / Children<T>

    /// <summary>
    /// EXPERIMENTAL
    /// </summary>
    /// <returns></returns>
    [PrivateApi("WIP, don't publish yet")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public T Child<T>([CallerMemberName] string name = default, NoParamOrder protector = default, bool? required = default)
        where T : class, ITypedItemWrapper16, ITypedItem, new()
        => Kit._CodeApiSvc._Cdf.AsCustom<T>(
            source: Child(name, required: required),
            kit: Kit, protector: protector, nullIfNull: true
        );

    [PrivateApi("WIP, don't publish yet")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public IEnumerable<T> Children<T>([CallerMemberName] string field = default, NoParamOrder protector = default,
        string type = default, bool? required = default)
        where T : class, ITypedItemWrapper16, ITypedItem, new()
        => Kit._CodeApiSvc._Cdf.AsCustomList<T>(
            source: Children(field: field, noParamOrder: protector, type: type, required: required),
            kit: Kit, protector: protector, nullIfNull: false
        );

    [PrivateApi("WIP, don't publish yet")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public T Parent<T>(NoParamOrder protector = default, bool? current = default, string type = default,
        string field = default)
        where T : class, ITypedItemWrapper16, ITypedItem, new()
        => Kit._CodeApiSvc._Cdf.AsCustom<T>(
            source: Parent(noParamOrder: protector, current: current, type: type, field: field),
            kit: Kit, protector: protector, nullIfNull: true
        );

    [PrivateApi("WIP, don't publish yet")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public IEnumerable<T> Parents<T>(NoParamOrder protector = default,
        string type = default, string field = default)
        where T : class, ITypedItemWrapper16, ITypedItem, new()
        => Kit._CodeApiSvc._Cdf.AsCustomList<T>(
            source: Parents(noParamOrder: protector, field: field, type: type),
            kit: Kit, protector: protector, nullIfNull: false
        );

    protected GpsCoordinates Gps(string name, NoParamOrder protector = default, bool? required = default)
        => Kit.Json.To<GpsCoordinates>(String(name, required: required, fallback: "{}"));

    #endregion
}