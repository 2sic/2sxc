using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Lib.Services;
using ToSic.Sxc.Internal;

namespace ToSic.Sxc.Code.Generate.Internal;

/// <summary>
/// Experimental
/// </summary>
[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract class CSharpGeneratorBase(IUser user, IAppStates appStates, string logName)
    : ServiceBase(logName ?? (SxcLogName + ".DMoGen"))
{

    internal IUser User = user;

    #region Information for the interface

    public string NameId => GetType().FullName;

    public string Name => GetType().Name;

    public string Version => SharedAssemblyInfo.AssemblyVersion;

    public string OutputLanguage => "CSharp";

    #endregion

    internal CSharpCodeSpecs BuildSpecs(IFileGeneratorSpecs parameters)
    {
        // Prepare App State and add to Specs
        var appCache = appStates.GetCacheState(parameters.AppId);
        var appState = appStates.ToReader(appCache);

        var specs = new CSharpCodeSpecs
        {
            AppId = parameters.AppId,
            Edition = parameters.Edition ?? "",
            DateTime = parameters.DateTime,
            AppState = appState,
            AppName = appState.Name,
        };
        return specs;
    }
    
}