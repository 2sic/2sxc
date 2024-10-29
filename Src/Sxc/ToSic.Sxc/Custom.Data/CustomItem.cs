using System.Text.Json.Serialization;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Cms.Data;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Internal;
using ToSic.Sxc.Images;
using ToSic.Sxc.Services.Tweaks;

// ReSharper disable once CheckNamespace
namespace Custom.Data;

/// <summary>
/// Base class for custom data objects, which are typed and can be used in Razor Components.
///
/// It is used by 2sxc Copilot when generating base classes for custom data objects.
/// </summary>
/// <remarks>
/// - Released in v17.06
/// - It's not abstract, even if the most common case is to inherit, as there are cases where you want to use it directly.
/// </remarks>
[PublicApi]
public partial class CustomItem: ITypedItem, ITypedItemWrapper16, IHasPropLookup
{
    #region Explicit Interfaces for internal use - Setup, etc.

    /// <inheritdoc />
    void ITypedItemWrapper16.Setup(ITypedItem baseItem) => _item = baseItem;

    /// <inheritdoc />
    string ITypedItemWrapper16.ForContentType => GetType().Name;

    /// <summary>
    /// The actual item which is being wrapped, in rare cases where you must access it.
    ///
    /// It's only on the explicit interface, so it is not available from outside or inside, unless you cast to it.
    /// Goal is that inheriting classes don't access it to keep API surface small.
    /// </summary>
    ITypedItem ICanBeItem.Item => _item;

    /// <summary>
    /// This is necessary so the object can be used in places where an IEntity is expected,
    /// like toolbars.
    ///
    /// It's an explicit interface implementation, so that the object itself doesn't broadcast this.
    /// </summary>
    [PrivateApi]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    IEntity ICanBeEntity.Entity => _item.Entity;

    IBlock ICanBeItem.TryGetBlockContext() => _item.TryGetBlockContext();

    IPropertyLookup IHasPropLookup.PropertyLookup => _propLookup ??= ((IHasPropLookup)((ICanBeItem)this).Item).PropertyLookup;
    private IPropertyLookup _propLookup;

    #endregion

    /// <summary>
    /// The item - for inheriting classes to access.
    /// </summary>
    /// <remarks>
    /// This property is protected, not public, as it should only be used internally.
    /// It uses an unusual name _item to avoid naming conflicts with properties generated in inheriting classes.
    /// </remarks>
    // ReSharper disable once InconsistentNaming
#pragma warning disable IDE1006
    protected internal ITypedItem _item { get; private set; }
#pragma warning restore IDE1006

    /// <summary>
    /// Override ToString to give more information about the current object
    /// </summary>
    public override string ToString() 
        => $"{nameof(CustomItem)} Data Model {GetType().FullName} " + (_item == null ? "without backing data (null)" : $"for id:{Id} ({_item})");


    #region Keys and Empty-Checks

    /// <inheritdoc cref="IHasKeys.ContainsKey"/>
    public bool ContainsKey(string name) => ((IHasKeys)_item).ContainsKey(name);

    /// <inheritdoc cref="IHasKeys.Keys"/>
    public IEnumerable<string> Keys(NoParamOrder noParamOrder = default, IEnumerable<string> only = default) => _item.Keys(noParamOrder, only);

    /// <inheritdoc cref="IHasKeys.IsEmpty"/>
    public bool IsEmpty(string name, NoParamOrder noParamOrder = default, string language = default)
        => _item.IsEmpty(name, noParamOrder, language: language);

    /// <inheritdoc cref="IHasKeys.IsNotEmpty"/>
    public bool IsNotEmpty(string name, NoParamOrder noParamOrder = default, string language = default)
        => _item.IsNotEmpty(name, noParamOrder, language: language);

    #endregion


    #region Basic Get

    /// <inheritdoc />
    public object Get(string name, NoParamOrder noParamOrder = default, bool? required = default, string language = default)
        => _item.Get(name: name, noParamOrder: noParamOrder, required: required, language: language);

    /// <inheritdoc />
    public TValue Get<TValue>(string name, NoParamOrder noParamOrder = default, TValue fallback = default, bool? required = default, string language = default)
        => _item.Get(name: name, noParamOrder: noParamOrder, fallback: fallback, required: required, language: language);

    #endregion

    #region Typed Get

    /// <inheritdoc />
    public bool Bool(string name, NoParamOrder noParamOrder = default, bool fallback = default, bool? required = default) => _item.Bool(name, noParamOrder, fallback, required);

    /// <inheritdoc />
    public DateTime DateTime(string name, NoParamOrder noParamOrder = default, DateTime fallback = default,
        bool? required = default) =>
        _item.DateTime(name, noParamOrder, fallback, required);

    /// <inheritdoc />
    public string String(string name, NoParamOrder noParamOrder = default, string fallback = default, bool? required = default,
        object scrubHtml = default) =>
        _item.String(name, noParamOrder, fallback, required, scrubHtml);

    /// <inheritdoc />
    public int Int(string name, NoParamOrder noParamOrder = default, int fallback = default, bool? required = default) => _item.Int(name, noParamOrder, fallback, required);

    /// <inheritdoc />
    public long Long(string name, NoParamOrder noParamOrder = default, long fallback = default, bool? required = default) => _item.Long(name, noParamOrder, fallback, required);

    /// <inheritdoc />
    public float Float(string name, NoParamOrder noParamOrder = default, float fallback = default, bool? required = default) => _item.Float(name, noParamOrder, fallback, required);

    /// <inheritdoc />
    public decimal Decimal(string name, NoParamOrder noParamOrder = default, decimal fallback = default, bool? required = default) => _item.Decimal(name, noParamOrder, fallback, required);

    /// <inheritdoc />
    public double Double(string name, NoParamOrder noParamOrder = default, double fallback = default, bool? required = default) => _item.Double(name, noParamOrder, fallback, required);

    /// <inheritdoc />
    public string Url(string name, NoParamOrder noParamOrder = default, string fallback = default, bool? required = default) => _item.Url(name, noParamOrder, fallback, required);

    #endregion


    /// <inheritdoc />
    [JsonIgnore] // prevent serialization as it's not a normal property
    public bool IsDemoItem => _item.IsDemoItem;

    #region Advanced Get Methods: Attribute, Html, File, Folder etc.

    /// <inheritdoc />
    public IRawHtmlString Attribute(string name, NoParamOrder noParamOrder = default, string fallback = default,
        bool? required = default) =>
        _item.Attribute(name, noParamOrder, fallback, required);

    /// <inheritdoc />
    public IHtmlTag Html(string name, NoParamOrder noParamOrder = default, object container = default, bool? toolbar = default,
        object imageSettings = default, bool? required = default, bool debug = default, Func<ITweakInput<string>, ITweakInput<string>> tweak = default) =>
        _item.Html(name, noParamOrder, container, toolbar, imageSettings, required, debug, tweak);

    /// <inheritdoc />
    public IResponsivePicture Picture(string name, NoParamOrder noParamOrder = default,
        Func<ITweakMedia, ITweakMedia> tweak = default,
        object settings = default,
        object factor = default, object width = default, string imgAlt = default, string imgAltFallback = default,
        string imgClass = default, object imgAttributes = default, string pictureClass = default,
        object pictureAttributes = default, object toolbar = default, object recipe = default) =>
        _item.Picture(name, noParamOrder, tweak, settings, factor, width, imgAlt, imgAltFallback, imgClass, imgAttributes, pictureClass, pictureAttributes, toolbar, recipe);

    /// <inheritdoc />
    public IResponsiveImage Img(string name, NoParamOrder noParamOrder = default, Func<ITweakMedia, ITweakMedia> tweak = default, object settings = default, object factor = default, object width = default,
        string imgAlt = default, string imgAltFallback = default, string imgClass = default, object imgAttributes = default, object toolbar = default, object recipe = default) =>
        _item.Img(name, noParamOrder, tweak, settings, factor, width, imgAlt, imgAltFallback, imgClass, imgAttributes, toolbar, recipe);

    /// <inheritdoc />
    public IFolder Folder(string name, NoParamOrder noParamOrder = default, bool? required = default)
        => _item.Folder(name, noParamOrder, required);

    /// <inheritdoc />
    public IFile File(string name, NoParamOrder noParamOrder = default, bool? required = default)
        => _item.File(name, noParamOrder, required);

    #endregion

    #region Children and Parents

    /// <inheritdoc />
    public ITypedItem Child(string name, NoParamOrder noParamOrder = default, bool? required = default)
        => _item.Child(name, noParamOrder, required);

    /// <inheritdoc />
    public IEnumerable<ITypedItem> Children(string field, NoParamOrder noParamOrder = default, string type = default, bool? required = default)
        => _item.Children(field, noParamOrder, type, required);

    /// <inheritdoc />
    public ITypedItem Parent(NoParamOrder noParamOrder = default, bool? current = default, string type = default, string field = default)
        => _item.Parent(noParamOrder, current, type, field);

    /// <inheritdoc />
    public IEnumerable<ITypedItem> Parents(NoParamOrder noParamOrder = default, string type = default, string field = default)
        => _item.Parents(noParamOrder, type, field);

    #endregion

    /// <inheritdoc />
    [JsonIgnore] // prevent serialization as it's not a normal property
    public bool IsPublished => _item.IsPublished;

    /// <inheritdoc />
    [JsonIgnore] // prevent serialization as it's not a normal property
    public IPublishing Publishing => _item.Publishing;

    /// <summary>
    /// Explicit, obsolete `Dyn` implementation, not to be used.
    /// </summary>
    [PrivateApi]
    [Obsolete("Not available on Custom objects, use Get(...) to access any property.")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    [JsonIgnore] // prevent serialization as it's not a normal property
    dynamic ITypedItem.Dyn => throw new NotSupportedException($"{nameof(ITypedItem.Dyn)} is not supported on the {nameof(CustomItem)} by design");

    /// <inheritdoc />
    [JsonIgnore] // prevent serialization as it's not a normal property
    public ITypedItem Presentation => _item.Presentation;

    /// <inheritdoc />
    [JsonIgnore] // prevent serialization as it's not a normal property
    public IMetadata Metadata => _item.Metadata;

    /// <inheritdoc />
    public IField Field(string name, NoParamOrder noParamOrder = default, bool? required = default) => _item.Field(name, noParamOrder, required);


    #region Core Data: Id, Guid, Title, Type

    /// <inheritdoc />
    [JsonPropertyOrder(-100)]
    public int Id => _item.Id;

    /// <inheritdoc />
    [JsonPropertyOrder(-99)]
    public Guid Guid => _item.Guid;

    /// <inheritdoc />
    [JsonIgnore] // prevent serialization as it maps to a property which could be different; better let the inheriting class define it
    public string Title => _item.Title;

    /// <inheritdoc />
    [JsonIgnore] // prevent serialization as it's not a normal property
    public IContentType Type => _item.Type;

    #endregion


    #region New Child<T> / Children<T>

    /// <inheritdoc />
    public T Child<T>(string name, NoParamOrder protector = default, bool? required = default)
        where T : class, ITypedItemWrapper16, ITypedItem, new()
        => _item.Child<T>(name, protector: protector, required: required);

    /// <inheritdoc />
    public IEnumerable<T> Children<T>(string field, NoParamOrder protector = default,
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
        => _item.Parents<T>(protector: protector, type: type ?? typeof(T).Name, field: field);

    /// <inheritdoc />
    public GpsCoordinates Gps(string name, NoParamOrder protector = default, bool? required = default)
        => _item.Gps(name: name, protector: protector, required: required);

    #endregion

    #region As...

    /// <summary>
    /// Convert an Entity or TypedItem into a strongly typed object.
    /// Typically, the type will be from your `AppCode.Data`.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="item"></param>
    /// <returns></returns>
    /// <remarks>
    /// BETA in v17.03
    /// </remarks>
    protected T As<T>(ITypedItem item)
        where T : class, ITypedItemWrapper16, ITypedItem, new()
        => CodeDataFactory.AsCustomFromItem<T>(item);

    /// <summary>
    /// Convert a list of Entities or TypedItems into a strongly typed list.
    /// Typically, the type will be from your `AppCode.Data`.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <param name="protector"></param>
    /// <param name="nullIfNull"></param>
    /// <returns></returns>
    /// <remarks>
    /// BETA in v17.03
    /// </remarks>
    protected IEnumerable<T> AsList<T>(IEnumerable<ITypedItem> source, NoParamOrder protector = default, bool nullIfNull = false)
        where T : class, ITypedItemWrapper16, ITypedItem, new()
        => (source ?? (nullIfNull ? null : []))?.Select(CodeDataFactory.AsCustomFromItem<T>).ToList();

    #endregion

    /// <summary>
    /// Get by name should never throw an error, as it's used to get null if not found.
    /// </summary>
    object ICanGetByName.Get(string name) => (this as ITypedItem).Get(name, required: false);

}