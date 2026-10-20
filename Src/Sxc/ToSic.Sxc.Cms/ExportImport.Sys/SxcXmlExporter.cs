//using ToSic.Eav.Apps;
//using ToSic.Eav.Context;
//using ToSic.Eav.ImportExport.Sys;
//using ToSic.Eav.ImportExport.Sys.Xml;
//using ToSic.Eav.ImportExport.Sys.XmlExport;

//namespace ToSic.Sxc.ExportImport.Sys;

///// <summary>
///// Xml Exporter which is aware of the Sxc context, and can therefore export apps and entities.
///// </summary>
//public abstract class SxcXmlExporter(XmlSerializer xmlSerializer, IAppsCatalog appsCatalog, IContextOfAppFactory appCtxFactory, string logPrefix, object[]? connect = default)
//    : XmlExporter(xmlSerializer, appsCatalog, logPrefix, connect: [..connect ?? [], appCtxFactory])
//{
//    /// <summary>
//    /// Not that the overload of this must take care of creating the EavAppContext and calling the Constructor
//    /// </summary>
//    /// <returns></returns>
//    public override XmlExporter Init(AppExportSpecs specs, IAppReader appReader, bool appExport, string[] attrSetIds, string[] entityIds)
//    {
//        //currentContextService.SetApp(specs);
//        //var ctxOfApp = currentContextService.AppContextRequired();
//        var ctxOfApp = appCtxFactory.Create(specs);
//        PostContextInit(ctxOfApp);
//        Constructor(specs, appReader, appExport, attrSetIds, entityIds, ctxOfApp.Site.DefaultCultureCode);

//        return this;
//    }

//    /// <summary>
//    /// Post context init the caller must be able to init Adam, which is not part of this project, so we're handling it as a callback
//    /// </summary>
//    /// <param name="appContext"></param>
//    protected abstract void PostContextInit(IContextOfApp appContext);

//}
