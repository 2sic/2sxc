using System.Net;
using ToSic.Eav.Data.ContentTypes;
using ToSic.Eav.Data.Processing;
using ToSic.Eav.Metadata;
using ToSic.Eav.Models;
using ToSic.Sys.Utils.Assemblies;
using static ToSic.Eav.WebApi.Sys.Helpers.Validation.ValidatorBase;

namespace ToSic.Sxc.Backend.SaveHelpers;

[PrivateApi]
public class DataValidatorContentTypeDataStore(IServiceProvider sp) : ServiceBase("Val.DtStor")
{

    /// <summary>
    /// Check if entity was able to deserialize, and if it has attributes.
    /// In rare cases, no-attributes are allowed, but this requires metadata decorators to allow it.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="ent"></param>
    /// <returns></returns>
    internal async Task<Result> PreEdit(int index, IEntity ent) =>
        await Shared(index, ent, DataProcessingEvents.PreEdit);

    /// <summary>
    /// Check if entity was able to deserialize, and if it has attributes.
    /// In rare cases, no-attributes are allowed, but this requires metadata decorators to allow it.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="ent"></param>
    /// <returns></returns>
    internal async Task<Result> PreSave(int index, IEntity ent)
    {
        var l = Log.Fn<Result>();

        // Check if Save is disabled because of content-type metadata (new v21)
        // This should prevent entities from being put in the DB, where the UI was only meant for some other configuration
        var sharedWork = await Shared(index, ent, DataProcessingEvents.PreSave);

        // Preprocessor exists, and supports pre-saving, so execute it
        if (sharedWork.Exception != null)
            return l.Return(sharedWork, "error from shared");

        // If we have a decorator, check if it forbids saving.
        // For example for Debug-Settings which should never hit the backend
        if (sharedWork.Decorator?.SaveIsDisabled == true)
            return l.Return(new(sharedWork.Entity, sharedWork.Decorator, BuildExceptionIfHasIssues($"Save is disabled for content-type {ent.Type.Name} (index: {index})", l)), "save disabled!");

        return l.Return(sharedWork);

    }

    /// <summary>
    /// Shared code
    /// </summary>
    /// <param name="index"></param>
    /// <param name="ent"></param>
    /// <param name="action"></param>
    /// <returns></returns>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    private async Task<Result> Shared(int index, IEntity ent, string action)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        var l = Log.Fn<Result>($"action: {action}");

        // Check if Save is disabled because of content-type metadata (new v21)
        // This should prevent entities from being put in the DB, where the UI was only meant for some other configuration
        var ct = ent.Type;
        var decorator = ct.GetMetadata<DataStorageDecorator>();

        if (decorator == null)
            return l.Return(new (ent, decorator), "no decorator");

        if (string.IsNullOrEmpty(decorator.DataProcessingHandler))
            return l.Return(new (ent, decorator), "no data processing handler");


        // generate an object of the specified type name
        var dataProcessingType = AssemblyHandling.GetTypeOrNull(decorator.DataProcessingHandler);

        if (dataProcessingType == null)
            return l.Return(AsError("not found"), "data type results in null");

        // Check if the type is ok, before instantiating it, to avoid security issues with instantiating random types.
        // It must be a data processor, otherwise it is not valid for this purpose.
        if (!typeof(IDataProcessor).IsAssignableFrom(dataProcessingType))
            return l.Return(AsError("is not a valid data processor"), $"not assignable from {nameof(IDataProcessor)}");

        // Re-verify it's a dataProcessor and not null
        var probablyProcessor = sp.GetService(dataProcessingType);
        if (probablyProcessor is not IDataProcessor dataProcessor)
            return l.Return(AsError("could not be instantiated"), "Instantiated type null or wrong type");

        // Preprocessor exists, and supports pre-saving, so execute it
        var result = await dataProcessor.Process(action, new() { Data = ent });
        var exception = HttpExceptionAbstraction.FromPossibleException(result.Exceptions.FirstOrDefault(), HttpStatusCode.Forbidden);
        return l.Return(new(result.Data, null, exception), $"action: {action}, {(exception != null ? "with exception" : "")}");


        Result AsError(string msg) =>
            new(ent, decorator, BuildExceptionIfHasIssues(
                $"Data processing handler '{decorator.DataProcessingHandler}' {msg} for content-type {ct.Name} (id: {ct.Id})", l));
    }

    public record Result(
        IEntity? Entity,
        DataStorageDecorator? Decorator,
        HttpExceptionAbstraction? Exception = null,
        IDataProcessor? Processor = null
    );
}