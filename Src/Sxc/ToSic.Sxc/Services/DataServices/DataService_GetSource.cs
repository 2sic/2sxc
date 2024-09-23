using ToSic.Eav;
using ToSic.Eav.Context;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.Internal.Errors;
using static ToSic.Eav.Data.DataConstants;


namespace ToSic.Sxc.Services.DataServices;

internal partial class DataService
{

    // IMPORTANT - this is different! from the _DynCodeRoot - as it should NOT auto attach!
    public T GetSource<T>(
        NoParamOrder noParamOrder = default,
        IDataSourceLinkable attach = default,
        object parameters = default,
        object options = default) where T : IDataSource
    {
        var l = Log.Fn<T>($"{nameof(attach)}: {attach}, {nameof(options)}: {options}");

        // If no in-source was provided, make sure that we create one from the current app
        var fullOptions = OptionsMs.SafeOptions(parameters, options: options);
        var ds = dataSources.Value.Create<T>(attach: attach, options: fullOptions);

        return l.Return(ds);
    }

    public IDataSource GetSource(
        NoParamOrder noParamOrder = default,
        string name = null,
        IDataSourceLinkable attach = null,
        object parameters = default,
        object options = null,
        bool? debug = default
    )
    {
        var l = Log.Fn<IDataSource>($"{nameof(name)}: {name}, {nameof(attach)}: {attach}, {nameof(options)}: {options}");

        // Do this first, to ensure AppIdentity is really known/set
        var safeOptions = OptionsMs.SafeOptions(parameters, options: options);
        var appId = safeOptions.AppIdentityOrReader.AppId;

        var dsInfo = catalog.Value.FindDataSourceInfo(name, appId);
        if (dsInfo == null)
            throw new ArgumentException($"Tried to create DataSource with name '{name}' but it was not found. " +
                                        "Either you a) mis-typed it, " +
                                        $"b) it's not located in the 'DataSources' folder of the app '{appId}', " +
                                        $"c) the class name is not 'public class {name}', " +
                                        $"d) the file name is not 'DataSources/{name}.cs'. ");

        // Decide if we should show errors or not
        var showErrors = debug == true || (user.IsSystemAdmin && debug != false);
        if (dsInfo.ErrorOrNull != null && showErrors)
            throw l.Done(DevException(name, "a compile error",
                "It could also be that the file name and class names don't match. \n" +
                $"Title: '{dsInfo.ErrorOrNull.Title}'; \n" +
                $"Message: {dsInfo.ErrorOrNull.Message}; \n" +
                $"Debug Info: {ErrorDebugMessage}"));

        var ds = dataSources.Value.Create(dsInfo.Type, attach: attach, options: safeOptions);

        // If it's the super user (often developing the DS) we should show errors instead of letting it just happen
        if (!showErrors || !ds.IsError())
            return l.Return(ds);

        // Work out more information about the error
        var errEntity = ds.List.First();
        var message = $"Title: '{errEntity.Get<string>(ErrorFieldTitle)}'; \n" +
                      $"Message: {errEntity.Get<string>(ErrorFieldMessage)}; \n" +
                      $"Debug Info: {errEntity.Get<string>(ErrorFieldDebugNotes)}";
        throw l.Done(DevException(name, "a bug in the code", message));

    }

    private static Exception DevException(string name, string reason, string more = default)
    {
        var intro =  $"The DataSource {name} threw an error. Probably: '{reason}'. " +
                     "Normally this would be invisible and just return a first item with the error message, " +
                     $"but because you are logged in as {nameof(IUser.IsSystemAdmin)} the error is shown directly. " +
                     "If you wish to disable this, set 'debug: false'. " +
                     "These are the error details: \n" +
                     more;
        return new ExceptionSuperUserOnly(new(intro));
    }
}