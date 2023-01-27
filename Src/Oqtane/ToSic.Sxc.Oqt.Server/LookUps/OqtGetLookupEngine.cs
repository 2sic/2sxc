using System;
using ToSic.Lib.Logging;
using ToSic.Eav.LookUp;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.LookUp;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.LookUps
{
    public class OqtGetLookupEngine : ServiceBase, ILookUpEngineResolver
    {
        #region Constructor and DI

        public OqtGetLookupEngine(
            LazySvc<QueryStringLookUp> queryStringLookUp,
            LazySvc<SiteLookUp> siteLookUp,
            LazySvc<OqtPageLookUp> pageLookUp,
            LazySvc<OqtModuleLookUp> moduleLookUp,
            LazySvc<UserLookUp> userLookUp) : base($"{OqtConstants.OqtLogPrefix}.LookUp")
        {
            _queryStringLookUp = queryStringLookUp;
            _siteLookUp = siteLookUp;
            _pageLookUp = pageLookUp;
            _moduleLookUp = moduleLookUp;
            _userLookUp = userLookUp;
        }
        private readonly LazySvc<QueryStringLookUp> _queryStringLookUp;
        private readonly LazySvc<SiteLookUp> _siteLookUp;
        private readonly LazySvc<OqtPageLookUp> _pageLookUp;
        private readonly LazySvc<OqtModuleLookUp> _moduleLookUp;
        private readonly LazySvc<UserLookUp> _userLookUp;

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
