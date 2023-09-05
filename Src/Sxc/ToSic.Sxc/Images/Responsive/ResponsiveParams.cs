using ToSic.Eav;
using ToSic.Eav.Metadata;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Data;
using ToSic.Sxc.Edit.Toolbar;

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
        public IHasMetadata HasMetadataOrNull { get; }
        public IResizeSettings Settings { get; }
        public string ImgAlt { get; }
        public string ImgAltFallback { get; }
        public string ImgClass { get; }

        public string PicClass { get; }

        public object Toolbar { get; }

        internal ResponsiveParams(
            string method,
            object target,
            string noParamOrder = Parameters.Protector,
            IResizeSettings settings = default,
            string imgAlt = default,
            string imgAltFallback = default,
            string imgClass = default,
            string picClass = default,
            object toolbar = default
            )
        {
            Parameters.ProtectAgainstMissingParameterNames(noParamOrder, method,
                $"{nameof(target)}, {nameof(settings)}, factor, {nameof(imgAlt)}, {nameof(imgClass)}, recipe");

            Field = target as IField ?? (target as IFromField)?.Field;
            HasMetadataOrNull = target as IHasMetadata ?? Field;
            Link = target as IHasLink ?? new HasLink(target as string);
            Settings = settings;
            ImgAlt = imgAlt;
            ImgAltFallback = imgAltFallback;
            ImgClass = imgClass;
            PicClass = picClass;
            Toolbar = toolbar;
        }
    }
}
