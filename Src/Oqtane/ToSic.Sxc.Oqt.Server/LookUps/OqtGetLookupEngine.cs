using System;
using ToSic.Lib.Logging;
using ToSic.Eav.LookUp;
using ToSic.Sxc.LookUp;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.LookUps
{
    public class OqtGetLookupEngine : HasLog<ILookUpEngineResolver>, ILookUpEngineResolver
    {
        #region Constructor and DI

        public OqtGetLookupEngine(
            Lazy<QueryStringLookUp> queryStringLookUp,
            Lazy<SiteLookUp> siteLookUp,
            Lazy<OqtPageLookUp> pageLookUp,
            Lazy<OqtModuleLookUp> moduleLookUp,
            Lazy<UserLookUp> userLookUp) : base($"{OqtConstants.OqtLogPrefix}.LookUp")
        {
            _queryStringLookUp = queryStringLookUp;
            _siteLookUp = siteLookUp;
            _pageLookUp = pageLookUp;
            _moduleLookUp = moduleLookUp;
            _userLookUp = userLookUp;
        }
        private readonly Lazy<QueryStringLookUp> _queryStringLookUp;
        private readonly Lazy<SiteLookUp> _siteLookUp;
        private readonly Lazy<OqtPageLookUp> _pageLookUp;
        private readonly Lazy<OqtModuleLookUp> _moduleLookUp;
        private readonly Lazy<UserLookUp> _userLookUp;

        #endregion

        public ILookUpEngine GetLookUpEngine(int instanceId)
        {
            var providers = new LookUpEngine(Log);

            providers.Add(_queryStringLookUp.Value);
            providers.Add(new DateTimeLookUp());
            providers.Add(new TicksLookUp());

            providers.Add(_siteLookUp.Value);
            providers.Add(_pageLookUp.Value);
            providers.Add(_moduleLookUp.Value);
            providers.Add(_userLookUp.Value);

            return providers;
        }
    }
}
