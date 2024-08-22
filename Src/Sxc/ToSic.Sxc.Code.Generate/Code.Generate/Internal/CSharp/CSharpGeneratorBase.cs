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
public abstract class CSharpGeneratorBase(IUser user, IAppReaderFactory appReadFac, string logName)
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
        var appReader = appReadFac.Get(parameters.AppId);

        var specs = new CSharpCodeSpecs
        {
            AppId = parameters.AppId,
            Edition = parameters.Edition ?? "",
            DateTime = parameters.DateTime,
            AppContentTypes = appReader,
            AppName = appReader.Specs.Name,
        };
        return specs;
    }
    
}