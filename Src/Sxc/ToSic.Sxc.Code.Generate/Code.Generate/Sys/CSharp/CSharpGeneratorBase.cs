using ToSic.Eav.Apps;
using ToSic.Sxc.Sys;
using ToSic.Sys.Users;

namespace ToSic.Sxc.Code.Generate.Sys;

/// <summary>
/// Experimental
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public abstract class CSharpGeneratorBase(IUser user, IAppReaderFactory appReadFac, string? logName)
    : ServiceBase(logName ?? (SxcLogName + ".DMoGen"))
{

    internal IUser User = user;

    #region Information for the interface

    public string NameId => GetType().FullName!;

    public string Name => GetType().Name;

    public string Version => SxcSharedAssemblyInfo.AssemblyVersion;

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
            Namespace = parameters.Namespace,
            TargetPath = parameters.TargetPath,
            ContentTypes = parameters.ContentTypes,
            Prefix = parameters.Prefix,
            Suffix = parameters.Suffix,
            AppContentTypes = appReader,
            AppName = appReader.Specs.Name,
        };
        if (!string.IsNullOrWhiteSpace(parameters.Namespace))
            specs = specs with { DataNamespace = parameters.Namespace! };
        //if (parameters.Suffix != null)
            //specs = specs with { DataClassGeneratedSuffix = parameters.Suffix };

        return specs;
    }
    
}
