using System.Linq;
using static ToSic.Sxc.Plumbing.ParseObject;

namespace ToSic.Sxc.Images
{
    public class FactorMapHelper
    {
        
        private static MultiResizeRuleBundle Find(ResizeSettings resizeSettings)
        {
            var maps = resizeSettings?.MultiResize?.Factors;
            if (maps == null || !maps.Any()) return null;
            var factor = resizeSettings.Factor;
            if (DNearZero(factor)) factor = 1;
            var fm = maps.FirstOrDefault(m => DNearZero(m.Factor - factor));
            return fm;
        }

        public static IMultiResizeRule Find(ResizeSettings resizeSettings, SrcSetType srcSetType)
        {
            var advancedSettings = resizeSettings?.MultiResize;
            if (advancedSettings == null) return null;
            var fm = Find(resizeSettings);
            var def = advancedSettings.Resize;
            
            if (srcSetType == SrcSetType.ImgSrc || srcSetType == SrcSetType.ImgSrcSet)
                return fm?.Img ?? fm ?? def?.Img ?? def;

            return fm?.Sources.FirstOrDefault() ?? fm ?? def?.Sources.FirstOrDefault() ?? def;
        }

    }

    public enum SrcSetType
    {
        ImgSrc,
        ImgSrcSet,
        Sources
    }

}
