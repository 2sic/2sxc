using System.Collections.Specialized;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Images
{
    [InternalApi_DoNotUse_MayChangeWithoutNotice("Still WIP")]
    public interface IResizeParameters
    {
        /// <summary>
        /// Width to resize to.
        /// If 0, width will not be changed
        /// </summary>
        int Width { get; set; }

        /// <summary>
        /// Height to resize to.
        /// If 0, height will not be changed
        /// </summary>
        int Height { get; set; }

        /// <summary>
        /// Quality factor for image formats which support quality.
        /// Usually a value between 0 and 100.
        /// If 0, quality will not be changed.
        /// </summary>
        int Quality { get; set; }

        /// <summary>
        /// Resize mode.
        /// If empty or "(none)" will not be used. 
        /// </summary>
        string Mode { get; set; }

        /// <summary>
        /// Scale Mode.
        /// If empty or "(none)" will not be used. 
        /// </summary>
        string Scale { get; set; }

        /// <summary>
        /// Target format like 'jpg' or 'png'.
        /// If empty will not be used. 
        /// </summary>
        string Format { get; set; }

        /// <summary>
        /// Additional url parameters in case the final link would need this.
        /// Rarely used, but can be used for resize parameters which are not standard. 
        /// </summary>
        NameValueCollection Parameters { get; set; }
    }
}