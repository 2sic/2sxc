using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Context
{
    [PrivateApi("Experimental")]
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
        /// The Configuration Entity
        /// </summary>
        [PrivateApi]
        dynamic Configuration { get; }

        /// <summary>
        /// Possible i18n Resources for this View WIP
        /// </summary>
        [PrivateApi]
        dynamic Resources { get; }

        /// <summary>
        /// Possible Settings for this View WIP
        /// If we do implement it, it's meant for views which share the same template file
        /// </summary>
        [PrivateApi]
        dynamic Settings { get; }
    }
}
