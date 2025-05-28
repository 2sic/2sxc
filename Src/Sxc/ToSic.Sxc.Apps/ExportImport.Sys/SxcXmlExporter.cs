using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Eav.Context.Internal;
using ToSic.Eav.ImportExport.Internal;
using ToSic.Eav.ImportExport.Internal.Xml;

namespace ToSic.Sxc.ExportImport.Sys;

/// <summary>
/// Xml Exporter which is aware of the Sxc context, and can therefore export apps and entities.
/// </summary>
/// <param name="xmlSerializer"></param>
/// <param name="appsCatalog"></param>
/// <param name="contextResolver"></param>
/// <param name="logPrefix"></param>
/// <param name="connect"></param>
public abstract class SxcXmlExporter(XmlSerializer xmlSerializer, IAppsCatalog appsCatalog, IContextResolver contextResolver, string logPrefix, object[] connect = default)
    : XmlExporter(xmlSerializer, appsCatalog, logPrefix, connect: [..connect ?? [], contextResolver])
{

    protected IContextResolver ContextResolver { get; } = contextResolver;

    /// <summary>
    /// Not that the overload of this must take care of creating the EavAppContext and calling the Constructor
    /// </summary>
    /// <returns></returns>
    public override XmlExporter Init(AppExportSpecs specs, IAppReader appRuntime, bool appExport, string[] attrSetIds, string[] entityIds)
    {
        ContextResolver.SetApp(new AppIdentity(specs.ZoneId, specs.AppId));
        var ctxOfApp = ContextResolver.AppRequired();
        PostContextInit(ctxOfApp);
        Constructor(specs, appRuntime, ctxOfApp.AppReader.Specs.NameId, appExport, attrSetIds, entityIds, ctxOfApp.Site.DefaultCultureCode);

        return this;
    }

    /// <summary>
    /// Post context init the caller must be able to init Adam, which is not part of this project, so we're handling it as a callback
    /// </summary>
    /// <param name="appContext"></param>
    protected abstract void PostContextInit(IContextOfApp appContext);

}
