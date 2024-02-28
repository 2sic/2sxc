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
    /// <inheritdoc />
    void ITypedItemWrapper16.Setup(ITypedItem baseItem) => _myItem = baseItem;

    /// <inheritdoc />
    string ITypedItemWrapper16.ForContentType => GetType().Name;

    /// <summary>
    /// The actual item which is being wrapped, in rare cases where you must access it.
    ///
    /// It's only on the explicit interface, so it is not available from outside or inside, unless you cast to it.
    /// Goal is that inheriting classes don't access it to keep API surface small.
    /// </summary>
    ITypedItem ICanBeItem.Item => _myItem;
    
    /// <summary>
    /// The item - for inheriting classes to access.
    /// </summary>
    /// <remarks>
    /// This property is protected, not public, as it should only be used internally.
    /// It uses an unusual name _myItem to avoid naming conflicts with properties generated in inheriting classes.
    /// </remarks>
    // ReSharper disable once InconsistentNaming
#pragma warning disable IDE1006
    protected internal ITypedItem _myItem { get; private set; }
#pragma warning restore IDE1006

    IBlock ICanBeItem.TryGetBlockContext() => _myItem.TryGetBlockContext();

    /// <summary>
    /// Override ToString to give more information about the current object
    /// </summary>
    public override string ToString() 
        => $"Custom Data Model {GetType().FullName} " + (_myItem == null ? "without backing data (null)" : $"for id:{Id} ({_myItem})");

    #region Auto-Replay Properties


    /// <summary>
    /// This is necessary so the object can be used in places where an IEntity is expected,
    /// like toolbars.
    ///
    /// It's an explicit interface implementation, so that the object itself doesn't broadcast this.
    /// </summary>
    [PrivateApi]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    IEntity ICanBeEntity.Entity => _myItem.Entity;

    /// <inheritdoc />
    public bool ContainsKey(string name) => ((IHasKeys)_myItem).ContainsKey(name);

    /// <inheritdoc />
    public IEnumerable<string> Keys(NoParamOrder noParamOrder = default, IEnumerable<string> only = default) => _myItem.Keys(noParamOrder, only);

    /// <inheritdoc />
    public object Get(string name, NoParamOrder noParamOrder = default, bool? required = default) => _myItem.Get(name, noParamOrder, required);

    /// <inheritdoc />
    public TValue Get<TValue>(string name, NoParamOrder noParamOrder = default, TValue fallback = default,
        bool? required = default) =>
        _myItem.Get(name, noParamOrder, fallback, required);

    /// <inheritdoc />
    public bool Bool(string name, NoParamOrder noParamOrder = default, bool fallback = default, bool? required = default) => _myItem.Bool(name, noParamOrder, fallback, required);

    /// <inheritdoc />
    public DateTime DateTime(string name, NoParamOrder noParamOrder = default, DateTime fallback = default,
        bool? required = default) =>
        _myItem.DateTime(name, noParamOrder, fallback, required);

    /// <inheritdoc />
    public string String(string name, NoParamOrder noParamOrder = default, string fallback = default, bool? required = default,
        object scrubHtml = default) =>
        _myItem.String(name, noParamOrder, fallback, required, scrubHtml);

    /// <inheritdoc />
    public int Int(string name, NoParamOrder noParamOrder = default, int fallback = default, bool? required = default) => _myItem.Int(name, noParamOrder, fallback, required);

    /// <inheritdoc />
    public long Long(string name, NoParamOrder noParamOrder = default, long fallback = default, bool? required = default) => _myItem.Long(name, noParamOrder, fallback, required);

    /// <inheritdoc />
    public float Float(string name, NoParamOrder noParamOrder = default, float fallback = default, bool? required = default) => _myItem.Float(name, noParamOrder, fallback, required);

    /// <inheritdoc />
    public decimal Decimal(string name, NoParamOrder noParamOrder = default, decimal fallback = default, bool? required = default) => _myItem.Decimal(name, noParamOrder, fallback, required);

    /// <inheritdoc />
    public double Double(string name, NoParamOrder noParamOrder = default, double fallback = default, bool? required = default) => _myItem.Double(name, noParamOrder, fallback, required);

    /// <inheritdoc />
    public string Url(string name, NoParamOrder noParamOrder = default, string fallback = default, bool? required = default) => _myItem.Url(name, noParamOrder, fallback, required);

    /// <inheritdoc />
    public IRawHtmlString Attribute(string name, NoParamOrder noParamOrder = default, string fallback = default,
        bool? required = default) =>
        _myItem.Attribute(name, noParamOrder, fallback, required);

    /// <inheritdoc />
    public bool IsEmpty(string name, NoParamOrder noParamOrder = default) => _myItem.IsEmpty(name, noParamOrder);

    /// <inheritdoc />
    public bool IsNotEmpty(string name, NoParamOrder noParamOrder = default) => _myItem.IsNotEmpty(name, noParamOrder);


    /// <inheritdoc />
    public bool Equals(ITypedItem other) => _myItem.Equals(other);

    /// <inheritdoc />
    public bool IsDemoItem => _myItem.IsDemoItem;

    /// <inheritdoc />
    public IHtmlTag Html(string name, NoParamOrder noParamOrder = default, object container = default, bool? toolbar = default,
        object imageSettings = default, bool? required = default, bool debug = default, Func<ITweakInput<string>, ITweakInput<string>> tweak = default) =>
        _myItem.Html(name, noParamOrder, container, toolbar, imageSettings, required, debug, tweak);

    /// <inheritdoc />
    public IResponsivePicture Picture(string name, NoParamOrder noParamOrder = default, object settings = default,
        object factor = default, object width = default, string imgAlt = default, string imgAltFallback = default,
        string imgClass = default, object imgAttributes = default, string pictureClass = default,
        object pictureAttributes = default, object toolbar = default, object recipe = default) =>
        _myItem.Picture(name, noParamOrder, settings, factor, width, imgAlt, imgAltFallback, imgClass, imgAttributes, pictureClass, pictureAttributes, toolbar, recipe);

    /// <inheritdoc />
    public IFolder Folder(string name, NoParamOrder noParamOrder = default, bool? required = default) => _myItem.Folder(name, noParamOrder, required);

    /// <inheritdoc />
    public IFile File(string name, NoParamOrder noParamOrder = default, bool? required = default) => _myItem.File(name, noParamOrder, required);

    /// <inheritdoc />
    public ITypedItem Child(string name, NoParamOrder noParamOrder = default, bool? required = default) => _myItem.Child(name, noParamOrder, required);

    /// <inheritdoc />
    public IEnumerable<ITypedItem> Children(string field, NoParamOrder noParamOrder = default, string type = default,
        bool? required = default) =>
        _myItem.Children(field, noParamOrder, type, required);

    /// <inheritdoc />
    public ITypedItem Parent(NoParamOrder noParamOrder = default, bool? current = default, string type = default,
        string field = default) =>
        _myItem.Parent(noParamOrder, current, type, field);

    /// <inheritdoc />
    public IEnumerable<ITypedItem> Parents(NoParamOrder noParamOrder = default, string type = default, string field = default) => _myItem.Parents(noParamOrder, type, field);

    /// <inheritdoc />
    public bool IsPublished => _myItem.IsPublished;

    /// <inheritdoc />
    public IPublishing Publishing => _myItem.Publishing;

    /// <summary>
    /// Explicit, obsolete `Dyn` implementation, not to be used.
    /// </summary>
    [PrivateApi]
    [Obsolete("Not available on Custom objects, use Get(...) to access any property.")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    dynamic ITypedItem.Dyn => throw new NotSupportedException($"{nameof(ITypedItem.Dyn)} is not supported on the {nameof(ITypedStack)} by design");

    /// <inheritdoc />
    public ITypedItem Presentation => _myItem.Presentation;

    /// <inheritdoc />
    public IMetadata Metadata => _myItem.Metadata;

    /// <inheritdoc />
    public IField Field(string name, NoParamOrder noParamOrder = default, bool? required = default) => _myItem.Field(name, noParamOrder, required);

    /// <inheritdoc />
    public int Id => _myItem.Id;

    /// <inheritdoc />
    public Guid Guid => _myItem.Guid;

    /// <inheritdoc />
    public string Title => _myItem.Title;

    /// <inheritdoc />
    public IContentType Type => _myItem.Type;

    #endregion

    object IHasJsonSource.JsonSource() => _myItem?.JsonSource();

    #region New Child<T> / Children<T>

    /// <inheritdoc />
    public T Child<T>(string name, NoParamOrder protector = default, bool? required = default)
        where T : class, ITypedItemWrapper16, ITypedItem, new()
        => _myItem.Child<T>(name, protector: protector, required: required);

    /// <inheritdoc />
    public IEnumerable<T> Children<T>(string field, NoParamOrder protector = default,
        string type = default, bool? required = default)
        where T : class, ITypedItemWrapper16, ITypedItem, new()
        => _myItem.Children<T>(field: field, protector: protector, type: type, required: required);

    /// <inheritdoc />
    public T Parent<T>(NoParamOrder protector = default, bool? current = default, string type = default,
        string field = default)
        where T : class, ITypedItemWrapper16, ITypedItem, new()
        => _myItem.Parent<T>(protector: protector, current: current, type: type, field: field);

    /// <inheritdoc />
    public IEnumerable<T> Parents<T>(NoParamOrder protector = default,
        string type = default, string field = default)
        where T : class, ITypedItemWrapper16, ITypedItem, new()
        => _myItem.Parents<T>(protector: protector, type: type, field: field);

    public GpsCoordinates Gps(string name, NoParamOrder protector = default, bool? required = default)
        => _myItem.Gps(name: name, protector: protector, required: required);

    #endregion

    IPropertyLookup IHasPropLookup.PropertyLookup => _propLookup ??= ((IHasPropLookup)((ICanBeItem)this).Item).PropertyLookup;
    private IPropertyLookup _propLookup;

}