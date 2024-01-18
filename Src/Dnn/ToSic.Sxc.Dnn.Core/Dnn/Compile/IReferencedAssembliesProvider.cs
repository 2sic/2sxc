namespace ToSic.Sxc.Dnn.Compile
{
    [PrivateApi]
    public interface IReferencedAssembliesProvider
    {
        string[] Locations();
    }
}