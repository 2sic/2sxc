using ToSic.Eav.Context;
using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Context
{
    [PrivateApi]
    public class CmsCulture: ICmsCulture
    {
        private readonly CmsContext _parent;

        internal CmsCulture(CmsContext parent)
        {
            _parent = parent;
        }

        public string DefaultCode => _parent.Context.Site.DefaultCultureCode;

        public string CurrentCode => _parent.Context.Site.SafeCurrentCultureCode();
    }
}
