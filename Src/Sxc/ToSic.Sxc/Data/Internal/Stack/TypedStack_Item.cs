using System.Text.Json.Serialization;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Plumbing;
using ToSic.Razor.Blade;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Cms.Data;
using ToSic.Sxc.Data.Internal.Typed;
using ToSic.Sxc.Images;
using ToSic.Sxc.Services.Tweaks;
using static ToSic.Sxc.Data.Internal.Typed.TypedHelpers;

namespace ToSic.Sxc.Data.Internal.Stack;

internal partial class TypedStack: ITypedItem
{
    private const string NotImplementedError = "This is a stack. This method/property is not clearly defined on a stack of objects.";

    private const string ParentNotImplemented = "This is a stack. There are no parents as nothing can ever point to a stack directly, only to specific objects inside it.";

    IEntity ICanBeEntity.Entity => throw new NotImplementedException(NotImplementedError);

    IBlock ICanBeItem.TryGetBlockContext() => Cdf?.BlockOrNull;

    ITypedItem ICanBeItem.Item => this;

    bool IEquatable<ITypedItem>.Equals(ITypedItem other) => ReferenceEquals(this, other);

    bool ITypedItem.IsDemoItem => false;

    IHtmlTag ITypedItem.Html(string name, NoParamOrder noParamOrder, object container, bool? toolbar,
        object imageSettings, bool? required, bool debug, Func<ITweakInput<string>, ITweakInput<string>> tweak)
        => TypedItemHelpers.Html(Cdf, this, name, noParamOrder, container, toolbar, imageSettings, required, debug, tweak);

    IResponsivePicture ITypedItem.Picture(string name, NoParamOrder noParamOrder,
        Func<ITweakMedia, ITweakMedia> tweak,
        object settings,
        object factor, object width, string imgAlt, string imgAltFallback,
        string imgClass, object imgAttributes, string pictureClass,
        object pictureAttributes, object toolbar, object recipe
    ) => TypedItemHelpers.Picture(cdf: Cdf, item: this, name: name, noParamOrder: noParamOrder, tweak: tweak, settings: settings,
        factor: factor, width: width, imgAlt: imgAlt, imgAltFallback: imgAltFallback,
        imgClass: imgClass, imgAttributes: imgAttributes, pictureClass: pictureClass,
        pictureAttributes: pictureAttributes,
        toolbar: toolbar, recipe: recipe);

    IResponsiveImage ITypedItem.Img(string name, NoParamOrder noParamOrder, Func<ITweakMedia, ITweakMedia> tweak, object settings, object factor,
        object width, string imgAlt, string imgAltFallback, string imgClass, object imgAttributes, object toolbar, object recipe
    ) => TypedItemHelpers.Img(cdf: Cdf, item: this, name: name, noParamOrder: noParamOrder, tweak: tweak, settings: settings,
        factor: factor, width: width, imgAlt: imgAlt, imgAltFallback: imgAltFallback,
        imgClass: imgClass, imgAttributes: imgAttributes,
        toolbar: toolbar, recipe: recipe);


    GpsCoordinates ITypedItem.Gps(string name, NoParamOrder protector, bool? required)
        => GpsCoordinates.FromJson(((ITypedItem)this).String(name, required: required));

    #region ADAM

    IField ITypedItem.Field(string name, NoParamOrder noParamOrder, bool? required)
    {
        // Try to find the object which has that field with a valid value etc.
        var sourceItem = FindSubItemHavingField(name);
        return sourceItem?.Field(name, required: required)
               ?? (IsErrStrict(false, required, _helper.PropsRequired)
                   ? throw ErrStrict(name)
                   : null);
    }

    /// <summary>
    /// Find the first source having the specified field with a real value...
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private ITypedItem FindSubItemHavingField(string name)
    {
        // Try to find the object which has that field with a valid value etc.
        var logOrNull = _helper.LogOrNull.SubLogOrNull("Stk.Field", Debug);
        var specs = new PropReqSpecs(name, Cdf.Dimensions, true, logOrNull);
        var path = new PropertyLookupPath().Add("DynEntStart", name);

        var findResult = _stackPropLookup.FindPropertyInternal(specs, path);
        if (findResult == null || findResult.ValueType == ValueTypesWithState.NotFound)
            return null;

        var sourceItem = findResult.Source as ITypedItem
                         ?? (findResult.Source as ICanBeItem)?.Item
                         ?? (findResult.Source as ICanBeEntity).NullOrGetWith(e => Cdf.AsItem(e));

        return sourceItem;
    }

    IFolder ITypedItem.Folder(string name, NoParamOrder noParamOrder, bool? required)
    {
        // Try to find the object which has that field with a valid value etc.
        var sourceItem = FindSubItemHavingField(name);
        return sourceItem?.Folder(name, required: required)
               ?? (IsErrStrict(false, required, _helper.PropsRequired)
                   ? throw ErrStrict(name)
                   : null);
    }

    IFile ITypedItem.File(string name, NoParamOrder noParamOrder, bool? required)
    {
        // Try to find the object which has that field with a valid value etc.
        var sourceItem = FindSubItemHavingField(name);
        return sourceItem?.File(name, required: required)
               ?? (IsErrStrict(false, required, _helper.PropsRequired)
                   ? throw ErrStrict(name)
                   : null);
    }

    #endregion

    //ITypedItem ITypedItem.Child(string name, NoParamOrder noParamOrder, bool? required)
    //    => (this as ITypedStack).Child(name, noParamOrder, required);

    //IEnumerable<ITypedItem> ITypedItem.Children(string field, NoParamOrder noParamOrder, string type, bool? required)
    //    => (this as ITypedStack).Children(field, noParamOrder, type, required);

    T ITypedItem.Child<T>(string name, NoParamOrder protector, bool? required)
        => Cdf.AsCustom<T>(
            source: ((ITypedItem)this).Child(name, required: required), protector: protector, mock: false
        );

    IEnumerable<T> ITypedItem.Children<T>(string field, NoParamOrder protector, string type, bool? required)
        => Cdf.AsCustomList<T>(
            source: ((ITypedItem)this).Children(field: field, noParamOrder: protector, type: type, required: required),
            protector: protector,
            nullIfNull: false
        );

    #region Not implemented: Parents, Publishing, Dyn, Presentation, Metadata

    ITypedItem ITypedItem.Parent(NoParamOrder noParamOrder, bool? current, string type, string field)
        => throw new NotImplementedException(ParentNotImplemented);

    IEnumerable<ITypedItem> ITypedItem.Parents(NoParamOrder noParamOrder, string type, string field)
        => throw new NotImplementedException(ParentNotImplemented);

    T ITypedItem.Parent<T>(NoParamOrder protector, bool? current, string type, string field)
        => throw new NotImplementedException(ParentNotImplemented);

    IEnumerable<T> ITypedItem.Parents<T>(NoParamOrder protector, string type, string field)
        => throw new NotImplementedException(ParentNotImplemented);

    bool ITypedItem.IsPublished => throw new NotImplementedException(NotImplementedError);

    IPublishing ITypedItem.Publishing => throw new NotImplementedException(NotImplementedError);

    [JsonIgnore] // prevent serialization as it's not a normal property
    dynamic ITypedItem.Dyn => throw new NotImplementedException($"{nameof(ITypedItem.Dyn)} is not supported on the {nameof(TypedStack)} by design");

    [JsonIgnore] // prevent serialization as it's not a normal property
    ITypedItem ITypedItem.Presentation => throw new NotImplementedException(NotImplementedError);

    [JsonIgnore] // prevent serialization as it's not a normal property
    IMetadata ITypedItem.Metadata => throw new NotImplementedException(NotImplementedError);

    #endregion

    int ITypedItem.Id => 0;

    Guid ITypedItem.Guid => Guid.Empty;

    string ITypedItem.Title => "";

    IContentType ITypedItem.Type => throw new NotImplementedException();
}