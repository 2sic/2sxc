using System;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Logging;
using ToSic.Razor.Blade;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Services.CmsService
{
    [PrivateApi("WIP")]
    public class CmsService: HasLog, ICmsService
    {
        public CmsService() : base(Constants.SxcLogName + ".CmsSrv")
        {
        }

        public IHtmlTag Show(object thing, string noParamOrder = Eav.Parameters.Protector, object container = null)
        {
            var l = Log.Fn<IHtmlTag>();

            // Prepare the real container
            var realContainer = GetContainer(container);

            // If it's not a field, we cannot find out more about the object
            // In that case, just wrap the result in the container and return it
            if (!(thing is IDynamicField field))
                return l.Return(realContainer.Wrap(thing), "No field, will just treat as value");

            // Get Content type and field information
            var contentType = field.Parent.Entity.Type;
            if (contentType == null)
                return l.Return(realContainer.Wrap(thing), "can't find content-type, treat as value");

            var attribute = contentType[field.Name];
            if (attribute == null) 
                return l.Return(realContainer.Wrap(thing), "no attribute info, treat as value");

            // Now we handle all kinds of known special treatments
            // Start with strings...
            if (attribute.ControlledType == ValueTypes.String)
            {
                // ...wysiwyg
                if (attribute.InputType() == InputTypes.InputTypeWysiwyg)
                {
                    // todo: special treatment now @STV
                    // 1. check if we have an img tags with data-cmsid="file:..." attributes
                    // ...if not, return wrapped

                    // 2. If yes, take them apart using regex
                    // ...best re-use code that you already have to extract script tags
                    // - also specially look at the classes
                    // - and the data-cmsid property

                    // 3. Based on the classes, check if you can find something like
                    // ..."wysiwyg-width#of#" - this is for resize ratios
                    // Convert to a format like "#/#" - as a string


                    // 4. Then use the IImageService to create Picture tags for it
                    // - use the "#/#" as the `factor` parameter

                    // Remember to re-attach an alt-attribute, class etc. from the original if it had it
                    // Then reconstruct the original html
                    // ...and return wrapped in the realContainer
                }
            }


            // Fallback...
            return l.Return(realContainer.Wrap(thing), "nothing else hit, will treat as value");
        }

        private IHtmlTag GetContainer(object container)
        {
            var l = Log.Fn<IHtmlTag>();
            // Already an ITag
            if (container is IHtmlTag iTagContainer) return l.Return(iTagContainer, "container is Blade tag");

            if (container is string tagName)
            {
                if (!tagName.IsEmpty() && !tagName.Contains(" "))
                    return l.Return(Tag.Custom(tagName), "was a tag name, created tag");
                throw new ArgumentException("Must be a tag name like 'div' or a RazorBlade Html Tag object",
                    nameof(container));
            }

            // Nothing to do, just return an empty tag which can be filled...
            return l.Return(Tag.RawHtml(), "no container, return empty tag");
        }

    }
}
