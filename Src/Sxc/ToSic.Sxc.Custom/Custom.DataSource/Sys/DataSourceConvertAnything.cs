using System.Collections;
using System.Collections.Immutable;
using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.Raw;
using ToSic.Eav.Data.Raw.Sys;
using ToSic.Eav.DataSource;
using ToSic.Sys.Performance;
using ToSic.Sys.Utils;

namespace Custom.DataSource.Sys;

/// <summary>
/// Helper Service to convert data to IEntity for data sources.
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class DataSourceConvertAnything(Generator<IDataFactory, DataFactoryOptions> genDataFactory) : ServiceBase("Ds.Conv")
{

    internal IImmutableList<IEntity> ConvertAny(IDataSource parent, Func<object>? source, Func<DataFactoryOptions>? options)
    {
        var l = Log.Fn<IImmutableList<IEntity>>();

        // Call the Generator and handle errors/null
        object? funcResult;
        try
        {
            funcResult = source?.Invoke();
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            var runErr = parent.Error.Create(title: $"Error calling source generator of \"ProvideOut\". " +
                                                    "Error details can be found in Insights.", exception: ex);
            return l.ReturnAsError(runErr);
        }
        if (funcResult is null)
            return l.Return([], "null, no data returned");

        // Make a list out of the result
        List<object> data;
        try
        {
            data = funcResult is IEnumerable enumerable
                ? [.. enumerable.Cast<object>()]
                : [funcResult];
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            var runErr = parent.Error.Create(title: $"Error handling result of source generator of \"ProvideOut\". " +
                                                    "Error details can be found in Insights.", exception: ex);
            return l.ReturnAsError(runErr);
        }

        // Handle empty list
        if (data.SafeNone())
            return l.Return([], "no items returned");

        // Handle all is already converted to IEntity
        if (data.All(i => i is IEntity))
            return l.Return(data.Cast<IEntity>().ToImmutableOpt(), "IEntities");


        // If all are Anonymous, convert to Raw
        if (data.All(d => d.IsAnonymous()))
        {
            l.A("Was anonymous, converted to raw");
            var converter = new RawFromAnonymousHelper(Log);
            var rawFromAnon = data.Select(converter.Convert).ToList();
            var result = GetFactory(options).Create(rawFromAnon);
            return l.Return(result, "was anonymous, converted to RawEntity");
        }

        // Handle data is already IRawEntity
        if (data.All(i => i is IRawEntity))
        {
            var rawEntities = data.Cast<IRawEntity>().ToList();
            var result = GetFactory(options).Create(rawEntities);
            return l.Return(result, "was IRawEntity");
        }

        // todo - maybe also process IHasEntity - but only after doing the raw entities

        var err = parent.Error.Create(title: $"Error in \"ProvideOut\"",
            message: "The list received was tested against all possible data types but non matched. " +
                     $"Expected was a list of either {nameof(IEntity)}, {nameof(IRawEntity)} or anonymous objects. " +
                     "Note that all items must be of the same type. ");
        return l.ReturnAsError(err);
    }

    public IDataFactory GetFactory(Func<DataFactoryOptions>? options)
        => genDataFactory.New(options: options?.Invoke() ?? new());

}