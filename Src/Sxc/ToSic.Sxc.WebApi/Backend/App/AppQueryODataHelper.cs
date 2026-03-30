using ToSic.Eav.DataFormats.EavLight;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.OData;
using ToSic.Eav.DataSource.Sys.Convert;
using ToSic.Eav.DataSources;
using ToSic.Eav.Services;
using ToSic.Sxc.Data.Sys.Convert;
using ToSic.Sys.OData;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Backend.App;

/// <summary>
/// Helper to apply odata to a query.
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class AppQueryODataHelper(Generator<IConvertToEavLight> dataConverter, IDataSourcesService dataSourcesService)
    : ServiceBase("Sxc.ApiApQ", connect: [dataConverter, dataSourcesService])
{


    internal IDictionary<string, IEnumerable<EavLightEntity>> ApplyOData(IDataSource query, IDictionary<string, ODataOptions> streams, string[]? filterGuids, bool withGuid = false, bool withEdit = false)
    {
        var l = Log.Fn<IDictionary<string, IEnumerable<EavLightEntity>>>();
        var oDataEngine = new ODataQueryEngine(dataSourcesService);

        var guidFilter = DataSourceConvertHelper.SafeParseGuidList(filterGuids);

        // only apply odata to the "Default" stream or the first one.
        var streamToFilter = streams.ContainsKey(DataSourceConstants.StreamDefaultName)
            ? DataSourceConstants.StreamDefaultName
            : streams.Keys.First();

        var filtered = streams
            .Select(stream =>
            {
                var streamName = stream.Key;
                var sourceStream = query.GetStream(streamName, nullIfNotFound: true);

                // Null-check - not really expected, but just in case...
                if (sourceStream == null)
                {
                    l.A($"Stream '{streamName}' not found, skip OData.");
                    return (streamName, []);
                }

                // If it's not the one to apply OData to, exit here.
                if (!streamName.EqualsInsensitive(streamToFilter))
                    return (name: streamName, list: PrepareConverter(stream.Value).Convert(sourceStream));

                // Apply OData to this stream
                // For the internal processing, we need it to be in an IDataSource
                var oDataQuery = stream.Value.ToQuery();
                var wrapper = dataSourcesService.Create<PassThrough>(sourceStream);
                var execution = oDataEngine.Execute(wrapper, oDataQuery);
                var entities = guidFilter.Any()
                    ? execution.Items.Where(e => guidFilter.Contains(e.EntityGuid))
                    : execution.Items;

                // The filtered OData path must still honor that stream's $select,
                // otherwise adding $filter/$orderby/$top causes the selected fields to be lost.
                var converted = PrepareConverter(stream.Value).Convert(entities);
                return (name: streamName, list: converted);
            })
            .Where(pair => pair.list != null)
            .ToDictionary(
                kvp => kvp.name,
                kvp => kvp.list,
                StringComparer.OrdinalIgnoreCase
            );
        return l.Return(filtered!);

        IConvertToEavLight PrepareConverter(ODataOptions options)
        {
            var converter = dataConverter.New();
            converter.WithGuid = withGuid;
            if (converter is ConvertToEavLightWithCmsInfo serializerWithEdit)
                serializerWithEdit.WithEdit = withEdit;
            if (converter is ConvertToEavLight serializerWithOData)
                serializerWithOData.AddSelectFields(options.Select.ToListOpt());
            return converter;
        }

        // Old, not functional, not ideal
        //var results = new Dictionary<string, IEnumerable<EavLightEntity>>(StringComparer.OrdinalIgnoreCase);

        //foreach (var streamName in streams)
        //{
        //    var sourceStream = query.GetStream(streamName, nullIfNotFound: true);
        //    if (sourceStream == null)
        //    {
        //        l.A($"Stream '{streamName}' not found, skip OData.");
        //        continue;
        //    }

        //    var wrapper = dataSourcesService.Create<PassThrough>(sourceStream);
        //    var execution = engine.Execute(wrapper, oDataQuery);
        //    var entities = guidFilter.Any()
        //        ? execution.Items.Where(e => guidFilter.Contains(e.EntityGuid))
        //        : execution.Items;

        //    results[streamName] = dataConverter.Convert(entities);
        //}

        //return l.Return(results);
    }
}
