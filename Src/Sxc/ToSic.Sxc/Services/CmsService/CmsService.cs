using System;
using ToSic.Eav.Data;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Razor.Blade;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Services.CmsService
{
    [PrivateApi("WIP + Hide Implementation")]
    public class CmsService: ServiceForDynamicCode, ICmsService
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

        public IHtmlTag Show(
            object thing,
            string noParamOrder = Eav.Parameters.Protector,
            object container = default,
            string classes = default
        ) => Log.Func(l =>
        {
            // Prepare the real container
            var realContainer = GetContainer(container);
            if (!classes.IsEmptyOrWs())
                realContainer.Class(classes);

            // If it's not a field, we cannot find out more about the object
            // In that case, just wrap the result in the container and return it
            if (!(thing is IDynamicField field))
                return (realContainer.Wrap(thing), "No field, will just treat as value");

            // Get Content type and field information
            var contentType = field.Parent.Entity.Type;
            if (contentType == null)
                return (realContainer.Wrap(thing), "can't find content-type, treat as value");

            var attribute = contentType[field.Name];
            if (attribute == null)
                return (realContainer.Wrap(thing), "no attribute info, treat as value");

            // Now we handle all kinds of known special treatments
            // Start with strings...
            if (attribute.Type == ValueTypes.String)
            {
                // ...wysiwyg
                if (attribute.InputType() == InputTypes.InputTypeWysiwyg)
                {
                    var html = _stringWysiwyg.New().Init(field, contentType, attribute).Process();
                    return html == null
                        ? (realContainer.Wrap(thing), "not converted")
                        : (realContainer.Class(CmsServiceStringWysiwyg.WysiwygClassToAdd).Wrap(html), "ok");
                }
            }

            // Fallback...
            return (realContainer.Wrap(thing), "nothing else hit, will treat as value");
        });



        private IHtmlTag GetContainer(object container) => Log.Func(l =>
        {
            // Already an ITag
            if (container is IHtmlTag iTagContainer)
                return (iTagContainer, "container is Blade tag");

            if (container is string tagName)
            {
                if (!tagName.IsEmpty() && !tagName.Contains(" "))
                    return (Tag.Custom(tagName), "was a tag name, created tag");
                throw new ArgumentException("Must be a tag name like 'div' or a RazorBlade Html Tag object",
                    nameof(container));
            }

            // Nothing to do, just return an empty tag which can be filled...
            return (Tag.RawHtml(), "no container, return empty tag");
        });

    }
}
