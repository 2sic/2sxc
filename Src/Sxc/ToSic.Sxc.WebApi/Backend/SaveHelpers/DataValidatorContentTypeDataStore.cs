using System.Net;
using ToSic.Eav.Data.ContentTypes;
using ToSic.Eav.Data.Processing;
using ToSic.Eav.Metadata.Sys;
using ToSic.Eav.Models;
using ToSic.Sys.HookUp;
using ToSic.Sys.Users;
using ToSic.Sys.Utils.Types;
using static ToSic.Eav.WebApi.Sys.Helpers.Validation.ValidatorBase;

namespace ToSic.Sxc.Backend.SaveHelpers;

[PrivateApi]
public class DataValidatorContentTypeDataStore(IServiceProvider sp, RemoteWork<WorkEntityBlockUsers, PermissionCheckPayload, IEntity?> blockUser) : ServiceBase("Val.DtStor")
{

    /// <summary>
    /// Check if entity was able to deserialize, and if it has attributes.
    /// In rare cases, no-attributes are allowed, but this requires metadata decorators to allow it.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="ent"></param>
    /// <returns></returns>
    internal async Task<Result> PreEdit(int index, IEntity ent) =>
        await RunProcessorsFromDecorator(index, ent, DataProcessingEvents.PreEdit);

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
        var result = await RunProcessorsFromDecorator(index, ent, DataProcessingEvents.PreSave);

        // Preprocessor exists, and supports pre-saving, so execute it
        if (result.Exception != null)
            return l.Return(result, "error from shared");

        // If we have a decorator, check if it forbids saving.
        // For example for Debug-Settings which should never hit the backend
        if (result.Decorator?.SaveIsDisabled == true)
            return l.Return(new(
                    result.Entity,
                    result.Decorator,
                    BuildExceptionIfHasIssues($"Save is disabled for content-type {ent.Type.Name} (index: {index})", l)),
                "save disabled!"
            );

        return l.Return(result);

    }

    /// <summary>
    /// RunProcessorsFromDecorator code
    /// </summary>
    /// <param name="index"></param>
    /// <param name="ent"></param>
    /// <param name="action"></param>
    /// <returns></returns>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    private async Task<Result> RunProcessorsFromDecorator(int index, IEntity ent, string action)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        var l = Log.Fn<Result>($"action: {action}");

        // Check if Save is disabled because of content-type metadata (new v21)
        // This should prevent entities from being put in the DB, where the UI was only meant for some other configuration
        var ct = ent.Type;
        var decorator = ct.GetMetadataModel<DataStorageDecorator>();

        if (decorator == null)
            return l.Return(new (ent, decorator), "no decorator");

        // Try to build the instance using dependency injection
        // will return a message if empty, not valid, etc.
        var (_, dataProcessor, message) = sp.BuildByName<IWorkEntityAction>(decorator.DataProcessingHandler);
        if (dataProcessor == null)
            return l.Return(AsError(message), message);

        // Preprocessor exists, and supports pre-save and post-save, so execute it
        var result = await dataProcessor.Handle(new(), new(new PermissionCheckPayload(action, ent, UserElevation.SiteAdmin)));
        var exception = HttpExceptionAbstraction.FromPossibleException(result.Exceptions.FirstOrDefault(), HttpStatusCode.Forbidden);
        return l.Return(new(result.Data, decorator, exception, dataProcessor), $"action: {action}, {(exception != null ? "with exception" : "")}");


        Result AsError(string msg) =>
            new(ent, decorator, BuildExceptionIfHasIssues(
                $"Data processing handler '{decorator.DataProcessingHandler}' {msg} for content-type {ct.Name} (id: {ct.Id})", l));
    }
    

    public record Result(
        IEntity? Entity,
        DataStorageDecorator? Decorator,
        HttpExceptionAbstraction? Exception = null,
        IWorkEntityAction? Processor = null
    );
}