using System;
using ToSic.Eav.Documentation;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Logging;
using ToSic.Razor.Blade;
using ToSic.Razor.Markup;

namespace ToSic.Sxc.Services.CmsService
{
    [PrivateApi("WIP")]
    public class CmsService: HasLog, ICmsService
    {
        public CmsService() : base(Constants.SxcLogName + ".CmsSrv")
        {
        }

        public IHtmlTag Show(object thing, string noParamOrder = Eav.Parameters.Protector,
            object container = null)
        {
            // Prepare the real container
            var realContainer = GetContainer(container);

            // Todo: Check if the thing is a Field, if not, just return the thing wrapped inside a raw tag

            // todo: check if the field is of type string-wysiwyg, otherwise just return the thing inside a tag
            return realContainer;
        }

        private IHtmlTag GetContainer(object container)
        {
            // Already an ITag
            if (container is IHtmlTag iTagContainer) return iTagContainer;

            if (container is string tagName)
            {
                if (!tagName.IsEmpty() && !tagName.Contains(" "))
                    return Tag.Custom(tagName);
                throw new ArgumentException("Must be a tag name like 'div' or a RazorBlade Html Tag object",
                    nameof(container));
            }

            // Nothing to do, just return an empty tag which can be filled...
            return Tag.RawHtml();
        }

    }
}
