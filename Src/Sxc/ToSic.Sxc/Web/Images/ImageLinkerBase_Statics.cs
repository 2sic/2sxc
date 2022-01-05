namespace ToSic.Sxc.Web.Images
{
    public abstract partial class ImageLinkerBase
    {
        internal static string CorrectScales(string scale)
        {
            // ReSharper disable RedundantCaseLabel
            switch (scale?.ToLowerInvariant())
            {
                case "up":
                case "upscaleonly":
                    return "upscaleonly";
                case "both":
                    return "both";
                case "down":
                case "downscaleonly":
                    return "downscaleonly";
                case null:
                default:
                    return null;
            }
            // ReSharper restore RedundantCaseLabel
        }

        internal static string CorrectFormats(string format)
        {
            switch (format?.ToLowerInvariant())
            {
                case "jpg":
                case "jpeg": return "jpg";
                case "png": return "png";
                case "gif": return "gif";
                default: return null;
            }
        }
    }
}
