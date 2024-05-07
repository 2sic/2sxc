using ToSic.Eav.Plumbing;
using ToSic.Lib.Helpers;
using ToSic.Lib.Services;
using ToSic.Razor.Blade;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Internal;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Internal.Decorators;
using ToSic.Sxc.Edit.Toolbar;
using ToSic.Sxc.Edit.Toolbar.Internal;

namespace ToSic.Sxc.Services.CmsService;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class CmsServiceContainerHelper(
    ICodeApiService dynCodeRoot,
    IField field,
    object container,
    string classes,
    bool? toolbar,
    ILog parentLog)
    : HelperBase(parentLog, "Cms.SvcCnt")
{
    private string Classes { get; set; } = classes;


    private ServiceKit14 ServiceKit => _svcKit.Get(dynCodeRoot.GetKit<ServiceKit14>);
    private readonly GetOnce<ServiceKit14> _svcKit = new();


    public IHtmlTag Wrap(CmsProcessed result, bool defaultToolbar)
    {
        Classes = string.Join(" ", new[] { Classes, result.Classes }.Where(x => x.HasValue()));
        return Wrap(result.Contents, defaultToolbar: defaultToolbar);
    }

    public IHtmlTag Wrap(object contents, bool defaultToolbar)
    {
        var l = Log.Fn<IHtmlTag>($"{nameof(defaultToolbar)}: {defaultToolbar}");
        var tag = GetContainer(container);
        tag = tag.Wrap(contents);
        // If tag is not a real tag (no name) then it also can't have classes or toolbars; just finish and return
        if (!tag.TagName.HasValue())
            return l.Return(tag, "no wrapper tag, stop here");

        // Add classes if we can
        if (Classes.HasValue()) tag = tag.Class(Classes);

        // Add Toolbar if relevant
        if (field.Parent.IsDemoItem)
            return l.Return(tag, "demo-item, so no toolbar");

        if (field.Parent.Entity.DisableInlineEditSafe())
            return l.Return(tag, "decorator no-edit");

        var toolbar1 = toolbar ?? defaultToolbar;
        if (!toolbar1 || field == null)
            return l.Return(tag, "no toolbar added");

        l.A("Will add toolbar");
        tag = tag.Attr(ServiceKit.Toolbar.Empty().Edit(field.Parent, tweak: b => b
            .Icon(EditFieldIcon)
            .Parameters(ToolbarBuilder.BetaEditUiFieldsParamName, field.Name)
        ));
        return l.Return(tag, "added toolbar");

    }

    private const string EditFieldIcon =
        "<svg xmlns=\"http://www.w3.org/2000/svg\" height=\"48\" viewBox=\"0 96 960 960\" width=\"48\"><path d=\"M180 1044q-24 0-42-18t-18-42V384q0-24 18-42t42-18h405l-60 60H180v600h600V636l60-60v408q0 24-18 42t-42 18H180Zm300-360Zm182-352 43 42-285 284v86h85l286-286 42 42-303 304H360V634l302-302Zm171 168L662 332l100-100q17-17 42.311-17T847 233l84 85q17 18 17 42.472T930 402l-97 98Z\"/></svg>";

    private IHtmlTag GetContainer(object cont)
    {
        var l = Log.Fn<IHtmlTag>();
        return cont switch
        {
            // Already an ITag
            IHtmlTag iTagContainer => l.Return(iTagContainer, "container is pre-built RazorBlade tag"),
            string tagName when tagName.IsEmpty() => l.Return(Tag.RawHtml(), "no container, return empty tag"),
            string tagName when !tagName.Contains(" ") => l.Return(Tag.Custom(tagName), "was a tag name, created tag"),
            string tagName => throw l.Done(new ArgumentException(
                $"Must be a tag name like 'div' or a RazorBlade Html Tag object but got '{tagName}'", nameof(cont))),
            _ => l.Return(Tag.Div(), "no container, return div tag")
        };
    }

}