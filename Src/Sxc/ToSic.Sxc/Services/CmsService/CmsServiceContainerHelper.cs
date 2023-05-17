using System;
using System.Linq;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Helpers;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Razor.Blade;
using ToSic.Sxc.Code;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Services.CmsService
{
    internal class CmsServiceContainerHelper: ServiceBase
    {
        private readonly IDynamicCodeRoot _dynCodeRoot;
        private readonly object _container;
        private string Classes { get; set; }
        private readonly bool? _toolbar;
        private readonly IDynamicField _field;

        public CmsServiceContainerHelper(IDynamicCodeRoot dynCodeRoot,
            IDynamicField field,
            object container,
            string classes,
            bool? toolbar,
            ILog parentLog
            ) : base("Cms.SvcCnt")
        {
            this.LinkLog(parentLog);
            _dynCodeRoot = dynCodeRoot;
            _field = field;
            _container = container;
            Classes = classes;
            _toolbar = toolbar;
        }


        private ServiceKit14 ServiceKit => _svcKit.Get(() => _dynCodeRoot.GetKit<ServiceKit14>());
        private readonly GetOnce<ServiceKit14> _svcKit = new GetOnce<ServiceKit14>();


        public IHtmlTag Wrap(CmsProcessed result, bool defaultToolbar)
        {
            Classes = string.Join(" ", new[] { Classes, result.Classes }.Where(x => x.HasValue()));
            return Wrap(result.Contents, defaultToolbar: defaultToolbar);
        }

        public IHtmlTag Wrap(object contents, bool defaultToolbar)
        {
            var tag = GetContainer(_container);
            var toolbar = _toolbar ?? defaultToolbar;
            // If tag is not a real tag (no name) then it also can't have classes or toolbars; just finish and return
            if (!tag.TagName.HasValue())
                return tag.Wrap(contents);

            // Add classes if we can
            if (Classes.HasValue()) tag = tag.Class(Classes);
            if (toolbar && _field != null)
                tag.Attr(ServiceKit.Toolbar.Empty().Edit(_field.Parent, fields: _field.Name, tweak: b => b.Icon(EditFieldIcon)));
            return tag.Wrap(contents);
        }

        private const string EditFieldIcon =
            "<svg xmlns=\"http://www.w3.org/2000/svg\" height=\"48\" viewBox=\"0 96 960 960\" width=\"48\"><path d=\"M180 1044q-24 0-42-18t-18-42V384q0-24 18-42t42-18h405l-60 60H180v600h600V636l60-60v408q0 24-18 42t-42 18H180Zm300-360Zm182-352 43 42-285 284v86h85l286-286 42 42-303 304H360V634l302-302Zm171 168L662 332l100-100q17-17 42.311-17T847 233l84 85q17 18 17 42.472T930 402l-97 98Z\"/></svg>";

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
