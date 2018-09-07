using System;
using System.Collections.Generic;
using System.Linq;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Apps;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.WebApi.Formats;
using ToSic.SexyContent.Environment.Dnn7;

namespace ToSic.SexyContent.WebApi.SaveHelpers
{
    internal class ContentGroup: SaveHelperBase
    {
        public ContentGroup(SxcInstance sxcInstance, Log parentLog) : base(sxcInstance, parentLog, "Api.GrpPrc") {}

        internal void DoGroupProcessingIfNecessary<T>(int appId, List<BundleWithHeader<T>> items, Dictionary<Guid, int> ids)
        {
            Log.Add("check groupings");
            var groupItems = items.Where(i => i.Header.Group != null)
                .GroupBy(i => i.Header.Group.Guid.ToString() + i.Header.Group.Index.ToString() + i.Header.Group.Add)
                .ToList();

            // if it's new, it has to be added to a group
            // only add if the header wants it, AND we started with ID unknown
            if (groupItems.Any())
                DoAdditionalGroupProcessing(appId, ids, groupItems);
            else
                Log.Add("no additional group processing necessary");
        }

        private void DoAdditionalGroupProcessing<T>(
            int appId,
            Dictionary<Guid, int> postSaveIds,
            IEnumerable<IGrouping<string, BundleWithHeader<T>>> groupItems)
        {
            var myLog = new Log("2Ap.GrpPrc", Log, "start");
            var app = new App(new DnnTenant(PortalSettings.Current), appId);
            var userMayEdit = SxcInstance.UserMayEdit;

            app.InitData(userMayEdit, SxcInstance.Environment.PagePublishing.IsEnabled(SxcInstance.EnvInstance.Id), SxcInstance.Data.ConfigurationProvider);

            foreach (var entitySets in groupItems)
            {
                myLog.Add("processing:" + entitySets.Key);
                var contItem =
                    entitySets.FirstOrDefault(e => e.Header.Group.Part.ToLower() == AppConstants.ContentLower) ??
                    entitySets.FirstOrDefault(e => e.Header.Group.Part.ToLower() == AppConstants.ListContentLower);
                if (contItem == null)
                    throw new Exception("unexpected group-entity assigment, cannot figure it out");

                var presItem =
                    entitySets.FirstOrDefault(e => e.Header.Group.Part.ToLower() == AppConstants.PresentationLower) ??
                    entitySets.FirstOrDefault(e => e.Header.Group.Part.ToLower() == AppConstants.ListPresentationLower);

                // Get group to assign to and parameters
                var contentGroup = app.ContentGroupManager.GetContentGroup(contItem.Header.Group.Guid);
                var partName = contItem.Header.Group.Part;

                // var part = contentGroup[partName];
                var index = contItem.Header.Group.Index;

                // Get saved entity (to get its ID)
                if (!postSaveIds.ContainsKey(contItem.EntityGuid))
                    throw new Exception("Saved entity not found - not able to update ContentGroup");

                var postSaveId = postSaveIds[contItem.EntityGuid];

                int? presentationId = null;

                if (presItem != null)
                {
                    if (postSaveIds.ContainsKey(presItem.EntityGuid))
                        presentationId = postSaveIds[presItem.EntityGuid];

                    presentationId = presItem.Header.Group.SlotIsEmpty ? null : presentationId;
                    // use null if it shouldn't have one
                }
                // add or update slots
                var reallyAddGroup = contItem.EntityId == 0; // only really add if it's really new
                if (contItem.Header.Group.Add && reallyAddGroup) // this cannot be auto-detected, it must be specified
                    contentGroup.AddContentAndPresentationEntity(partName, index, postSaveId, presentationId);
                else // if (part.Count <= index || part[index] == null)
                    contentGroup.UpdateEntityIfChanged(partName, index, postSaveId, true, presentationId);
            }

            // update-module-title
            SxcInstance.ContentBlock.Manager.UpdateTitle();
        }

        internal List<ItemIdentifier> ConvertListIndexToId(List<ItemIdentifier> items, App app)
        {
            Log.Add("ConvertListIndexToId()");
            var newItems = new List<ItemIdentifier>();
            foreach (var reqItem in items)
            {
                // only do special processing if it's a "group" item
                if (reqItem.Group == null)
                {
                    newItems.Add(reqItem);
                    continue;
                }

                var contentGroup = app.ContentGroupManager.GetContentGroup(reqItem.Group.Guid);
                var contentTypeStaticName = contentGroup.Template.GetTypeStaticName(reqItem.Group.Part);

                // if there is no content-type for this, then skip it (don't deliver anything)
                if (contentTypeStaticName == "")
                    continue;

                ConvertListIndexToEntityIds(contentGroup, reqItem, contentTypeStaticName);

                newItems.Add(reqItem);
            }
            return newItems;
        }


        private static void ConvertListIndexToEntityIds(SexyContent.ContentGroup contentGroup, ItemIdentifier reqItem,
            string contentTypeStaticName)
        {
            var part = contentGroup[reqItem.Group.Part];
            reqItem.ContentTypeName = contentTypeStaticName;
            if (!reqItem.Group.Add && // not in add-mode
                part.Count > reqItem.Group.Index && // has as many items as desired
                part[reqItem.Group.Index] != null) // and the slot has something
                reqItem.EntityId = part[reqItem.Group.Index].EntityId;

            // tell the UI that it should not actually use this data yet, keep it locked
            if (!reqItem.Group.Part.ToLower().Contains(AppConstants.PresentationLower))
                return;

            reqItem.Group.SlotCanBeEmpty = true; // all presentations can always be locked

            if (reqItem.EntityId != 0)
                return;

            reqItem.Group.SlotIsEmpty = true; // if it is blank, then lock this one to begin with

            reqItem.DuplicateEntity =
                reqItem.Group.Part.ToLower() == AppConstants.PresentationLower
                    ? contentGroup.Template.PresentationDemoEntity?.EntityId
                    : contentGroup.Template.ListPresentationDemoEntity?.EntityId;
        }

    }
}
