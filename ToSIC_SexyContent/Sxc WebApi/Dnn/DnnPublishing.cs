using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi.Formats;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Dnn.Code;
using ToSic.Sxc.Security;
using ToSic.Sxc.WebApi;

namespace ToSic.Sxc.Dnn
{
    internal class DnnPublishing:SaveHelperBase
    {
        public DnnPublishing(IBlockBuilder blockBuilder, ILog parentLog) : base(blockBuilder, parentLog, "Api.DnnPub") { }

        internal Dictionary<Guid, int> SaveWithinDnnPagePublishingAndUpdateParent(
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
            Dictionary<Guid, int> SaveAndSaveGroupsInnerCall(Func<bool, Dictionary<Guid, int>> call, bool forceSaveAsDraft)
            {
                var ids = call.Invoke(forceSaveAsDraft);
                // now assign all content-groups as needed
                new ContentGroupList(BlockBuilder, Log).IfChangesAffectListUpdateIt(appId, items, ids);
                return ids;
            }


            // use dnn versioning if partOfPage
            if (partOfPage)
            {
                Log.Add("partOfPage - save with publishing");
                var versioning = Eav.Factory.Resolve<IEnvironmentFactory>().PagePublisher(Log);
                var context = DnnDynamicCode.Create(BlockBuilder, Log);
                versioning.DoInsidePublishing(context.Dnn.Module.ModuleID, context.Dnn.User.UserID,
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
