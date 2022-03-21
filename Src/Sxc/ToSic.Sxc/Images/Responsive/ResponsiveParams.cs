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

        public string Url { get; }

        public IDynamicField Field { get; }
        public object Settings { get; }
        public object Factor { get; }
        public string ImgAlt { get; }
        public string ImgClass { get; }
        public AdvancedSettings Recipe { get; }

        internal ResponsiveParams(
            string method,
            string noParamOrder = Parameters.Protector,
            string url = default,
            IDynamicField field = default,
            object settings = null,
            object factor = null,
            string imgAlt = null,
            string imgClass = null,
            AdvancedSettings recipe = null)
        {
            Field = field;
            Url = url ?? field?.Parent.Get(field.Name) as string;
            Settings = settings;
            Factor = factor;
            ImgAlt = imgAlt;
            ImgClass = imgClass;
            Recipe = recipe;
            WarningParamsPicImg(method, noParamOrder);
        }


        private static void WarningParamsPicImg(string mName, string noParamOrder)
            => Parameters.ProtectAgainstMissingParameterNames(noParamOrder, mName, "url, field, factor, imgAlt, imgClass, recipe");

    }
}
