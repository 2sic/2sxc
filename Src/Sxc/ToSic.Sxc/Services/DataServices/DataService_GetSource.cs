using System;
using System.Linq;
using ToSic.Eav.Context;
using ToSic.Eav.DataSource;
using ToSic.Lib.Logging;
using static ToSic.Eav.Data.DataConstants;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Services
{
    internal partial class DataService
    {

        // IMPORTANT - this is different! from the _DynCodeRoot - as it should NOT auto attach!
        public T GetSource<T>(
            string noParamOrder = Protector,
            IDataSourceLinkable attach = default,
            object parameters = default,
            object options = default) where T : IDataSource
        {
            var l = Log.Fn<T>($"{nameof(attach)}: {attach}, {nameof(options)}: {options}");
            Protect(noParamOrder, $"{nameof(attach)}, {nameof(parameters)}, {nameof(options)}");

            // If no in-source was provided, make sure that we create one from the current app
            var fullOptions = OptionsMs.SafeOptions(parameters, options: options);
            var ds = _dataSources.Value.Create<T>(attach: attach, options: fullOptions);

            return l.Return(ds);
        }

        public IDataSource GetSource(
            string noParamOrder = Protector,
            string name = null,
            IDataSourceLinkable attach = null,
            object parameters = default,
            object options = null,
            bool? debug = default
        )
        {
            var l = Log.Fn<IDataSource>($"{nameof(name)}: {name}, {nameof(attach)}: {attach}, {nameof(options)}: {options}");
            Protect(noParamOrder, $"{nameof(attach)}, {nameof(parameters)}, {nameof(options)}");
            // Decide if we should show errors or not
            var showErrors = debug == true || (_user.IsSystemAdmin && debug != false);

            // Do this first, to ensure AppIdentity is really known/set
            var safeOptions = OptionsMs.SafeOptions(parameters, options: options);
            var appId = safeOptions.AppIdentity.AppId;

            var dsInfo = _catalog.Value.FindDataSourceInfo(name, appId);
            if (dsInfo == null)
                throw new ArgumentException($"Tried to create DataSource with name '{name}' but it was not found. " +
                                            $"Either you a) mis-typed it, " +
                                            $"b) it's not located in the 'DataSources' folder of the app '{appId}', " +
                                            $"c) the class name is not 'public class {name}', " +
                                            $"d) the file name is not 'DataSources/{name}.cs'. ");

            if (dsInfo.ErrorOrNull != null && showErrors)
                throw l.Done(new Exception($"{ErrorIntro(name, "compile error")}\n" +
                                         $"It could also be that the file name and class names don't match. \n" +
                                         $"Title: '{dsInfo.ErrorOrNull.Title}'; \n" +
                                         $"Message: {dsInfo.ErrorOrNull.Message}; \n" +
                                         $"Debug Info: {ErrorDebugMessage}"));

            var ds = _dataSources.Value.Create(dsInfo.Type, attach: attach, options: safeOptions);

            // If it's the super user (often developing the DS) we should show any errors
            if (debug == false || !_user.IsSystemAdmin || !ds.IsError())
                return l.Return(ds);

            // Work out more information about the error
            var errEntity = ds.List.First();
            var message = $"Title: '{errEntity.Get<string>(ErrorFieldTitle)}'; \n" +
                          $"Message: {errEntity.Get<string>(ErrorFieldMessage)}; \n" +
                          $"Debug Info: {errEntity.Get<string>(ErrorFieldDebugNotes)}";
            throw l.Done(new Exception($"{ErrorIntro(name, "bug in the code")}\n{message}"));

        }

        private static string ErrorIntro(string name, string reason) =>
            $"The DataSource {name} threw an error. Probably a {reason}. " +
            $"Normally this would be invisible and just return a first item with the error message, " +
            $"but because you are logged in as {nameof(IUser.IsSystemAdmin)} the error is shown directly. " +
            $"If you wish to disable this, set 'debug: false'. " +
            $"These are the error details: \n";
    }
}
