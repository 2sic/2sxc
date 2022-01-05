using System;
using System.Linq;
using static ToSic.Sxc.Plumbing.ParseObject;

namespace ToSic.Sxc.Web.Images
{
    public class SrcSetParser
    {
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
                .Split('=')
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
                part.Size = (float)Math.Round(DoubleOrNull(mainPart) ?? 0, 2);

                // If it's a real size and we didn't already set the Type, set it based on the value range
                if (part.SizeType == SrcSetPart.SizeDefault && !DNearZero(part.Size))
                    part.SizeType = part.Size < 10 ? SrcSetPart.SizePixelDensity : SrcSetPart.SizeWidth;
            }

            // If size type is width, then we must round to int and the width must have the same value
            if (part.SizeType == SrcSetPart.SizeWidth) 
                part.Size = part.Width = (int)part.Size;

            if (withMore.Length > 1)
            {
                var moreParts = withMore[1].Split(MoreSeparator).Select(s => s.Trim()).ToArray();
                if (!string.IsNullOrEmpty(moreParts[0]))
                    part.Width = IntOrNull(moreParts[0]) ?? 0;
                if (moreParts.Length > 1 && !string.IsNullOrEmpty(moreParts[1]))
                    part.Height = IntOrNull(moreParts[1]) ?? 0;
            }

            return part;
        }

        private const char PartSeparator = ',';
        private const char MoreSeparator = ':';

    }
}
