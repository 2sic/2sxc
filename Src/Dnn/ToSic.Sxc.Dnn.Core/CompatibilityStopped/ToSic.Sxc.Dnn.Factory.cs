using System.Runtime.CompilerServices;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Context;
using ToSic.Sxc.Dnn.Code;
using IApp = ToSic.Sxc.Apps.IApp;
using ILog = ToSic.Lib.Logging.ILog;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.Dnn;

/// <summary>
/// This is a factory to create CmsBlocks, Apps etc. and related objects from DNN.
/// </summary>
[PublicApi]
[Obsolete]
public static class Factory
{
    internal static string GenerateMessage([CallerMemberName] string cName = default)
        => $"The old {nameof(Factory)}.{cName}() API has been deprecated since v13 and announced for removal in v15. They were removed in v20. " +
           $"Please use Dependency Injection and the IRenderService or IDynamicCodeService instead. " +
           $"For guidance, see https://go.2sxc.org/brc-13-dnn-factory";

    /// <summary>
    /// Get a Root CMS Block if you know the TabId and the ModId
    /// </summary>
    /// <param name="pageId">The DNN tab id (page id)</param>
    /// <param name="modId">The DNN Module id</param>
    /// <returns>An initialized CMS Block, ready to use/render</returns>
    [Obsolete]
    public static IBlockBuilder CmsBlock(int pageId, int modId)
        => throw new NotSupportedException(GenerateMessage());

    /// <summary>
    /// Get a Root CMS Block if you know the TabId and the ModId
    /// </summary>
    /// <param name="pageId">The DNN tab id (page id)</param>
    /// <param name="modId">The DNN Module id</param>
    /// <param name="parentLog">The parent log, optional</param>
    /// <returns>An initialized CMS Block, ready to use/render</returns>
    [Obsolete]
    public static IBlockBuilder CmsBlock(int pageId, int modId, ILog parentLog)
        => throw new NotSupportedException(GenerateMessage());

    /// <summary>
    /// Get a Root CMS Block if you have the ModuleInfo object
    /// </summary>
    /// <param name="moduleInfo">A DNN ModuleInfo object</param>
    /// <returns>An initialized CMS Block, ready to use/render</returns>
    [Obsolete]
    public static IBlockBuilder CmsBlock(ModuleInfo moduleInfo)
        => throw new NotSupportedException(GenerateMessage());

    /// <summary>
    /// Get a Root CMS Block if you have the ModuleInfo object.
    /// </summary>
    /// <param name="module"></param>
    /// <param name="parentLog">optional logger to attach to</param>
    /// <returns>An initialized CMS Block, ready to use/render</returns>
    [Obsolete]
    public static IBlockBuilder CmsBlock(IModule module, ILog parentLog = null)
        => throw new NotSupportedException(GenerateMessage());

    /// <summary>
    /// Retrieve a helper object which provides commands like AsDynamic, AsEntity etc.
    /// </summary>
    /// <param name="blockBuilder">CMS Block for which the helper is targeted. </param>
    /// <returns>A Code Helper based on <see cref="IDnnDynamicCode"/></returns>
    [Obsolete]
    public static IDnnDynamicCode DynamicCode(IBlockBuilder blockBuilder)
        => throw new NotSupportedException(GenerateMessage());

    /// <summary>
    /// Get a full app-object for accessing data of the app from outside
    /// </summary>
    /// <param name="appId">The AppID of the app you need</param>
    /// <param name="unusedButKeepForApiStability">
    /// Tells the App that you'll be using page-publishing.
    /// So changes will me auto-drafted for a future release as the whole page together.
    /// </param>
    /// <param name="showDrafts">Show draft items - usually false for visitors, true for editors/admins.</param>
    /// <param name="parentLog">optional logger to attach to</param>
    /// <returns>An initialized App object which you can use to access App.Data</returns>
    [Obsolete]
    public static IApp App(int appId, bool unusedButKeepForApiStability = false, bool showDrafts = false, ILog parentLog = null)
        => throw new NotSupportedException(GenerateMessage());

    /// <summary>
    /// Get a full app-object for accessing data of the app from outside
    /// </summary>
    /// <param name="zoneId">The zone the app is in.</param>
    /// <param name="appId">The AppID of the app you need</param>
    /// <param name="unusedButKeepForApiStability">
    /// Tells the App that you'll be using page-publishing.
    /// So changes will me auto-drafted for a future release as the whole page together.
    /// </param>
    /// <param name="showDrafts">Show draft items - usually false for visitors, true for editors/admins.</param>
    /// <param name="parentLog">optional logger to attach to</param>
    /// <returns>An initialized App object which you can use to access App.Data</returns>
    [Obsolete]
    public static IApp App(int zoneId, int appId, bool unusedButKeepForApiStability = false, bool showDrafts = false, ILog parentLog = null)
        => throw new NotSupportedException(GenerateMessage());

    /// <summary>
    /// Get a full app-object for accessing data of the app from outside
    /// </summary>
    /// <param name="appId">The AppID of the app you need</param>
    /// <param name="ownerPortalSettings">The owner portal - this is important when retrieving Apps from another portal.</param>
    /// <param name="unusedButKeepForApiStability">
    /// Tells the App that you'll be using page-publishing.
    /// So changes will me auto-drafted for a future release as the whole page together.
    /// </param>
    /// <param name="showDrafts">Show draft items - usually false for visitors, true for editors/admins.</param>
    /// <param name="parentLog">optional logger to attach to</param>
    /// <returns>An initialized App object which you can use to access App.Data</returns>
    [Obsolete]
    public static IApp App(int appId,
        PortalSettings ownerPortalSettings,
        bool unusedButKeepForApiStability = false,
        bool showDrafts = false,
        ILog parentLog = null)
        => throw new NotSupportedException(GenerateMessage());
}