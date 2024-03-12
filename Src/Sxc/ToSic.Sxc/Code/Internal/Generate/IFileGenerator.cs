using ToSic.Lib.Data;

namespace ToSic.Sxc.Code.Internal.Generate;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IFileGenerator: IHasIdentityNameId
{
    public string Name { get; }

    public string Version { get; }
    public string Description { get; }

    public ICodeFileBundle[] Generate();
}