using System.IO;
using System.Xml.Linq;
using ToSic.Eav.Apps.Internal;
using ToSic.Eav.Data.Source;
using ToSic.Eav.Identity;
using ToSic.Eav.ImportExport.Internal;
using ToSic.Eav.ImportExport.Internal.Zip;
using ToSic.Eav.ImportExport.Json;
using ToSic.Eav.Integration.Environment;
using ToSic.Eav.Internal.Configuration;
using ToSic.Eav.Internal.Features;
using ToSic.Eav.Persistence.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Serialization.Internal;
using ToSic.Eav.WebApi.Assets;
using ToSic.Eav.WebApi.Validation;

namespace ToSic.Sxc.Backend.ImportExport;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class ImportContent(
    IEnvironmentLogger envLogger,
    LazySvc<ImportService> importerLazy,
    LazySvc<XmlImportWithFiles> xmlImportWithFilesLazy,
    ZipImport zipImport,
    Generator<JsonSerializer> jsonSerializerGenerator,
    IGlobalConfiguration globalConfiguration,
    IAppReaderFactory appReaders,
    LazySvc<IUser> userLazy,
    AppCachePurger appCachePurger,
    LazySvc<IEavFeaturesService> features)
    : ServiceBase("Bck.Export",
        connect:
        [
            envLogger, importerLazy, xmlImportWithFilesLazy, zipImport, jsonSerializerGenerator, globalConfiguration,
            appReaders, appCachePurger, userLazy, features
        ])
{

    protected readonly AppCachePurger AppCachePurger = appCachePurger;

    public ImportResultDto Import(int zoneId, int appId, string fileName, Stream stream, string defaultLanguage) => Log.Func(l =>
    {
        var result = new ImportResultDto();

        var allowSystemChanges = userLazy.Value.IsSystemAdmin;
        if (fileName.EndsWith(".zip"))
        {   // ZIP
            try
            {
                zipImport.Init(zoneId, appId, userLazy.Value.IsSystemAdmin);
                var temporaryDirectory = Path.Combine(globalConfiguration.TemporaryFolder, Mapper.GuidCompress(Guid.NewGuid()).Substring(0, 8));

                result.Success = zipImport.ImportZip(stream, temporaryDirectory);
                result.Messages.AddRange(zipImport.Messages);
            }
            catch (Exception ex)
            {
                l.Ex(ex);
                envLogger.LogException(ex);
            }
        }
        else
        {
            // XML
            using var fileStreamReader = new StreamReader(stream);
            var xmlImport = xmlImportWithFilesLazy.Value.Init(defaultLanguage, allowSystemChanges);
            var xmlDocument = XDocument.Parse(fileStreamReader.ReadToEnd());
            result.Success = xmlImport.ImportXml(zoneId, appId, parentAppId: null /* not sure if we never have a parent here */, xmlDocument);
            result.Messages.AddRange(xmlImport.Messages);
        }
        return result;
    });


    public ImportResultDto ImportJsonFiles(int zoneId, int appId, List<FileUploadDto> files,
        string defaultLanguage
    ) => Log.Func($"{zoneId}, {appId}, {defaultLanguage}", l =>
    {
        try
        {
            // 0. Verify it's json etc.
            if (files.Any(file => !Json.IsValidJson(file.Contents)))
                throw new ArgumentException("a file is not json");

            // 1. Create content types
            var serializer = jsonSerializerGenerator.New().SetApp(appReaders.Get(new AppIdentity(zoneId, appId)));

            // 1.1 Deserialize json files
            var packages = files.ToDictionary(file => file.Name, file => serializer.UnpackAndTestGenericJsonV1(file.Contents));
            l.A($"Packages: {packages.Count}");

            // 1.2. Build content types
            var types = new List<IContentType>();
                
            var isEnabled = features.Value.IsEnabled(BuiltInFeatures.DataExportImportBundles);
            l.A($"Is bundle packages import feature enabled: {isEnabled}");

            foreach (var package in packages)
            {
                l.A($"import content-types from package: {package.Key}");

                // bundle json
                if (isEnabled && package.Value.Bundles.SafeAny())
                    types.AddRange(serializer.GetContentTypesFromBundles(package.Value).Select(set => set.ContentType));

                // single json
                if (package.Value.ContentType != null)
                    types.Add(serializer.ConvertContentType(package.Value).ContentType);
            }

            if (types.Any(t => t == null))
                throw new NullReferenceException("One ContentType is null, something is wrong");

            // 1.3 Import the type
            var import = importerLazy.Value.Init(zoneId, appId, true, true);
            if (types.Any())
            {
                import.ImportIntoDb(types, null);

                l.A($"Purging {zoneId}/{appId}");
                AppCachePurger.Purge(zoneId, appId);
            }

            // are there any entities from bundles for import?
            if (!isEnabled || packages.All(p => p.Value.Bundles?.Any(b => b.Entities.SafeAny()) != true))
                return (new(true), "ok (types only)");

            // 2. Create Entities

            // 2.1 Reset serializer to use the new app
            var appState = appReaders.Get(new AppIdentity(zoneId, appId));
            serializer = jsonSerializerGenerator.New().SetApp(appState);
            l.A("Load items");

            // 2.2. Build content types
            var entities = new List<IEntity>();
            DirectEntitiesSource.Using(relationships =>
            {
                foreach (var package in packages)
                {
                    l.A($"import entities from package: {package.Key}");
                    if (package.Value.Bundles.SafeNone()) continue;
                    // bundle json
                    var entitiesFromBundles = serializer.GetEntitiesFromBundles(package.Value, relationships.Source);
                    l.A($"entities from bundles: {entitiesFromBundles.Count}");
                    entities.AddRange(entitiesFromBundles);
                    relationships.List.AddRange(entitiesFromBundles);
                }
                return "dummy";
            });

            if (entities.Any(t => t == null))
                throw new NullReferenceException("One Entity is null, something is wrong");

            // 2.3 Import the entities
            l.A($"Load entity {entities.Count} items");
            if (entities.Any())
            {
                import.ImportIntoDb(null, entities.Cast<Entity>().ToList());

                l.A($"Purging {zoneId}/{appId}");
                AppCachePurger.Purge(zoneId, appId);
                    
                //foreach (var entity in entities)
                //    appState.Add(entity as Entity, null, true);
            }

            // 3. possibly show messages / issues
            return (new(true), "ok (with entities)");
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            envLogger.LogException(ex);
            return (new ImportResultDto(false, ex.Message, Message.MessageTypes.Error), "error");
        }
    });
}