using ToSic.Eav.Apps;
using ToSic.Sxc.Code.Generate.Sys;
using ToSic.Sys.Users;

namespace ToSic.Sxc.Code.Generate.Data;

/// <summary>
/// Experimental
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class CSharpTypedDataModelsGenerator(IUser user, IAppReaderFactory appReadFac)
    : CSharpModelsGeneratorBase(user, appReadFac, SxcLogName + ".DMoGen") // IFileGenerator is inherited from base
{
    #region Information for the interface

    public override string Description => "Generates C# Data Classes for the AppCode/Data folder";

    public override string DescriptionHtml => $"The {Name} will generate <code>[TypeName].Generated.cs</code> files in the <code>AppCode/Data</code> folder.";

    protected override string GeneratedSetName => "C# Data Classes";

    #endregion

    protected internal override CSharpCodeSpecs BuildDerivedSpecs(IFileGeneratorSpecs parameters) => BuildSpecs(parameters);

    protected override IGeneratedFile? CreateFileGenerator(IContentType type, string className) 
        => new CSharpTypedDataModelGenerator(this, type, className).PrepareFile();
}