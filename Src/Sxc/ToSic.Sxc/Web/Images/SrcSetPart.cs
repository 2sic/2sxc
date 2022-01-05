namespace ToSic.Sxc.Web.Images
{
    public class SrcSetPart
    {
        public const char SizeDefault = 'd';
        public const char SizeWidth = 'w';
        public const char SizePixelDensity = 'x';
        public static readonly char[] SizeTypes = { SizeWidth, SizePixelDensity, SizeDefault };


        public float Size;
        public char SizeType = SizeDefault;
        public int Width;
        public int Height;

        public SrcSetPart() {}

        public SrcSetPart(float size, char sizeType, int width, int height)
        {
            Size = size;
            SizeType = sizeType;
            Width = width;
            Height = height;
        }
    }
}
