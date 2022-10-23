using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Images
{
    [PrivateApi("BETA, not sure if we need this")]
    public class ResizeSettingsFactory
    {

        /// <summary>
        /// Can only be constructed internally
        /// </summary>
        internal ResizeSettingsFactory(ImageService parent) => _parent = parent;
        private readonly ImageService _parent;


        //public MultiResizeRule MultiRule(
        //    string noParamOrder = Parameters.Protector,
        //    double factor = default,
        //    string srcset = default,
        //    string sizes = default,
        //    string media = default
        //)
        //{
        //    var rule = new MultiResizeRule();
        //    if (!DNearZero(factor)) rule.FactorParsed = factor;
        //    if (srcset != default) rule.SrcSet = srcset;
        //    if (media != default) rule.Media = media;
        //    if (sizes != default) rule.Sizes = sizes;
        //    return rule;
        //}

    }
}
