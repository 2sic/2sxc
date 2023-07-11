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
            LazySvc<OqtSiteLookUp> siteLookUp,
            LazySvc<OqtPageLookUp> pageLookUp,
            LazySvc<OqtModuleLookUp> moduleLookUp,
            LazySvc<OqtUserLookUp> userLookUp) : base($"{OqtConstants.OqtLogPrefix}.LookUp")
        {
            _queryStringLookUp = queryStringLookUp;
            _siteLookUp = siteLookUp;
            _pageLookUp = pageLookUp;
            _moduleLookUp = moduleLookUp;
            _userLookUp = userLookUp;
        }
        private readonly LazySvc<QueryStringLookUp> _queryStringLookUp;
        private readonly LazySvc<OqtSiteLookUp> _siteLookUp;
        private readonly LazySvc<OqtPageLookUp> _pageLookUp;
        private readonly LazySvc<OqtModuleLookUp> _moduleLookUp;
        private readonly LazySvc<OqtUserLookUp> _userLookUp;

        #endregion

        public ILookUpEngine GetLookUpEngine(int instanceId)
        {
            var luEngine = new LookUpEngine(Log);

            luEngine.Add(_queryStringLookUp.Value);
            luEngine.Add(new DateTimeLookUp());
            luEngine.Add(new TicksLookUp());

            luEngine.Add(_siteLookUp.Value);
            luEngine.Add(_pageLookUp.Value);
            luEngine.Add(_moduleLookUp.Value);
            luEngine.Add(_userLookUp.Value);

            return luEngine;
        }
    }
}
