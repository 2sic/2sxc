using System;
using System.Globalization;
using System.Linq;
using ToSic.Eav.Documentation;
using static ToSic.Sxc.Plumbing.ParseObject;
using static ToSic.Sxc.Images.SrcSetPart;

namespace ToSic.Sxc.Images
{
    [PrivateApi("Hide implementation")]
    public class SrcSetParser
    {
        public const char KeyValueSeparator = '=';
        public const char PartSeparator = ',';
        public const char WidthSeparator = ':';


        public static SrcSetPart[] ParseSet(string srcSet)
        {
            if (string.IsNullOrWhiteSpace(srcSet)) return Array.Empty<SrcSetPart>();

            var partStrings = srcSet.Split(PartSeparator)
                .Select(s => s.Trim())
                .Select(ParsePart);

            return partStrings.ToArray();
        }

        public static SrcSetPart ParsePart(string partString)
        {
            var withMore = (partString ?? string.Empty)
                .Split(KeyValueSeparator)
                .Select(more => more.Trim())
                .ToArray();

            var part = new SrcSetPart();

            // A 0 index must always exist if the previous string was non-null, which is guaranteed
            var mainPart = withMore[0];
            if (!string.IsNullOrEmpty(mainPart))
            {
                var lastChar = mainPart.ToLowerInvariant().Last();
                if (SrcSetPart.SizeTypes.Contains(lastChar)) part.SizeType = lastChar;
                mainPart = mainPart.TrimEnd(SrcSetPart.SizeTypes);

                var sizeAsNumber = DoubleOrNull(mainPart);
                if (sizeAsNumber == null)
                {
                    // Try calculating - if it is a calculation, we always assume factor-mode '*'
                    sizeAsNumber = DoubleOrNullWithCalculation(mainPart);
                    if (sizeAsNumber != null && part.SizeType == SrcSetPart.SizeDefault)
                        part.SizeType = SrcSetPart.SizeFactorOf;
                }
                part.Size = (float)Math.Round(sizeAsNumber ?? 0, 2);

                // If it's a real size and we didn't already set the Type, set it based on the value range
                if (part.SizeType == SrcSetPart.SizeDefault && !DNearZero(part.Size))
                    part.SizeType = part.Size < 1 
                        ? SrcSetPart.SizeFactorOf // Less than 1 - can't be pixel density, must be factor
                        : part.Size < 10 ? SrcSetPart.SizePixelDensity : SrcSetPart.SizeWidth;
            }

            // If size type is width, then we must round to int and the width must have the same value
            if (part.SizeType == SrcSetPart.SizeWidth) 
                part.Size = part.Width = (int)part.Size;

            if (withMore.Length > 1)
            {
                var moreParts = withMore[1].Split(WidthSeparator).Select(s => s.Trim()).ToArray();
                if (!string.IsNullOrEmpty(moreParts[0]))
                    part.Width = IntOrNull(moreParts[0]) ?? 0;
                if (moreParts.Length > 1 && !string.IsNullOrEmpty(moreParts[1]))
                    part.Height = IntOrNull(moreParts[1]) ?? 0;
            }

            return part;
        }

        public static string SrcSetSuffix(SrcSetPart ssConfig, int finalWidth)
        {
            var srcSetSize = ssConfig.Size;
            var srcSetSizeTypeCode = ssConfig.SizeType;
            if (srcSetSizeTypeCode == SizeFactorOf)
            {
                srcSetSize = finalWidth;
                srcSetSizeTypeCode = SizeWidth;
            }

            var suffix = $" {srcSetSize.ToString(CultureInfo.InvariantCulture)}{srcSetSizeTypeCode}";
            return suffix;
        }

    }
}
