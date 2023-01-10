using System;
using ToSic.Eav.Apps.Run;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Context
{
    /// <summary>
    /// A unit / block within the CMS. Contains all necessary identification to pass around.
    /// </summary>
    [PrivateApi]

    public interface IModule
#if NETFRAMEWORK
#pragma warning disable 618
        // in this case we must also inherit from IContainer - legacy of the signature for CustomizeSearch
        : ToSic.Eav.Run.IContainer
#pragma warning restore 618
#endif
    {
        [PrivateApi("Workaround till we have DI to inject the current container")]
        IModule Init(int id);

        /// <summary>
        /// The module id on the page. 
        /// 
        /// 🪒 Use in Razor: `CmsContext.Module.Id`
        /// </summary>
        /// <remarks>
        /// Corresponds to the Dnn ModuleId or the Oqtane Module Id.
        /// 
        /// In some systems a module can be re-used on multiple pages, and possibly have different settings for re-used modules.
        /// 2sxc doesn't use that, so the module id corresponds to the Dnn ModuleId and not the PageModuleId.  
        /// </remarks>
        /// <returns>The ID, unless unknown, in which case it's a negative number</returns>
        int Id { get; }


        /// <summary>
        /// Determines if this is a the primary App (the content-app) as opposed to any additional app
        /// </summary>
        [PrivateApi("don't think this should be here! also not sure if it's the primary - or the contentApp! reason seems to be that we detect it by the DNN module name")]
        bool IsContent { get; }

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
