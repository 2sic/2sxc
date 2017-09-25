using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.Persistence;
using EntityRelationship = ToSic.Eav.Data.EntityRelationship;

namespace ToSic.SexyContent
{
    public class ContentGroup: HasLog
    {
        private Eav.Interfaces.IEntity _contentGroupEntity;
        private readonly int _zoneId;
        private readonly int _appId;
        private readonly bool _showDrafts;
        private readonly bool _versioningEnabled;
        private readonly Guid? _previewTemplateId;

        public ContentGroup(Eav.Interfaces.IEntity contentGroupEntity, int zoneId, int appId, bool showDrafts, bool versioningEnabled, Log parentLog): base("ConGrp", parentLog)
        {
            _contentGroupEntity = contentGroupEntity ?? throw new Exception("ContentGroup entity is null. This usually happens when you are duplicating a site, and have not yet imported the other content/apps. If that is your issue, check 2sxc.org/help?tag=export-import");
            _zoneId = zoneId;
            _appId = appId;
            _showDrafts = showDrafts;
            _versioningEnabled = versioningEnabled;
        }

        /// <summary>
        /// Instanciate a "temporary" ContentGroup with the specified templateId and no Content items
        /// </summary>
        public ContentGroup(Guid? previewTemplateId, int zoneId, int appId, bool showDrafts, bool versioningEnabled, Log parentLog): base("ConGrp", parentLog)
        {
            _previewTemplateId = previewTemplateId;
            _zoneId = zoneId;
            _appId = appId;
            _showDrafts = showDrafts;
        }

        /// <summary>
        /// Returns true if a content group entity for this group really exists
        /// Means for example, that the app can't be changed anymore
        /// </summary>
        public bool Exists => _contentGroupEntity != null;

        internal bool DataIsMissing = false;
        #region Template stuff

        private Template _template;

        public Template Template
        {
            get
            {
                if (_template != null) return _template;

                Eav.Interfaces.IEntity templateEntity = null;

                if (_previewTemplateId.HasValue)
                {
                    var dataSource = DataSource.GetInitialDataSource(_zoneId, _appId);
                    // ToDo: Should use an indexed Guid filter
                    templateEntity =
                        dataSource.List.FirstOrDefault(e => e.Value.EntityGuid == _previewTemplateId).Value;
                }
                else if (_contentGroupEntity != null)
                    templateEntity =
                        ((EntityRelationship) _contentGroupEntity.Attributes["Template"][0]).FirstOrDefault();

                _template = templateEntity == null ? null : new Template(templateEntity, Log);

                return _template;
            }
        }

        public void UpdateTemplate(int? templateId)
        {
            var values = new Dictionary<string, object>
            {
                {"Template", templateId.HasValue ? new List<int?> {templateId.Value} : new List<int?>()}
            };

            new AppManager(_zoneId, _appId).Entities.UpdateParts(_contentGroupEntity.EntityId, values);
        }

        #endregion

        public int ContentGroupId => _contentGroupEntity?.EntityId ?? 0;

        public Guid ContentGroupGuid => _contentGroupEntity?.EntityGuid ?? Guid.Empty;

        #region Retrieve the lists - either as object or by the type-indexer

        public List<Eav.Interfaces.IEntity> Content
        {
            get
            {
                if (_contentGroupEntity == null) return new List<Eav.Interfaces.IEntity> {null};
                var list = ((EntityRelationship) _contentGroupEntity.GetBestValue(AppConstants.Content)).ToList();
                return list.Count > 0 ? list : new List<Eav.Interfaces.IEntity> {null};
            }
        }

        public List<Eav.Interfaces.IEntity> Presentation => ((EntityRelationship) _contentGroupEntity?.GetBestValue(AppConstants.Presentation))?.ToList() ?? new List<Eav.Interfaces.IEntity>();

        public List<Eav.Interfaces.IEntity> ListContent => ((EntityRelationship) _contentGroupEntity?.GetBestValue(AppConstants.ListContent/*cListC*/))?.ToList() ?? new List<Eav.Interfaces.IEntity>();

        public List<Eav.Interfaces.IEntity> ListPresentation => ((EntityRelationship) _contentGroupEntity?.GetBestValue(AppConstants.ListPresentation/* cListP*/))?.ToList() ?? new List<Eav.Interfaces.IEntity>();

        public List<Eav.Interfaces.IEntity> this[string type]
        {
            get
            {
                switch (type.ToLower())
                {
                    case AppConstants.ContentLower:
                        return Content;
                    case AppConstants.PresentationLower: 
                        return Presentation;
                    case AppConstants.ListContentLower:
                        return ListContent;
                    case AppConstants.ListPresentationLower:
                        return ListPresentation;
                    default:
                        throw new Exception("Type " + type + " not allowed");
                }
            }
        }

        public List<int?> ListWithNulls(string type)
        {
            return this[type].Select(p => p?.EntityId).ToList();
        }

        #endregion

        public void UpdateEntityIfChanged(string type, int sortOrder, int? entityId, bool updatePresentation,
            int? presentationId)
        {
            var somethingChanged = false;
            if (sortOrder == -1)
                throw new Exception("Sortorder is never -1 any more; deprecated");

            var listMain = ListWithNulls(type); // this[type].Select(p => p?.EntityId).ToList();

            // if necessary, add to end
            if (listMain.Count < sortOrder + 1)
                listMain.AddRange(Enumerable.Repeat(new int?(), (sortOrder + 1) - listMain.Count));

            if (listMain[sortOrder] != entityId)
            {
                listMain[sortOrder] = entityId;
                somethingChanged = true;
            }

            var package = PrepareSavePackage(type, listMain);

            if (updatePresentation)
            {
                var type2 = ReCapitalizePartName(type).Replace(AppConstants.Content, AppConstants.Presentation);
                var listPres = ListWithNulls(type2);
                if (listPres.Count < sortOrder + 1)
                    listPres.AddRange(Enumerable.Repeat(new int?(), (sortOrder + 1) - listPres.Count));
                if (listPres[sortOrder] != presentationId)
                {
                    listPres[sortOrder] = presentationId;
                    somethingChanged = true;
                }
                package = PrepareSavePackage(type2, listPres, package);
            }

            if (somethingChanged)
                SaveChangedLists(package);
        }

        private void SaveChangedLists(Dictionary<string, List<int?>> values)
        {
            // ensure that there are never more presentations than values
            if (values.ContainsKey(AppConstants.Presentation))
            {
                var contentCount = Content.Count;
                if (values.ContainsKey(AppConstants.Content))
                    contentCount = values[AppConstants.Content].Count;
                if (values[AppConstants.Presentation].Count > contentCount)
                    throw new Exception("Presentation may not contain more items than Content.");
            }

            // 2017-04-01 2dm centralizing eav access
            var dicObj = values.ToDictionary(x => x.Key, x => x.Value as object);
            var newEnt = new Entity(_appId, 0, "", dicObj);
            var saveOpts = SaveOptions.Build(_zoneId);
            saveOpts.PreserveUntouchedAttributes = true;

            var saveEnt = EntitySaver.CreateMergedForSaving(_contentGroupEntity, newEnt, saveOpts);

            if (_versioningEnabled)
            {
                // Force saving as draft if needed (if versioning is enabled)
                ((Entity)saveEnt).PlaceDraftInBranch = true;
                ((Entity)saveEnt).IsPublished = false;
            }

            new AppManager(_zoneId, _appId).Entities.Save(saveEnt, saveOpts);

            // Refresh content group entity (ensures contentgroup is up to date)
            _contentGroupEntity = new ContentGroupManager(_zoneId, _appId, _showDrafts, _versioningEnabled, Log).GetContentGroup(_contentGroupEntity.EntityGuid)._contentGroupEntity;
        }

        private Dictionary<string, List<int?>> PrepareSavePackage(string type, List<int?> entityIds,
            Dictionary<string, List<int?>> alreadyInitializedDictionary = null)
        {
            type = ReCapitalizePartName(type);
            if (alreadyInitializedDictionary == null) alreadyInitializedDictionary = new Dictionary<string, List<int?>>();
            alreadyInitializedDictionary.Add(type, entityIds.ToList());
            return alreadyInitializedDictionary;
        }


        private string ReCapitalizePartName(string partName)
        {
            partName = partName.ToLower();
            if (partName == AppConstants.ContentLower) partName = AppConstants.Content;
            else if (partName == AppConstants.PresentationLower) partName = AppConstants.Presentation;
            else if (partName == AppConstants.ListContentLower) partName = AppConstants.ListContent;// cListC;
            else if (partName == AppConstants.ListPresentationLower) partName = AppConstants.ListPresentation;// cListP;
            else throw new Exception("Wanted to capitalize part name - but part name unknown: " + partName);
            return partName;
        }

        /// <summary>
        /// Removes entities from a group. This will also remove the corresponding presentation entities.
        /// </summary>
        /// <param name="type">Should be 'Content' or "ListContent"</param>
        /// <param name="sortOrder"></param>
        public void RemoveContentAndPresentationEntities(string type, int sortOrder)
        {

            var list1 = ListWithNulls(type);
            list1.RemoveAt(sortOrder);
            var type2 = ReCapitalizePartName(type).Replace(AppConstants.Content, AppConstants.Presentation);
            var list2 = ListWithNulls(type2);
            if (list2.Count > sortOrder)    // in many cases the presentation-list is empty, then there is nothing to remove
                list2.RemoveAt(sortOrder);
            SaveChangedLists(PrepareSavePackage(type, list1, PrepareSavePackage(type2, list2)));
        }


        /// <summary>
        /// If SortOrder is not specified, adds at the end
        /// </summary>
        /// <param name="type"></param>
        /// <param name="sortOrder"></param>
        /// <param name="contentId"></param>
        /// <param name="presentationId"></param>
        public void AddContentAndPresentationEntity(string type, int? sortOrder, int? contentId, int? presentationId)
        {
            if (type.ToLower() != AppConstants.ContentLower)
                throw new Exception("This is only meant to work for content, not for list-content");

            if (!sortOrder.HasValue)
                sortOrder = Content.Count;

            var list1 = ListWithNulls(type);
            list1.Insert(sortOrder.Value, contentId);

            var list2 = GetPresentationIdWithSameLengthAsContent();
            list2.Insert(sortOrder.Value, presentationId);

            SaveChangedLists(PrepareSavePackage(AppConstants.Content, list1, PrepareSavePackage(AppConstants.Presentation, list2)));

        }


        private List<int?> GetPresentationIdWithSameLengthAsContent()
        {
            var difference = Content.Count - Presentation.Count;

            // this should fix https://github.com/2sic/2sxc/issues/1178 and https://github.com/2sic/2sxc/issues/1158
            // goal is to simply remove the last presentation items, if there is a missmatch. Usually they will simply be nulls anyhow
            if (difference < 0)
                Presentation.RemoveRange(Content.Count, difference);

            var entityIds = ListWithNulls(AppConstants.Presentation);

            // extend as necessary
            if (difference != 0)
                entityIds.AddRange(Enumerable.Repeat(new int?(), difference));

            return entityIds;
        }

        public void ReorderEntities(int sortOrder, int destinationSortOrder)
        {
            var contentIds = ListWithNulls(AppConstants.Content); 
            var presentationIds = GetPresentationIdWithSameLengthAsContent();
            var contentId = contentIds[sortOrder];
            var presentationId = presentationIds[sortOrder];

            /*
             * ToDo 2017-08-28:
             * Create a DRAFT copy of the ContentGroup if versioning is enabled.
             */
            
            contentIds.RemoveAt(sortOrder);
            presentationIds.RemoveAt(sortOrder);

            contentIds.Insert(destinationSortOrder, contentId);
            presentationIds.Insert(destinationSortOrder, presentationId);

            var list = PrepareSavePackage(AppConstants.Content, contentIds);
            list = PrepareSavePackage(AppConstants.Presentation, presentationIds, list);
            SaveChangedLists(list);
        }

        public bool ReorderAll(int[] newSequence)
        {
            var oldCIds = ListWithNulls(AppConstants.Content);
            var oldPIds = GetPresentationIdWithSameLengthAsContent();

            // some error checks
            if(newSequence.Length != oldCIds.Count)
                throw new Exception("Can't re-order - list length is different");

            var newContentIds = new List<int?>();
            var newPresIds = new List<int?>();

            for (var seqItem = 0; seqItem < newSequence.Length; seqItem++)
            {
                var cId = oldCIds[newSequence[seqItem]];
                newContentIds.Add(cId);

                var pId = oldPIds[newSequence[seqItem]];
                newPresIds.Add(pId);
            }

            var list = PrepareSavePackage(AppConstants.Content, newContentIds);
            list = PrepareSavePackage(AppConstants.Presentation, newPresIds, list);
            SaveChangedLists(list);

            return true;
        }
    }
}