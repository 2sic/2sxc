using System.Runtime.CompilerServices;
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
/// Base class for custom data objects, which are typed and can be used in Razor Components.
///
/// It is used by 2sxc Copilot when generating base classes for custom data objects.
/// </summary>

// TODO: @2dm
//[JsonConverter(typeof(DynamicJsonConverter))]

public abstract class Item16: ITypedItem, ITypedItemWrapper16, IHasJsonSource, IHasPropLookup
{

    void ITypedItemWrapper16.Setup(ITypedItem baseItem, ServiceKit16 addKit)
    {
        Item = baseItem;
        Kit = addKit;
    }

    string ITypedItemWrapper16.ForContentType => GetType().Name;

    /// <summary>
    /// The actual item which is being wrapped, in rare cases where you must access it.
    ///
    /// It's only on the explicit interface, so it is not available from outside or inside, unless you cast to it.
    /// Goal is that inheriting classes don't access it to keep API surface small.
    /// </summary>
    ITypedItem ICanBeItem.Item => Item;
    
    /// <summary>
    /// The item - for inheriting classes to access.
    /// </summary>
    /// <remarks>
    /// This property is protected, not public, as it should only be used internally.
    /// </remarks>
    protected ITypedItem Item;

    IBlock ICanBeItem.TryGetBlockContext() => Item.TryGetBlockContext();

    /// <summary>
    /// Kit - private, so not available to inheriting classes for now; keep API surface small.
    /// 2024-02-22 2dm - ATM used in Content, so must consider how to proceed - either remove from Content, or make it available here
    /// </summary>
    protected ServiceKit16 Kit { get; set; }

    /// <summary>
    /// Override ToString to give more information about the current object
    /// </summary>
    public override string ToString()
    {
        var msg = $"Custom Data Model {GetType().FullName}";
        return Item == null ? $"{msg} without backing data (null)" : msg + $" for id:{Id} ({Item})";
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
        => Item.Get(name, protector, fallback: fallback);

    #region Auto-Replay Properties


    /// <summary>
    /// This is necessary so the object can be used in places where an IEntity is expected,
    /// like toolbars.
    ///
    /// It's an explicit interface implementation, so that the object itself doesn't broadcast this.
    /// </summary>
    [PrivateApi]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    IEntity ICanBeEntity.Entity => Item.Entity;

    /// <inheritdoc />
    public bool ContainsKey(string name) => ((IHasKeys)Item).ContainsKey(name);

    /// <inheritdoc />
    public IEnumerable<string> Keys(NoParamOrder noParamOrder = default, IEnumerable<string> only = default) => Item.Keys(noParamOrder, only);

    /// <inheritdoc />
    public object Get([CallerMemberName] string name = default, NoParamOrder noParamOrder = default, bool? required = default) => Item.Get(name, noParamOrder, required);

    /// <inheritdoc />
    public TValue Get<TValue>([CallerMemberName] string name = default, NoParamOrder noParamOrder = default, TValue fallback = default,
        bool? required = default) =>
        Item.Get(name, noParamOrder, fallback, required);

    /// <inheritdoc />
    public bool Bool([CallerMemberName] string name = default, NoParamOrder noParamOrder = default, bool fallback = default, bool? required = default) => Item.Bool(name, noParamOrder, fallback, required);

    /// <inheritdoc />
    public DateTime DateTime([CallerMemberName] string name = default, NoParamOrder noParamOrder = default, DateTime fallback = default,
        bool? required = default) =>
        Item.DateTime(name, noParamOrder, fallback, required);

    /// <inheritdoc />
    public string String([CallerMemberName] string name = default, NoParamOrder noParamOrder = default, string fallback = default, bool? required = default,
        object scrubHtml = default) =>
        Item.String(name, noParamOrder, fallback, required, scrubHtml);

    /// <inheritdoc />
    public int Int([CallerMemberName] string name = default, NoParamOrder noParamOrder = default, int fallback = default, bool? required = default) => Item.Int(name, noParamOrder, fallback, required);

    /// <inheritdoc />
    public long Long([CallerMemberName] string name = default, NoParamOrder noParamOrder = default, long fallback = default, bool? required = default) => Item.Long(name, noParamOrder, fallback, required);

    /// <inheritdoc />
    public float Float([CallerMemberName] string name = default, NoParamOrder noParamOrder = default, float fallback = default, bool? required = default) => Item.Float(name, noParamOrder, fallback, required);

    /// <inheritdoc />
    public decimal Decimal([CallerMemberName] string name = default, NoParamOrder noParamOrder = default, decimal fallback = default, bool? required = default) => Item.Decimal(name, noParamOrder, fallback, required);

    /// <inheritdoc />
    public double Double([CallerMemberName] string name = default, NoParamOrder noParamOrder = default, double fallback = default, bool? required = default) => Item.Double(name, noParamOrder, fallback, required);

    /// <inheritdoc />
    public string Url([CallerMemberName] string name = default, NoParamOrder noParamOrder = default, string fallback = default, bool? required = default) => Item.Url(name, noParamOrder, fallback, required);

    /// <inheritdoc />
    public IRawHtmlString Attribute([CallerMemberName] string name = default, NoParamOrder noParamOrder = default, string fallback = default,
        bool? required = default) =>
        Item.Attribute(name, noParamOrder, fallback, required);

    /// <inheritdoc />
    public bool IsEmpty(string name, NoParamOrder noParamOrder = default) => Item.IsEmpty(name, noParamOrder);

    /// <inheritdoc />
    public bool IsNotEmpty(string name, NoParamOrder noParamOrder = default) => Item.IsNotEmpty(name, noParamOrder);


    /// <inheritdoc />
    public bool Equals(ITypedItem other) => Item.Equals(other);

    /// <inheritdoc />
    public bool IsDemoItem => Item.IsDemoItem;

    /// <inheritdoc />
    public IHtmlTag Html(string name, NoParamOrder noParamOrder = default, object container = default, bool? toolbar = default,
        object imageSettings = default, bool? required = default, bool debug = default, Func<ITweakInput<string>, ITweakInput<string>> tweak = default) =>
        Item.Html(name, noParamOrder, container, toolbar, imageSettings, required, debug, tweak);

    /// <inheritdoc />
    public IResponsivePicture Picture(string name, NoParamOrder noParamOrder = default, object settings = default,
        object factor = default, object width = default, string imgAlt = default, string imgAltFallback = default,
        string imgClass = default, object imgAttributes = default, string pictureClass = default,
        object pictureAttributes = default, object toolbar = default, object recipe = default) =>
        Item.Picture(name, noParamOrder, settings, factor, width, imgAlt, imgAltFallback, imgClass, imgAttributes, pictureClass, pictureAttributes, toolbar, recipe);

    /// <inheritdoc />
    public IFolder Folder([CallerMemberName] string name = default, NoParamOrder noParamOrder = default, bool? required = default) => Item.Folder(name, noParamOrder, required);

    /// <inheritdoc />
    public IFile File([CallerMemberName] string name = default, NoParamOrder noParamOrder = default, bool? required = default) => Item.File(name, noParamOrder, required);

    /// <inheritdoc />
    public ITypedItem Child([CallerMemberName] string name = default, NoParamOrder noParamOrder = default, bool? required = default) => Item.Child(name, noParamOrder, required);

    /// <inheritdoc />
    public IEnumerable<ITypedItem> Children([CallerMemberName] string field = default, NoParamOrder noParamOrder = default, string type = default,
        bool? required = default) =>
        Item.Children(field, noParamOrder, type, required);

    /// <inheritdoc />
    public ITypedItem Parent(NoParamOrder noParamOrder = default, bool? current = default, string type = default,
        string field = default) =>
        Item.Parent(noParamOrder, current, type, field);

    /// <inheritdoc />
    public IEnumerable<ITypedItem> Parents(NoParamOrder noParamOrder = default, string type = default, string field = default) => Item.Parents(noParamOrder, type, field);

    /// <inheritdoc />
    public bool IsPublished => Item.IsPublished;

    /// <inheritdoc />
    public IPublishing Publishing => Item.Publishing;

    /// <summary>
    /// Explicit, obsolete `Dyn` implementation, not to be used.
    /// </summary>
    [PrivateApi]
    [Obsolete("Not available on Custom objects, use Get(...) to access any property.")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    dynamic ITypedItem.Dyn => throw new NotSupportedException($"{nameof(ITypedItem.Dyn)} is not supported on the {nameof(ITypedStack)} by design");

    /// <inheritdoc />
    public ITypedItem Presentation => Item.Presentation;

    /// <inheritdoc />
    public IMetadata Metadata => Item.Metadata;

    /// <inheritdoc />
    public IField Field(string name, NoParamOrder noParamOrder = default, bool? required = default) => Item.Field(name, noParamOrder, required);

    /// <inheritdoc />
    public int Id => Item.Id;

    /// <inheritdoc />
    public Guid Guid => Item.Guid;

    /// <inheritdoc />
    public string Title => Item.Title;

    /// <inheritdoc />
    public IContentType Type => Item.Type;

    #endregion

    object IHasJsonSource.JsonSource() => Item?.JsonSource();

    #region New Child<T> / Children<T>

    /// <inheritdoc />
    public T Child<T>([CallerMemberName] string name = default, NoParamOrder protector = default,
        bool? required = default)
        where T : class, ITypedItemWrapper16, ITypedItem, new()
        => Item.Child<T>(name, protector: protector, required: required);

    /// <inheritdoc />
    public IEnumerable<T> Children<T>([CallerMemberName] string field = default, NoParamOrder protector = default,
        string type = default, bool? required = default)
        where T : class, ITypedItemWrapper16, ITypedItem, new()
        => Item.Children<T>(field: field, protector: protector, type: type, required: required);

    /// <inheritdoc />
    public T Parent<T>(NoParamOrder protector = default, bool? current = default, string type = default,
        string field = default)
        where T : class, ITypedItemWrapper16, ITypedItem, new()
        => Item.Parent<T>(protector: protector, current: current, type: type, field: field);

    /// <inheritdoc />
    public IEnumerable<T> Parents<T>(NoParamOrder protector = default,
        string type = default, string field = default)
        where T : class, ITypedItemWrapper16, ITypedItem, new()
        => Item.Parents<T>(protector: protector, type: type, field: field);

    public GpsCoordinates Gps(string name, NoParamOrder protector = default, bool? required = default)
        => Item.Gps(name: name, protector: protector, required: required);

    #endregion

    IPropertyLookup IHasPropLookup.PropertyLookup => _propLookup ??= ((IHasPropLookup)((ICanBeItem)this).Item).PropertyLookup;
    private IPropertyLookup _propLookup;

}