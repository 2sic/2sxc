
using ToSic.Eav.LookUp;
using ToSic.Eav.ValueProviders;

namespace ToSic.Sxc.Interfaces
{
    internal interface IEnvironmentValueProviders
    {
        TokenListFiller GetProviders(int instanceId);
    }
}