using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Context
{
    /// <summary>
    /// View context information - this is Experimental / BETA WIP
    /// </summary>
    /// <remarks>
    /// Added in 2sxc 12.02
    /// </remarks>
    public interface ICmsView
    {
        /// <summary>
        /// View configuration ID
        /// </summary>
        int Id { get; }
        
        /// <summary>
        /// Name of the view as configured
        /// </summary>
        string Name { get; }
        
        /// <summary>
        /// Edition used - if any
        /// </summary>
        string Edition { get; }
        
        /// <summary>
        /// The Configuration Entity of the View
        /// </summary>
        dynamic Configuration { get; }

        /// <summary>
        /// Custom Language Resources for this View
        /// </summary>
        dynamic Resources { get; }

        /// <summary>
        /// Custom Settings for this View
        /// </summary>
        dynamic Settings { get; }
    }
}
