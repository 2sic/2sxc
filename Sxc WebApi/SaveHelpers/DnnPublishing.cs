using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav;
using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi.Formats;
using ToSic.SexyContent.WebApi.Permissions;

namespace ToSic.SexyContent.WebApi.SaveHelpers
{
    internal class DnnPublishing:SaveHelperBase
    {
        public DnnPublishing(SxcInstance sxcInstance, Log parentLog) : base(sxcInstance, parentLog, "Api.DnnPub") { }

        internal Dictionary<Guid, int> SaveWithinDnnPagePublishing<T>(
            int appId,
            List<BundleWithHeader<T>> items,
            bool partOfPage,
            Func<bool, Dictionary<Guid, int>> internalSaveMethod,
            AppAndPermissions permCheck
            )
        {
            var allowWriteLive = permCheck.Permissions.UserMay(GrantSets.WritePublished);
            var forceDraft = !allowWriteLive;
            Log.Add($"allowWrite: {allowWriteLive} forceDraft: {forceDraft}");

            // list of saved IDs
            Dictionary<Guid, int> postSaveIds = null;

            // The internal call which will be used further down
            Dictionary<Guid, int> SaveAndSaveGroups(Func<bool, Dictionary<Guid, int>> call, bool forceSaveAsDraft)
            {
                var ids = call.Invoke(forceSaveAsDraft);
                // now assign all content-groups as needed
                new ContentGroup(SxcInstance, Log)
                    .DoGroupProcessingIfNecessary(appId, items, ids);
                return ids;
            }


            // use dnn versioning if partOfPage
            if (partOfPage)
            {
                Log.Add("partOfPage - save with publishing");
                var versioning = Factory.Resolve<IEnvironmentFactory>().PagePublisher(Log);
                var context = SxcApiControllerBase.GetContext(SxcInstance, Log);
                versioning.DoInsidePublishing(context.Dnn.Module.ModuleID, context.Dnn.User.UserID,
                    args => postSaveIds = SaveAndSaveGroups(internalSaveMethod, forceDraft));// SaveAndProcessGroups(appId, items, partOfPage, forceDraft));
            }
            else
            {
                Log.Add("partOfPage false, save without publishing");
                postSaveIds = SaveAndSaveGroups(internalSaveMethod, forceDraft);// SaveAndProcessGroups(appId, items, partOfPage, forceDraft);
            }

            Log.Add(() => $"post save IDs: {string.Join(",", postSaveIds.Select(psi => psi.Key + "(" + psi.Value + ")"))}");
            return postSaveIds;
        }
    }
}
