using ToSic.Eav;
using ToSic.Eav.Metadata;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Images
{
    /// <summary>
    /// Helper class to handle all kinds of parameters passed to a responsive tag
    /// </summary>
    [PrivateApi]
    public class ResponsiveParams
    {
        /// <summary>
        /// The only reliable object which knows about the url - can never be null
        /// </summary>
        public IHasLink Link { get; }

        /// <summary>
        /// The field used for this responsive output - can be null!
        /// </summary>
        public IField Field { get; }
        public IHasMetadata HasDecoOrNull { get; }
        public IResizeSettings Settings { get; }
        public string ImgAlt { get; }
        public string ImgAltFallback { get; }
        public string ImgClass { get; }

        public string PicClass { get; }

        internal ResponsiveParams(
            string method,
            object link,
            string noParamOrder = Parameters.Protector,
            IResizeSettings settings = default,
            string imgAlt = default,
            string imgAltFallback = default,
            string imgClass = default,
            string picClass = default
            )
        {
            Parameters.ProtectAgainstMissingParameterNames(noParamOrder, method,
                $"{nameof(link)}, {nameof(settings)}, factor, {nameof(imgAlt)}, {nameof(imgClass)}, recipe");

            Field = link as IField ?? (link as IFromField)?.Field;
            HasDecoOrNull = link as IHasMetadata;
            Link = link as IHasLink ?? new HasLink(link as string);
            Settings = settings;
            ImgAlt = imgAlt;
            ImgAltFallback = imgAltFallback;
            ImgClass = imgClass;
            PicClass = picClass;
        }

        public string Description => _description.Get(() => Field?.ImageDecoratorOrNull?.Description);
        private readonly GetOnce<string> _description = new GetOnce<string>();

    }
}
