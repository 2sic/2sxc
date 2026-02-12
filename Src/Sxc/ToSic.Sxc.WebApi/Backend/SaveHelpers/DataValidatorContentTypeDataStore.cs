using System.Net;
using ToSic.Eav.Data.ContentTypes;
using ToSic.Eav.Data.Processing;
using ToSic.Eav.Metadata;
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
    internal async Task<(IEntity? Entity, IDataProcessor? Processor, HttpExceptionAbstraction? Exception)> PreSave(int index, IEntity ent)
    {
        var l = Log.Fn<(IEntity?, IDataProcessor?, HttpExceptionAbstraction?)>();
        
        // Check if Save is disabled because of content-type metadata (new v21)
        // This should prevent entities from being put in the DB, where the UI was only meant for some other configuration
        var sharedWork = await Shared(index, ent);
        
        // Preprocessor exists, and supports pre-saving, so execute it
        if (sharedWork.Exception != null || sharedWork.Processor is not IDataProcessorPreSave preSave)
            return l.Return((ent, sharedWork.Processor, null), "from shared");


        var result = await preSave.Process(ent);
        var exception = HttpExceptionAbstraction.FromPossibleException(result.Exception, HttpStatusCode.Forbidden);
        return l.Return((result.Data, sharedWork.Processor, exception), $"pre-save, {(exception != null ? "with exception" : "")}");

    }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    private async Task<(IEntity? Entity, IDataProcessor? Processor, HttpExceptionAbstraction? Exception)> Shared(int index, IEntity ent)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
        var l = Log.Fn<(IEntity?, IDataProcessor?, HttpExceptionAbstraction?)>();

        // Check if Save is disabled because of content-type metadata (new v21)
        // This should prevent entities from being put in the DB, where the UI was only meant for some other configuration
        var ct = ent.Type;
        var storageDecorator = ct.TryGetMetadata<DataStorageDecorator>();

        if (storageDecorator == null)
            return l.Return((ent, null, null), "no decorator");

        if (storageDecorator.SaveIsDisabled)
            return l.Return((ent, null, BuildExceptionIfHasIssues($"Saving is disabled for content-type {ct.Name} (id: {ct.Id})", l)), "save disabled!");

        if (string.IsNullOrEmpty(storageDecorator.DataProcessingHandler))
            return l.Return((ent, null, null), "no data processing handler");


        // generate an object of the specified type name
        var dph = storageDecorator.DataProcessingHandler;
        var dataProcessingType = AssemblyHandling.GetTypeOrNull(dph); // Type.GetType(dph, throwOnError: false);

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

        return l.Return((ent, dataProcessor, null), "no pre-save");

        (IEntity?, IDataProcessor?, HttpExceptionAbstraction?) AsError(string msg) =>
            (ent, null, BuildExceptionIfHasIssues($"Data processing handler '{dph}' {msg} for content-type {ct.Name} (id: {ct.Id})", l));
    }
}