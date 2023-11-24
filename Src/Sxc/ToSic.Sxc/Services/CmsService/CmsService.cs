using ToSic.Eav.Data;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Razor.Blade;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Services.CmsService;

[PrivateApi("WIP + Hide Implementation")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class CmsService: ServiceForDynamicCode, ICmsService
{
    private readonly Generator<CmsServiceStringWysiwyg> _stringWysiwyg;

    public CmsService(
        Generator<CmsServiceStringWysiwyg> stringWysiwyg
    ) : base(Constants.SxcLogName + ".CmsSrv")
    {
        ConnectServices(
            _stringWysiwyg = stringWysiwyg.SetInit(s => s.ConnectToRoot(_DynCodeRoot))
        );
    }

    public IHtmlTag Html(
        object thing,
        string noParamOrder = Eav.Parameters.Protector,
        object container = default,
        string classes = default,
        bool debug = default,
        object imageSettings = default,
        bool? toolbar = default
    )
    {
        var field = thing as IField;
        var l = Log.Fn<IHtmlTag>($"Field: {field?.Name}");
        // Initialize the container helper, as we'll use it a few times
        var cntHelper = new CmsServiceContainerHelper(_DynCodeRoot, field, container, classes, toolbar, Log);

        // If it's not a field, we cannot find out more about the object
        // In that case, just wrap the result in the container and return it
        if (field is null)
            return l.Return(cntHelper.Wrap(thing, defaultToolbar: false), "No field, will just treat as value");

        // Get Content type and field information
        var value = field.Raw;
        var contentType = field.Parent.Entity?.Type; // Entity can be null on mock data
        if (contentType == null)
            return l.Return(cntHelper.Wrap(value, defaultToolbar: false), "can't find content-type, treat as value");

        var attribute = contentType[field.Name];
        if (attribute == null)
            return l.Return(cntHelper.Wrap(value, defaultToolbar: false), "no attribute info, treat as value");

        // Now we handle all kinds of known special treatments
        // Start with strings...
        if (attribute.Type == ValueTypes.String)
        {
            var inputType = attribute.InputType();
            if (debug) l.A($"Field type is: {ValueTypes.String}:{inputType}");
            // ...wysiwyg
            if (inputType == ToSic.Sxc.Compatibility.InputTypes.InputTypeWysiwyg)
            {
                var fieldAdam = _DynCodeRoot.AsAdam(field.Parent, field.Name);
                var htmlResult = _stringWysiwyg.New()
                    .Init(field, contentType, attribute, fieldAdam, debug, imageSettings)
                    .HtmlForStringAndWysiwyg();
                return htmlResult.IsProcessed
                    ? l.Return(cntHelper.Wrap(htmlResult, defaultToolbar: true), "wysiwyg, default w/toolbar")
                    : l.Return(cntHelper.Wrap(value, defaultToolbar: true), "wysiwyg, not converted, w/toolbar");
            }

            // normal string, no toolbar by default
            return l.Return(cntHelper.Wrap(value, defaultToolbar: false), "string, default no toolbar");
        }

        // Fallback...
        return l.Return(cntHelper.Wrap(value, defaultToolbar: false), "nothing else hit, will treat as value");
    }
        
}