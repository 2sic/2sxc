
using ToSic.Eav.LookUp;

// TODO: move to other namespaces, probably Eav.LookUp or Eav.Apps
namespace ToSic.Sxc.Interfaces
{
    internal interface IEnvironmentLookUps
    {
        TokenListFiller GetLookUps(int instanceId);
    }
}