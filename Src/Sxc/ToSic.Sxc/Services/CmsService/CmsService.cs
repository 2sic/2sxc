using ToSic.Lib.DI;
using ToSic.Razor.Blade;
using ToSic.Sxc.Data;
using ToSic.Sxc.Services.Internal;
using ToSic.Sxc.Services.Tweaks;
using InputTypes = ToSic.Sxc.Compatibility.Internal.InputTypes;

namespace ToSic.Sxc.Services.CmsService;

[PrivateApi("WIP + Hide Implementation")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class CmsService(Generator<CmsServiceStringWysiwyg> stringWysiwyg)
    : ServiceForDynamicCode($"{SxcLogName}.CmsSrv", connect: [stringWysiwyg]), ICmsService
{
    private Generator<CmsServiceStringWysiwyg> StringWysiwygGen => _stringWysiwyg ??= stringWysiwyg.SetInit(s => s.ConnectToRoot(_CodeApiSvc));
    private Generator<CmsServiceStringWysiwyg> _stringWysiwyg;

    public IHtmlTag Html(
        object thing,
        NoParamOrder noParamOrder = default,
        object container = default,
        string classes = default,
        bool debug = default,
        object imageSettings = default,
        bool? toolbar = default,
        Func<ITweakInput<string>, ITweakInput<string>> tweak = default
    )
    {
        var field = thing as IField;
        var l = Log.Fn<IHtmlTag>($"Field: {field?.Name}");
        // Initialize the container helper, as we'll use it a few times
        var cntHelper = new CmsServiceContainerHelper(_CodeApiSvc, field, container, classes, toolbar, Log);

        // New v17 - preprocess the tweaks if available
        // Note that we should use the field if one was found, only use the "thing" if there was no field
        // Otherwise there is the risk that "Raw" is null (like new wysiwyg field before adding text)
        // and it would then revert to showing "ToSic.Sxc.Data.Internal.Field"
        var value = field != null
            ? field.Raw?.ToString()
            : thing?.ToString();
        value = ProcessTweaks(tweak, value, l);

        // If it's not a field, we cannot find out more about the object
        // In that case, just wrap the result in the container and return it
        if (field is null)
            return l.Return(cntHelper.Wrap(value ?? thing, defaultToolbar: false), "No field, will just treat as value");

        // Get Content type and field information
        var contentType = field.Parent.Entity?.Type; // Entity can be null on mock data
        if (contentType == null)
            return l.Return(cntHelper.Wrap(value, defaultToolbar: false), "can't find content-type, treat as value");

        var attribute = contentType[field.Name];
        if (attribute == null)
            return l.Return(cntHelper.Wrap(value, defaultToolbar: false), "no attribute info, treat as value");

        // Now we handle all kinds of known special treatments
        // Start with strings...
        if (attribute.Type == ValueTypes.String)
            return l.Return(HtmlString(contentType, attribute, field, value, imageSettings, cntHelper, debug), "string");

        // Fallback...
        return l.Return(cntHelper.Wrap(value, defaultToolbar: false), "nothing else hit, will treat as value");
    }

    private IHtmlTag HtmlString(IContentType contentType, IContentTypeAttribute attribute, IField field, string value, object imageSettings, CmsServiceContainerHelper cntHelper, bool debug)
    {
        var l = Log.Fn<IHtmlTag>($"Attribute: {attribute?.Name}");
        var inputType = attribute.InputType();
        if (debug) l.A($"Field type is: {ValueTypes.String}:{inputType}");

        // Not WYSIWYG = normal string, no toolbar by default
        if (inputType != InputTypes.InputTypeWysiwyg)
            return l.Return(cntHelper.Wrap(value, defaultToolbar: false), "string, default no toolbar");

        // WYSIWYG
        var fieldAdam = _CodeApiSvc.Cdf.Folder(field.Parent, field.Name, field);
        var htmlResult = StringWysiwygGen.New()
            .Init(field, contentType, attribute, fieldAdam, debug, imageSettings)
            .HtmlForStringAndWysiwyg(value);

        return htmlResult.IsProcessed
            ? l.Return(cntHelper.Wrap(htmlResult, defaultToolbar: true), "wysiwyg, default w/toolbar")
            : l.Return(cntHelper.Wrap(value, defaultToolbar: true), "wysiwyg, not converted, w/toolbar");
    }

    private static string ProcessTweaks(Func<ITweakInput<string>, ITweakInput<string>> tweak, string value, ILog log)
    {
        var l = log.Fn<string>();
        if (tweak == null) return l.Return(value, "no tweaks");

        try
        {
            var tweakHtml = (TweakInput<string>)tweak(new TweakInput<string>());
            var valueTweak = tweakHtml.Preprocess(value);
            return l.Return(valueTweak.Value, "tweaked");
        }
        catch (Exception e)
        {
            var ex = new Exception($"Error in processing {nameof(tweak)}", e);
            throw l.Ex(ex);
        }
    }
}