using System;
using System.Linq;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Helpers;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Razor.Blade;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Internal.Decorators;
using ToSic.Sxc.Edit.Toolbar;

namespace ToSic.Sxc.Services.CmsService;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class CmsServiceContainerHelper: HelperBase
{
    private readonly ICodeApiService _dynCodeRoot;
    private readonly object _container;
    private string Classes { get; set; }
    private readonly bool? _toolbar;
    private readonly IField _field;

    public CmsServiceContainerHelper(ICodeApiService dynCodeRoot,
        IField field,
        object container,
        string classes,
        bool? toolbar,
        ILog parentLog
    ) : base(parentLog,"Cms.SvcCnt")
    {
        _dynCodeRoot = dynCodeRoot;
        _field = field;
        _container = container;
        Classes = classes;
        _toolbar = toolbar;
    }


    private ServiceKit14 ServiceKit => _svcKit.Get(() => _dynCodeRoot.GetKit<ServiceKit14>());
    private readonly GetOnce<ServiceKit14> _svcKit = new();


    public IHtmlTag Wrap(CmsProcessed result, bool defaultToolbar)
    {
        Classes = string.Join(" ", new[] { Classes, result.Classes }.Where(x => x.HasValue()));
        return Wrap(result.Contents, defaultToolbar: defaultToolbar);
    }

    public IHtmlTag Wrap(object contents, bool defaultToolbar)
    {
        var l = Log.Fn<IHtmlTag>($"{nameof(defaultToolbar)}: {defaultToolbar}");
        var tag = GetContainer(_container);
        tag = tag.Wrap(contents);
        // If tag is not a real tag (no name) then it also can't have classes or toolbars; just finish and return
        if (!tag.TagName.HasValue())
            return l.Return(tag, "no wrapper tag, stop here");

        // Add classes if we can
        if (Classes.HasValue()) tag = tag.Class(Classes);

        // Add Toolbar if relevant
        if (_field.Parent.IsDemoItem)
            return l.Return(tag, "demo-item, so no toolbar");

        if (_field.Parent.Entity.DisableInlineEditSafe())
            return l.Return(tag, "decorator no-edit");

        var toolbar = _toolbar ?? defaultToolbar;
        if (!toolbar || _field == null)
            return l.Return(tag, "no toolbar added");

        l.A("Will add toolbar");
        tag = tag.Attr(ServiceKit.Toolbar.Empty().Edit(_field.Parent, tweak: b => b
            .Icon(EditFieldIcon)
            .Parameters(ToolbarBuilder.BetaEditUiFieldsParamName, _field.Name)
        ));
        return l.Return(tag, "added toolbar");

    }

    private const string EditFieldIcon =
        "<svg xmlns=\"http://www.w3.org/2000/svg\" height=\"48\" viewBox=\"0 96 960 960\" width=\"48\"><path d=\"M180 1044q-24 0-42-18t-18-42V384q0-24 18-42t42-18h405l-60 60H180v600h600V636l60-60v408q0 24-18 42t-42 18H180Zm300-360Zm182-352 43 42-285 284v86h85l286-286 42 42-303 304H360V634l302-302Zm171 168L662 332l100-100q17-17 42.311-17T847 233l84 85q17 18 17 42.472T930 402l-97 98Z\"/></svg>";

    private IHtmlTag GetContainer(object container)
    {
        var l = Log.Fn<IHtmlTag>();
        switch (container)
        {
            // Already an ITag
            case IHtmlTag iTagContainer:
                return l.Return(iTagContainer, "container is pre-built RazorBlade tag");
            case string tagName when tagName.IsEmpty():
                return l.Return(Tag.RawHtml(), "no container, return empty tag");
            case string tagName when !tagName.Contains(" "):
                return l.Return(Tag.Custom(tagName), "was a tag name, created tag");
            case string tagName:
                throw l.Done(new ArgumentException($"Must be a tag name like 'div' or a RazorBlade Html Tag object but got '{tagName}'",
                    nameof(container)));
            default:
                // Nothing to do, just return an empty tag which can be filled...
                return l.Return(Tag.Div(), "no container, return div tag");
        }
    }

}