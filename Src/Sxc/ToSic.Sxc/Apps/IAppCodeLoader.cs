using System.Reflection;

namespace ToSic.Sxc.Apps
{
    public interface IAppCodeLoader
    {
        Assembly GetAppCodeAssemblyOrNull(int appId);
    }
}