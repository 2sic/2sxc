using ToSic.Eav.Data.Sys;
using ToSic.Eav.Models;

namespace ToSic.Sxc.Backend.Cms.Load.Settings;

internal abstract class LoadSettingsForBase(string logName, object[]? connect = default)
    : ServiceBase(logName, connect: connect), ILoadSettingsProvider
{
    public abstract Dictionary<string, object> GetSettings(LoadSettingsProviderParameters parameters);

    public Dictionary<string, object> GetSettings<TData, TModel>(
        LoadSettingsProviderParameters parameters,
        TData defaults,
        bool forceDefaults,
        string lookupPath,
        string outputPath,
        Func<TModel, TData> modelConverter)
        where TModel : class, IModelFromEntity
    {
        var l = Log.Fn<Dictionary<string, object>>();
        var data = defaults;

        if (!forceDefaults)
        {
            var getSettings = parameters.ContextOfApp.AppSettings.InternalGetPath(lookupPath);
            if (getSettings?.GetFirstResultEntity() is { } mapsEntity)
            {
                var model = mapsEntity.ToModel<TModel>();
                if (model != null)
                    data = modelConverter(model);
            }
        }

        var result = new Dictionary<string, object>
        {
            [outputPath] = data!,
        };
        return l.Return(result, $"{result.Count}");
    }
}