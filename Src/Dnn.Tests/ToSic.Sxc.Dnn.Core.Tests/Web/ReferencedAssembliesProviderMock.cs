using ToSic.Sxc.Web;

namespace ToSic.Sxc.Dnn.Core.Tests.Web
{
    public class ReferencedAssembliesProviderMock : IReferencedAssembliesProvider
    {
        public string[] Locations()
        {
            // dummy
            return new string[] {};
        }
    }
}
