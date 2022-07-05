// #UnusedFeatureHistoryOfGroup 2022-07-05 2dm removed - probably clean up ca. Q4 2022
//using ToSic.Eav.Apps;
//using ToSic.Eav.Logging;
//using ToSic.Eav.WebApi.Formats;
//using ToSic.Sxc.Apps;

//namespace ToSic.Sxc.WebApi.Cms
//{
//    /// <summary>
//    /// Special helper, ATM only used in the HistoryController
//    /// </summary>
//    public class IdentifierHelper: HasLog<IdentifierHelper>
//    {
//        public IdentifierHelper(CmsRuntime cmsRuntime, IAppStates appStates) : base("Bck.IdHlpr")
//        {
//            _cmsRuntime = cmsRuntime;
//            _appStates = appStates;
//        }

//        private readonly CmsRuntime _cmsRuntime;
//        private readonly IAppStates _appStates;

//        internal ItemIdentifier ResolveItemIdOfGroup(int appId, ItemIdentifier item, ILog log)
//        {
//            if (item.Group == null) return item;
//            var cms = _cmsRuntime.Init(_appStates.IdentityOfApp(appId), true, log);

//            var contentGroup = cms.Blocks.GetBlockConfig(item.Group.Guid);
//            var part = contentGroup[item.Group.Part];
//            var max = part.Count > 0 ? part.Count -1 : 0;
//            var idx = item.ListIndex(max);
//            item.EntityId = part[idx].EntityId;
//            return item;
//        }

//    }
//}
