using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Mvc.Dev;

namespace ToSic.Sxc.Mvc.Run
{
    public class MvcEnvironment : HasLog<IEnvironment>, IEnvironment
    {
        public MvcEnvironment() : base("Mvc.Enviro")
        {
        }

        public string DefaultLanguage => TestIds.DefaultLanguage;


        public IUser User { get; } = new MvcUser();
        
    }
}
