using System;
using System.Linq;
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
            string classes = default,
            bool debug = default
        ) => Log.Func(l =>
        {
            // If it's not a field, we cannot find out more about the object
            // In that case, just wrap the result in the container and return it
            if (!(thing is IDynamicField field))
                return (GetContainerAndWrap(container, thing, classes), "No field, will just treat as value");

            // Get Content type and field information
            var contentType = field.Parent.Entity.Type;
            if (contentType == null)
                return (GetContainerAndWrap(container, thing, classes), "can't find content-type, treat as value");

            var attribute = contentType[field.Name];
            if (attribute == null)
                return (GetContainerAndWrap(container, thing, classes), "no attribute info, treat as value");

            // Now we handle all kinds of known special treatments
            // Start with strings...
            if (attribute.Type == ValueTypes.String)
            {
                // ...wysiwyg
                if (attribute.InputType() == InputTypes.InputTypeWysiwyg)
                {
                    var htmlResult = _stringWysiwyg.New().Init(field, contentType, attribute, debug).Process();
                    return htmlResult.IsProcessed
                        ? (GetContainerAndWrap(container, htmlResult, classes), "ok")
                        : (GetContainerAndWrap(container, thing, classes), "not converted");
                }
            }

            // Fallback...
            return (GetContainerAndWrap(container, thing, classes), "nothing else hit, will treat as value");
        });

        private IHtmlTag GetContainerAndWrap(object container, CmsProcessed result, string classes)
        {
            classes = string.Join(" ", new[] { classes, result.Classes }.Where(x => x.HasValue()));
            return GetContainerAndWrap(container ?? result.DefaultTag, result.Contents, classes);
        }

        private IHtmlTag GetContainerAndWrap(object container, object contents, string classes)
        {
            var tag = GetContainer(container);
            if (classes.HasValue()) tag = tag.Class(classes);
            return tag.Wrap(contents);
        }

        private IHtmlTag GetContainer(object container) => Log.Func(l =>
        {
            // Already an ITag
            if (container is IHtmlTag iTagContainer)
                return (iTagContainer, "container is Blade tag");

            if (container is string tagName)
            {
                if (tagName.IsEmpty())
                    return (Tag.RawHtml(), "no container, return empty tag");
                if (!tagName.Contains(" "))
                    return (Tag.Custom(tagName), "was a tag name, created tag");
                throw new ArgumentException("Must be a tag name like 'div' or a RazorBlade Html Tag object",
                    nameof(container));
            }

            // Nothing to do, just return an empty tag which can be filled...
            return (Tag.Div(), "no container, return div tag");
        });

    }
}
