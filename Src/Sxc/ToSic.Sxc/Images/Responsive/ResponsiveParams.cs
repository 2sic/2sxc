using ToSic.Eav;
using ToSic.Eav.Documentation;
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
        public IDynamicField Field { get; }
        public object Settings { get; }
        public object Factor { get; }
        public string ImgAlt { get; }
        public string ImgClass { get; }
        public AdvancedSettings Advanced { get; }

        internal ResponsiveParams(
            string method,
            object link,
            string noParamOrder = Parameters.Protector,
            string url = default,
            IDynamicField field = default,
            object settings = null,
            object factor = null,
            string imgAlt = null,
            string imgClass = null,
            AdvancedSettings advanced = null)
        {
            Field = field ?? link as IDynamicField;
            Link = (IHasLink)Field ?? new HasLink(url ?? link as string);
            Settings = settings;
            Factor = factor;
            ImgAlt = imgAlt;
            ImgClass = imgClass;
            Advanced = advanced;
            WarningParamsPicImg(method, noParamOrder);
        }


        private static void WarningParamsPicImg(string mName, string noParamOrder)
            => Parameters.ProtectAgainstMissingParameterNames(noParamOrder, mName, "url, field, factor, imgAlt, imgClass, recipe");

    }
}
