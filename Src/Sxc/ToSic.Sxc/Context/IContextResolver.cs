using ToSic.Eav.Context;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Context
{
    public interface IContextResolver: IHasLog<IContextResolver>
    {
        IContextOfSite Site();

        IContextOfApp App(int appId);

        IContextOfApp App(string appPathOrName);
    }
}
