using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ToSic.Eav.Data;
using ToSic.Lib.Coding;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Data.Internal;
using ToSic.Sxc.Images;
using ToSic.Sxc.Services.Tweaks;

namespace ToSic.Sxc.Data.Experimental;

public abstract class TypedItem(ITypedItem item): ICanBeEntity, ITypedItem
{
    public IBlock TryGetBlockContext()
    {
        return item.TryGetBlockContext();
    }

    public ITypedItem Item => item;

#pragma warning disable IDE0060
    protected TValue GetThis<TValue>(NoParamOrder protector = default, TValue fallback = default, [CallerMemberName] string name = default)
#pragma warning restore IDE0060
    {
        return item.Get(name, fallback: fallback);
    }

    /// <summary>
    /// This is necessary so the object can be used in places where an IEntity is expected,
    /// like toolbars.
    /// </summary>
    IEntity ICanBeEntity.Entity => Item.Entity;

    bool IHasKeys.ContainsKey(string name)
    {
        return ((IHasKeys)item).ContainsKey(name);
    }

    IEnumerable<string> ITyped.Keys(NoParamOrder noParamOrder, IEnumerable<string> only)
    {
        return item.Keys(noParamOrder, only);
    }

    public object Get(string name, NoParamOrder noParamOrder = default, bool? required = default)
    {
        return item.Get(name, noParamOrder, required);
    }

    public TValue Get<TValue>(string name, NoParamOrder noParamOrder = default, TValue fallback = default,
        bool? required = default)
    {
        return item.Get(name, noParamOrder, fallback, required);
    }

    public bool Bool(string name, NoParamOrder noParamOrder = default, bool fallback = default, bool? required = default)
    {
        return item.Bool(name, noParamOrder, fallback, required);
    }

    public DateTime DateTime(string name, NoParamOrder noParamOrder = default, DateTime fallback = default,
        bool? required = default)
    {
        return item.DateTime(name, noParamOrder, fallback, required);
    }

    public string String(string name, NoParamOrder noParamOrder = default, string fallback = default, bool? required = default,
        object scrubHtml = default)
    {
        return item.String(name, noParamOrder, fallback, required, scrubHtml);
    }

    public int Int(string name, NoParamOrder noParamOrder = default, int fallback = default, bool? required = default)
    {
        return item.Int(name, noParamOrder, fallback, required);
    }

    public long Long(string name, NoParamOrder noParamOrder = default, long fallback = default, bool? required = default)
    {
        return item.Long(name, noParamOrder, fallback, required);
    }

    public float Float(string name, NoParamOrder noParamOrder = default, float fallback = default, bool? required = default)
    {
        return item.Float(name, noParamOrder, fallback, required);
    }

    public decimal Decimal(string name, NoParamOrder noParamOrder = default, decimal fallback = default, bool? required = default)
    {
        return item.Decimal(name, noParamOrder, fallback, required);
    }

    public double Double(string name, NoParamOrder noParamOrder = default, double fallback = default, bool? required = default)
    {
        return item.Double(name, noParamOrder, fallback, required);
    }

    public string Url(string name, NoParamOrder noParamOrder = default, string fallback = default, bool? required = default)
    {
        return item.Url(name, noParamOrder, fallback, required);
    }

    public IRawHtmlString Attribute(string name, NoParamOrder noParamOrder = default, string fallback = default,
        bool? required = default)
    {
        return item.Attribute(name, noParamOrder, fallback, required);
    }

    bool ITyped.IsEmpty(string name, NoParamOrder noParamOrder)
    {
        return item.IsEmpty(name, noParamOrder);
    }

    bool ITyped.IsNotEmpty(string name, NoParamOrder noParamOrder)
    {
        return item.IsNotEmpty(name, noParamOrder);
    }

    bool ITyped.ContainsKey(string name)
    {
        return item.ContainsKey(name);
    }

    IEnumerable<string> IHasKeys.Keys(NoParamOrder noParamOrder, IEnumerable<string> only)
    {
        return ((IHasKeys)item).Keys(noParamOrder, only);
    }

    bool IHasKeys.IsEmpty(string name, NoParamOrder noParamOrder)
    {
        return ((IHasKeys)item).IsEmpty(name, noParamOrder);
    }

    bool IHasKeys.IsNotEmpty(string name, NoParamOrder noParamOrder)
    {
        return ((IHasKeys)item).IsNotEmpty(name, noParamOrder);
    }

    public bool Equals(ITypedItem other)
    {
        return item.Equals(other);
    }

    public bool IsDemoItem => item.IsDemoItem;

    public IHtmlTag Html(string name, NoParamOrder noParamOrder = default, object container = default, bool? toolbar = default,
        object imageSettings = default, bool? required = default, bool debug = default, Func<ITweakInput<string>, ITweakInput<string>> tweak = default)
    {
        return item.Html(name, noParamOrder, container, toolbar, imageSettings, required, debug, tweak);
    }

    public IResponsivePicture Picture(string name, NoParamOrder noParamOrder = default, object settings = default,
        object factor = default, object width = default, string imgAlt = default, string imgAltFallback = default,
        string imgClass = default, object imgAttributes = default, string pictureClass = default,
        object pictureAttributes = default, object toolbar = default, object recipe = default)
    {
        return item.Picture(name, noParamOrder, settings, factor, width, imgAlt, imgAltFallback, imgClass, imgAttributes, pictureClass, pictureAttributes, toolbar, recipe);
    }

    public IFolder Folder(string name, NoParamOrder noParamOrder = default, bool? required = default)
    {
        return item.Folder(name, noParamOrder, required);
    }

    public IFile File(string name, NoParamOrder noParamOrder = default, bool? required = default)
    {
        return item.File(name, noParamOrder, required);
    }

    public ITypedItem Child(string name, NoParamOrder noParamOrder = default, bool? required = default)
    {
        return item.Child(name, noParamOrder, required);
    }

    public IEnumerable<ITypedItem> Children(string field = default, NoParamOrder noParamOrder = default, string type = default,
        bool? required = default)
    {
        return item.Children(field, noParamOrder, type, required);
    }

    public ITypedItem Parent(NoParamOrder noParamOrder = default, bool? current = default, string type = default,
        string field = default)
    {
        return item.Parent(noParamOrder, current, type, field);
    }

    public IEnumerable<ITypedItem> Parents(NoParamOrder noParamOrder = default, string type = default, string field = default)
    {
        return item.Parents(noParamOrder, type, field);
    }

    public bool IsPublished => item.IsPublished;

    public IPublishing Publishing => item.Publishing;

    public dynamic Dyn => item.Dyn;

    public ITypedItem Presentation => item.Presentation;

    public IMetadata Metadata => item.Metadata;

    public IField Field(string name, NoParamOrder noParamOrder = default, bool? required = default)
    {
        return item.Field(name, noParamOrder, required);
    }

    public int Id => item.Id;

    public Guid Guid => item.Guid;

    public string Title => item.Title;

    public IContentType Type => item.Type;
}