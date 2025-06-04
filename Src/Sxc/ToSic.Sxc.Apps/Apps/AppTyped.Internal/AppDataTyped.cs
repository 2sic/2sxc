using ToSic.Eav.Apps.Internal.Api01;
using ToSic.Eav.Data.Entities.Sys.Lists;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.Internal.Caching;
using ToSic.Eav.DataSources.Internal;
using ToSic.Sxc.Data.Internal;

namespace ToSic.Sxc.Apps.Internal;

internal class AppDataTyped(
    Eav.DataSources.App.MyServices services,
    LazySvc<SimpleDataEditService> dataController,
    LazySvc<IDataSourceCacheService> dsCacheSvc)
    : AppDataWithCrud(services, dataController, dsCacheSvc), IAppDataTyped
{
    #region Get Content Types - explicit implementation to ensure it's only available in Typed APIs

    IEnumerable<IContentType> IAppDataTyped.GetContentTypes()
        => AppReader.ContentTypes;

    IContentType IAppDataTyped.GetContentType(string name)
        => AppReader.GetContentType(name);

    #endregion

    #region Kit Attachments

    internal AppDataTyped Setup(ICodeDataFactory cdfConnected)
    {
        CdfConnected = cdfConnected;
        return this;
    }

    private ICodeDataFactory CdfConnected
    {
        get => field ?? throw new(nameof(CdfConnected) + " not set");
        set;
    }

    #endregion

    /// <inheritdoc />
    IEnumerable<T> IAppDataTyped.GetAll<T>(NoParamOrder protector, string typeName, bool nullIfNotFound)
    {
        //var streamName = typeName ?? DataModelAnalyzer.GetStreamName<T>();
        //var errStreamName = streamName;

        var streamNames = typeName == null
            ? DataModelAnalyzer.GetStreamNameList<T>()
            : ([typeName], typeName);

        // Get the list - will be null if not found
        IDataStream list = null;
        foreach (var streamName2 in streamNames.List)
            list ??= GetStream(streamName2, nullIfNotFound: true);
        

        //// If we didn't find it, check if the stream name is *Model and try without that common suffix
        //if (list == null && streamName.EndsWith("Model"))
        //{
        //    var shorterName = streamName.Substring(0, streamName.Length - 5);
        //    errStreamName += "," + shorterName;
        //    list = GetStream(shorterName, nullIfNotFound: true);
        //}

        // If we didn't find anything yet, then we must now try to re-access the stream
        // but in a way which will throw an exception with the expected stream names
        if (list == null && !nullIfNotFound)
            list = GetStream(/*errStreamName*/streamNames.Flat, nullIfNotFound: false);

        return list == null
            ? null
            : CdfConnected.AsCustomList<T>(source: list, protector: protector, nullIfNull: nullIfNotFound);
    }

    /// <inheritdoc />
    T IAppDataTyped.GetOne<T>(int id, NoParamOrder protector, bool skipTypeCheck)
        => CdfConnected.GetOne<T>(() => List.One(id), id, skipTypeCheck);

    /// <inheritdoc />
    T IAppDataTyped.GetOne<T>(Guid id, NoParamOrder protector, bool skipTypeCheck)
        => CdfConnected.GetOne<T>(() => List.One(id), id, skipTypeCheck);
}