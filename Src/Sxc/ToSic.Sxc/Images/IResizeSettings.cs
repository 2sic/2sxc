﻿using System.Collections.Specialized;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Images
{
    /// <summary>
    /// Settings how to resize an image for the `src` or `srcset` attributes.
    ///
    /// It's read only, to create it, use the <see cref="ToSic.Sxc.Services.IImageService"/>
    /// </summary>
    [InternalApi_DoNotUse_MayChangeWithoutNotice("Still WIP")]
    public interface IResizeSettings
    {
        /// <summary>
        /// Width to resize to.
        /// If 0, width will not be changed
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Height to resize to.
        /// If 0, height will not be changed
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Quality factor for image formats which support quality.
        /// Usually a value between 0 and 100.
        /// If 0, quality will not be changed.
        /// </summary>
        int Quality { get; }

        /// <summary>
        /// Resize mode.
        /// If empty or "(none)" will not be used. 
        /// </summary>
        string Mode { get; }

        /// <summary>
        /// Scale Mode.
        /// If empty or "(none)" will not be used. 
        /// </summary>
        string Scale { get; }

        /// <summary>
        /// Target format like 'jpg' or 'png'.
        /// If empty will not be used. 
        /// </summary>
        string Format { get; }

        /// <summary>
        /// SrcSet to generate.
        /// </summary>
        string SrcSet { get; }

        /// <summary>
        /// Additional url parameters in case the final link would need this.
        /// Rarely used, but can be used for resize parameters which are not standard. 
        /// </summary>
        NameValueCollection Parameters { get; }
    }
}