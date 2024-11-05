using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Backend.Cms;

/// <summary>
/// Load additional content-type definitions which are used by pickers.
/// This is so the UI can determine the best names for the create-new buttons.
/// </summary>
internal class LoadSettingsForPickerSources() : LoadSettingsProviderBase($"{SxcLogName}.LdPikS"), ILoadSettingsContentTypesProvider
{
    public static string[] PickerNames = ["entity-picker", "string-picker"];
    
    public List<IContentType> GetContentTypes(LoadSettingsProviderParameters parameters)
    {
        var l = Log.Fn<List<IContentType>>();

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
            .Select(p => p.DataSource.Get<string>(nameof(IUiPickerSourceEntity.CreateTypes), languages: []))
            .Where(s => s.HasValue())
            // TODO: INFO @SDV - he probably has comma separated values
            .SelectMany(s => s
                // TODO!!! NEW-LINES seem to be saved wrong!
                .Replace("\\n", "\n")
                .LinesToArrayWithoutEmpty())
            .ToList();

        // Look up the types in the app-state
        var typesToEnableCreate = createTypes
            // Do distinct first, no eliminate duplicate keys - like when there are many pickers with the same create-new-type
            .DistinctBy(s => s.ToLowerInvariant())
            .Select(nameId => new
            {
                NameId = nameId,
                Type = parameters.ContextOfApp.AppReader.GetContentType(nameId)
            })
            .Where(t => t.Type != null)
            .ToList();

        if (typesToEnableCreate.Count == 0) return l.Return([], "no types to enable create");

        var typesOnly = typesToEnableCreate.Select(t => t.Type).ToList();
        return l.Return(typesOnly, $"{typesOnly.Count}");
    }
}