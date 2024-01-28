using ToSic.Eav.ImportExport.Json;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Serialization.Internal;
using ToSic.Sxc.Internal;

namespace ToSic.Sxc.Backend.Cms;

/// <summary>
/// Load additional content-type definitions which are used by pickers.
/// This is so the UI can determine the best names for the create-new buttons.
/// </summary>
/// <param name="jsonSerializerGenerator"></param>
internal class LoadSettingsForPickerSources(Generator<JsonSerializer> jsonSerializerGenerator) : LoadSettingsProviderBase($"{SxcLogging.SxcLogName}.LdPikS"), ILoadSettingsProvider
{
    public static string[] PickerNames = ["entity-picker", "string-picker"];

    public Dictionary<string, object> GetSettings(LoadSettingsProviderParameters parameters)
    {
        var l = Log.Fn<Dictionary<string, object>>();

        // Find all attributes which show a picker
        var pickerAttributes = parameters.ContentTypes
            .SelectMany(ct => ct.Attributes
                .Where(a => PickerNames.Contains(a.InputType()))
            )
            .ToList();

        if (pickerAttributes.Count == 0) return l.Return([], "no picker fields");

        // For each picker, find all the data-sources.
        // Normally just one, but data model allows many.
        var pickerSources = pickerAttributes
            .SelectMany(a =>
            {
                // Find all entities which define a data-source
                var dsEntities = a.Metadata
                    .SelectMany(e => e.Children(nameof(IUiPicker.DataSources)));

                // Flatten and remember the attribute for debugging
                return dsEntities.Select(ds => new
                {
                    Attribute = a,
                    DataSource = ds
                });
            })
            .Where(ps => ps?.DataSource != null)
            .ToList();

        // Find all the NameIds which the DataSource says it can create
        var createTypes = pickerSources
            .Select(p => p.DataSource.GetBestValue<string>(nameof(IUiPickerSourceEntity.CreateTypes), []))
            .Where(s => s.HasValue())
            // TODO: INFO @SDV - he probably has comma separated values
            .SelectMany(s => s
                    // TODO!!! NEW-LINES seem to be saved wrong!
                .Replace("\\n", "\n")
                .LinesToArrayWithoutEmpty())
            .ToList();

        // Look up the types in the app-state
        var typesToEnableCreate = createTypes
            // Do distinct first, no eliminate duplicate keys - eg. when there are many pickers with the same create-new-type
            .DistinctBy(s => s.ToLowerInvariant())
            .Select(nameId => new
            {
                NameId = nameId,
                Type = parameters.ContextOfApp.AppState.GetContentType(nameId)
            })
            .Where(t => t.Type != null)
            .ToList();

        if (typesToEnableCreate.Count == 0) return l.Return([], "no types to enable create");

        try
        {
            // Setup Type Serializer - same as EditLoadBackend
            var serializerForTypes = jsonSerializerGenerator.New().SetApp(parameters.ContextOfApp.AppState);
            var serSettings = new JsonSerializationSettings
            {
                CtIncludeInherited = true,
                CtAttributeIncludeInheritedMetadata = true,
                CtWithEntities = false,
            };


            var nameMap = typesToEnableCreate.ToDictionary(
                t => t.NameId,
                t => serializerForTypes.ToPackage(t.Type, serSettings)
            );

            return l.Return(new() { ["PickerCreateTypes"] = nameMap }, $"all ok, found {nameMap.Count}");
        }
        catch (Exception e)
        {
            l.Ex(e);
            return l.Return([], "error serializing; skip");
        }

    }
}