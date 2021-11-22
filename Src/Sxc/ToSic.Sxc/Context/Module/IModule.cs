using System;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Context
{
    /// <summary>
    /// A unit / block within the CMS. Contains all necessary identification to pass around.
    /// </summary>
    [PrivateApi]

    public interface IModule: ICmsModule
#if NET472
        // in this case we must also inherit from IContainer - legacy of the signature for CustomizeSearch
#pragma warning disable 618
        , ToSic.Eav.Run.IContainer
#pragma warning restore 618
#endif
    {
        [PrivateApi("Workaround till we have DI to inject the current container")]
        IModule Init(int id, ILog parentLog);


        /// <summary>
        /// Determines if this is a the primary App (the content-app) as opposed to any additional app
        /// </summary>
        [PrivateApi("don't think this should be here! also not sure if it's the primary - or the contentApp! reason seems to be that we detect it by the DNN module name")]
        bool IsPrimary { get; }

        /// <summary>
        /// Identifies the content-block which should be shown in this container
        /// </summary>
        [PrivateApi("still experimental")]
        IBlockIdentifier BlockIdentifier { get; }
    }
}


// ReSharper disable once CheckNamespace
namespace ToSic.Eav.Run
{
    /// <summary>
    /// This interface is used in the Dnn RazorComponent of v10, so we must still support it.
    /// The only use case is in an overridable CustomizeSearch, so it is never really called,
    /// but just defined by a razor page.
    /// </summary>
    [Obsolete("this was replaced by IModule")]
    public interface IContainer
    {
    }
}
