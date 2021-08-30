using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.Security;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi.Formats;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Cms.Publishing;

namespace ToSic.Sxc.WebApi.Save
{
    public class SxcPagePublishing: SaveHelperBase<SxcPagePublishing>
    {
        #region Constructor / DI
        public SxcPagePublishing(ContentGroupList contentGroupList, IPagePublishing pagePublishing, IAppStates appStates) : base("Sxc.PgPubl")
        {
            _contentGroupList = contentGroupList;
            _pagePublishing = pagePublishing;
            _appStates = appStates;
        }
        private readonly ContentGroupList _contentGroupList;
        private readonly IPagePublishing _pagePublishing;
        private readonly IAppStates _appStates;

        #endregion

        internal Dictionary<Guid, int> SaveInPagePublishing(
            IBlock blockOrNull,
            int appId,
            List<BundleWithHeader<IEntity>> items,
            bool partOfPage,
            Func<bool, Dictionary<Guid, int>> internalSaveMethod,
            IMultiPermissionCheck permCheck
        )
        {
            var allowWriteLive = permCheck.UserMayOnAll(GrantSets.WritePublished);
            var forceDraft = !allowWriteLive;
            Log.Add($"allowWrite: {allowWriteLive} forceDraft: {forceDraft}");

            // list of saved IDs
            Dictionary<Guid, int> postSaveIds = null;

            // The internal call which will be used further down
            var appIdentity = _appStates.Identity(null, appId);
            var groupList = _contentGroupList.Init(appIdentity, Log, Context.UserMayEdit);
            Dictionary<Guid, int> SaveAndSaveGroupsInnerCall(Func<bool, Dictionary<Guid, int>> call,
                bool forceSaveAsDraft)
            {
                var ids = call.Invoke(forceSaveAsDraft);
                // now assign all content-groups as needed
                groupList.IfChangesAffectListUpdateIt(blockOrNull, items, ids);
                return ids;
            }


            // use dnn versioning if partOfPage
            if (partOfPage)
            {
                Log.Add("partOfPage - save with publishing");
                var versioning = _pagePublishing.Init(Log);
                versioning.DoInsidePublishing(Context,
                    args => postSaveIds = SaveAndSaveGroupsInnerCall(internalSaveMethod, forceDraft));
            }
            else
            {
                Log.Add("partOfPage false, save without publishing");
                postSaveIds = SaveAndSaveGroupsInnerCall(internalSaveMethod, forceDraft);
            }

            Log.Add(() => $"post save IDs: {string.Join(",", postSaveIds.Select(psi => psi.Key + "(" + psi.Value + ")"))}");
            return postSaveIds;
        }

    }
}
