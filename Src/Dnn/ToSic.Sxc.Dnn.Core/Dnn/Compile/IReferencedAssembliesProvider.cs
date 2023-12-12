using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Dnn.Compile
{
    [InternalApi_DoNotUse_MayChangeWithoutNotice()]
    public interface IReferencedAssembliesProvider
    {
        string[] Locations();
    }
}