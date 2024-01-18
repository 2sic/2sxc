using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using ToSic.Eav.Integration.Environment;
using ToSic.Sxc.Dnn.Search;

namespace ToSic.Sxc.Dnn;

public class DnnEnvironmentLogger: IEnvironmentLogger
{
    public void LogException(Exception ex) => Exceptions.LogException(ex);

    #region Diagnostics stuff

    public static int SearchErrorsMax = 10;

    public static int SearchErrorsCount { get; set; }

    #endregion

    public static void AddSearchExceptionToLog(ModuleInfo moduleInfo, Exception e, string nameOfSource)
    {
        var errCount = SearchErrorsCount++;
        // ignore errors after 10
        if (errCount > SearchErrorsMax) return;

        if (errCount == SearchErrorsMax)
        {
            Exceptions.LogException(new SearchIndexException(moduleInfo,
                new(
                    $"Hit {SearchErrorsMax} SearchIndex exceptions in 2sxc modules, will stop reporting them to not flood the error log. \n" +
                    $"To start reporting again up to {SearchErrorsMax} just restart the application. \n" +
                    $"To show more errors change 'ToSic.Sxc.Dnn.{nameof(DnnBusinessController)}.{nameof(SearchErrorsMax)}' to a higher number in some code of yours like in a temporary razor view. " +
                    $"Note that in the meantime, the count may already be higher. You can always get that from {nameof(SearchErrorsCount)}."),
                nameOfSource, errCount, SearchErrorsMax));
            return;
        }

        Exceptions.LogException(new SearchIndexException(moduleInfo, e, nameOfSource, errCount, SearchErrorsMax));
    }
}