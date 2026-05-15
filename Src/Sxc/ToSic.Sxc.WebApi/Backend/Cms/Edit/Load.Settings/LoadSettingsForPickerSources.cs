using ToSic.Eav.Data.Sys.ContentTypes;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Backend.Cms.Load.Settings;

/// <summary>
/// Load additional content-type definitions which are used by pickers.
/// This is so the UI can determine the best names for the create-new buttons.
/// </summary>
internal class LoadSettingsForPickerSources() : LoadSettingsProviderBase($"{SxcLogName}.LdPikS"), ILoadSettingsContentTypesProvider
{
    public List<IContentType> GetContentTypes(LoadSettingsProviderParameters parameters)
    {
        var l = Log.Fn<List<IContentType>>();

        // Find all attributes which show a picker
        var pickerAttributes = parameters.ContentTypes
            .SelectMany(ct => ct.Attributes.Where(a => a.IsPicker()))
            .ToListOpt();

        if (pickerAttributes.Count == 0)
            return l.Return([], "no picker fields");

        // For each picker, find all the data-sources.
        // Normally just one, but data model allows many.
        var pickerSources = pickerAttributes
            .SelectMany(a => a.GetPickerDataSources()
                // Find all entities which define a data-source, then Flatten and remember the attribute for debugging
                .Select(ds => new
                {
                    Attribute = a,
                    DataSource = ds
                }))
            .ToListOpt();

        // Find all the NameIds which the DataSource says it can create
        var typeNames = pickerSources
            .Select(p => p.DataSource)
            .GetPickerCreateTypeNames();

        // Look up the types in the app-state
        var appReader = parameters.ContextOfApp.AppReaderRequired;
        var types = typeNames
            // Do distinct first, no eliminate duplicate keys - like when there are many pickers with the same create-new-type
            .DistinctBy(s => s.ToLowerInvariant())
            .Select(nameId => new
            {
                NameId = nameId,
                Type = appReader.TryGetContentType(nameId)
            })
            .Where(t => t.Type != null)
            .ToListOpt();

        return types.Count == 0
            ? l.Return([], "no types to enable create")
            : l.Return(types.Select(t => t.Type!).ToList(), $"{types.Count}");
    }
}