using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi.Formats;
using ToSic.SexyContent.WebApi.Permissions;
using ICmsBlock = ToSic.Sxc.Blocks.ICmsBlock;

namespace ToSic.SexyContent.WebApi.SaveHelpers
{
    internal class DnnPublishing:SaveHelperBase
    {
        public DnnPublishing(ICmsBlock cmsInstance, ILog parentLog) : base(cmsInstance, parentLog, "Api.DnnPub") { }

        internal Dictionary<Guid, int> SaveWithinDnnPagePublishing<T>(
            int appId,
            List<BundleWithHeader<T>> items,
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
            Dictionary<Guid, int> SaveAndSaveGroups(Func<bool, Dictionary<Guid, int>> call, bool forceSaveAsDraft)
            {
                var ids = call.Invoke(forceSaveAsDraft);
                // now assign all content-groups as needed
                new ContentGroupList(CmsInstance, Log)
                    .IfInListUpdateList(appId, items, ids);
                return ids;
            }


            // use dnn versioning if partOfPage
            if (partOfPage)
            {
                Log.Add("partOfPage - save with publishing");
                var versioning = Factory.Resolve<IEnvironmentFactory>().PagePublisher(Log);
                var context = SxcApiControllerBase.GetContext(CmsInstance, Log);
                versioning.DoInsidePublishing(context.Dnn.Module.ModuleID, context.Dnn.User.UserID,
                    args => postSaveIds = SaveAndSaveGroups(internalSaveMethod, forceDraft));
            }
            else
            {
                Log.Add("partOfPage false, save without publishing");
                postSaveIds = SaveAndSaveGroups(internalSaveMethod, forceDraft);
            }

            Log.Add(() => $"post save IDs: {string.Join(",", postSaveIds.Select(psi => psi.Key + "(" + psi.Value + ")"))}");
            return postSaveIds;
        }
    }
}
