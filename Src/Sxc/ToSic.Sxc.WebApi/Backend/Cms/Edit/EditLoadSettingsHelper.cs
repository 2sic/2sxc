using System.Collections;
using ToSic.Eav.ImportExport.Json;
using ToSic.Eav.ImportExport.Json.V1;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Serialization.Internal;
using static System.StringComparer;

namespace ToSic.Sxc.Backend.Cms;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class EditLoadSettingsHelper(
    Generator<JsonSerializer> jsonSerializerGenerator,
    IEnumerable<ILoadSettingsProvider> loadSettingsProviders,
    IEnumerable<ILoadSettingsContentTypesProvider> loadSettingsTypesProviders,
    GenWorkPlus<WorkEntities> appEntities)
    : ServiceBase(SxcLogName + ".LodSet",
        connect: [jsonSerializerGenerator, loadSettingsProviders, appEntities])
{
    /// <summary>
    /// WIP v15.
    /// Later it should be built using a list of services that provide settings to the UI.
    /// - put gps coordinates in static
    /// - later get from settings
    /// </summary>
    /// <returns></returns>
    public EditSettingsDto GetSettings(IContextOfApp contextOfApp, List<IContentType> contentTypes, List<JsonContentType> jsonTypes, IAppWorkCtxPlus appWorkCtx)
    {
        var l = Log.Fn<EditSettingsDto>();
        var allInputTypes = jsonTypes
            .SelectMany(ct => ct.Attributes
                .Select(at => at.InputType)
            )
            .Distinct()
            .ToList();

        var lspParameters = new LoadSettingsProviderParameters
        {
            ContextOfApp = contextOfApp,
            ContentTypes = contentTypes,
            InputTypes = allInputTypes
        };

        var settings = new EditSettingsDto
        {
            Values = GetOrEmptyOnError(() => GetValues(lspParameters), () => nameof(GetValues)),
            Entities = GetSettingsEntities(appWorkCtx, allInputTypes),
            ContentTypes = GetOrEmptyOnError(() => GetContentTypes(lspParameters), () => nameof(GetContentTypes))
        };
        return l.Return(settings);
    }

    private TList GetOrEmptyOnError<TList>(Func<TList> getList, Func<string> errMessage) where TList : class, IEnumerable, new()
    {
        var l = Log.Fn<TList>();
        try
        {
            return l.ReturnAsOk(getList());
        }
        catch (Exception e)
        {
            l.E($"Error: {errMessage()}");
            l.Ex(e);
            return l.ReturnAsError(new());
        }
    }

    private Dictionary<string, object> GetValues(LoadSettingsProviderParameters lspParameters)
    {
        var l = Log.Fn<Dictionary<string, object>>();

        // Get all settings from all providers
        var settingsFromProviders = loadSettingsProviders
            .Select(lsp => GetOrEmptyOnError(
                () => lsp.LinkLog(Log).GetSettings(lspParameters),
                () => $"Error on {lsp.GetType().Name}")
            )
            .ToList();

        // Merge all settings into one dictionary
        var finalSettings = new Dictionary<string, object>(InvariantCultureIgnoreCase);
        foreach (var pair in settingsFromProviders.SelectMany(sfp => sfp))
            finalSettings[pair.Key] = pair.Value;
        return l.Return(finalSettings, $"{finalSettings.Count}");
    }

    public List<JsonContentTypeWithTitleWip> GetContentTypes(LoadSettingsProviderParameters parameters)
    {
        var l = Log.Fn<List<JsonContentTypeWithTitleWip>>();

        // Load all types from the providers
        var typesFromProviders = loadSettingsTypesProviders
            .SelectMany(lsp => GetOrEmptyOnError(
                () => lsp.LinkLog(Log).GetContentTypes(parameters),
                () => $"Error GetContentTypes of {lsp.GetType().Name}")
            )
            .ToList();

        // Setup Type Serializer - same as EditLoadBackend
        var serializerForTypes = jsonSerializerGenerator.New().SetApp(parameters.ContextOfApp.AppReader);
        var serSettings = new JsonSerializationSettings
        {
            CtIncludeInherited = true,
            CtAttributeIncludeInheritedMetadata = true,
            CtWithEntities = false,
        };


        var nameMap = typesFromProviders
            .Select(t =>
            {
                var normal = serializerForTypes.ToPackage(t, serSettings).ContentType;
                var title = t.Metadata.DetailsOrNull?.Title;
                return new JsonContentTypeWithTitleWip()
                {
                    Id = normal.Id,
                    Name = normal.Name,
                    Scope = normal.Scope,
                    Sharing = normal.Sharing,
                    Metadata = normal.Metadata,
                    Attributes = normal.Attributes,
                    Assets = normal.Assets,
                    Title = title.NullIfNoValue() ?? normal.Name,
                };
            })
            .ToList();

        return l.Return(nameMap, $"all ok, found {nameMap.Count}");
    }



    private List<JsonEntity> GetSettingsEntities(IAppWorkCtxPlus appWorkCtx, IEnumerable<string> allInputTypes)
    {
        var l = Log.Fn<List<JsonEntity>>();
        try
        {
            var hasWysiwyg = allInputTypes.Any(it => it.ContainsInsensitive("wysiwyg"));
            if (!hasWysiwyg)
                return l.Return([], "no wysiwyg field");

            var entities = appEntities.New(appWorkCtx)
                .GetWithParentAppsExperimental("StringWysiwygConfiguration")
                .ToList();

            var jsonSerializer = jsonSerializerGenerator.New().SetApp(appWorkCtx.AppReader);
            var result = entities.Select(e => jsonSerializer.ToJson(e)).ToList();

            return l.Return(result, $"{result.Count}");
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            return l.Return([], "error");
        }
    }
        
}