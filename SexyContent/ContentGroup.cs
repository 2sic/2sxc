using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav;
using ToSic.Eav.BLL;
using EntityRelationship = ToSic.Eav.Data.EntityRelationship;

namespace ToSic.SexyContent
{
    public class ContentGroup
    {
        #region NamingConstants

        public const string cContent = "Content";
        public const string cPresentation = "Presentation";
        public const string cListC = "ListContent";
        public const string cListP = "ListPresentation";

        #endregion

        private IEntity _contentGroupEntity;
        private readonly int _zoneId;
        private readonly int _appId;

        private readonly Guid? _previewTemplateId;


        public ContentGroup(IEntity contentGroupEntity, int zoneId, int appId)
        {
            if (contentGroupEntity == null)
                throw new Exception("ContentGroup entity is null");

            _contentGroupEntity = contentGroupEntity;
            _zoneId = zoneId;
            _appId = appId;
        }

        /// <summary>
        /// Instanciate a "temporary" ContentGroup with the specified templateId and no Content items
        /// </summary>
        public ContentGroup(Guid? previewTemplateId, int zoneId, int appId)
        {
            _previewTemplateId = previewTemplateId;
            _zoneId = zoneId;
            _appId = appId;
        }

        /// <summary>
        /// Returns true if a content group entity for this group really exists
        /// Means for example, that the app can't be changed anymore
        /// </summary>
        public bool Exists
        {
            get { return _contentGroupEntity != null; }
        }

        #region Template stuff

        private Template _template;

        public Template Template
        {
            get
            {
                if (_template == null)
                {

                    IEntity templateEntity = null;

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

                    _template = templateEntity == null ? null : new Template(templateEntity);
                }

                return _template;
            }
        }

        public void UpdateTemplate(int? templateId)
        {
            var values = new Dictionary<string, object>
            {
                {"Template", templateId.HasValue ? new[] {templateId.Value} : new int[] {}}
            };

            var context = EavDataController.Instance(_zoneId, _appId).Entities; // EavContext.Instance(_zoneId, _appId);
            context.UpdateEntity(_contentGroupEntity.EntityGuid, values);
        }

        #endregion


        public int ContentGroupId
        {
            get { return _contentGroupEntity.EntityId; }
        }

        public Guid ContentGroupGuid
        {
            get { return _contentGroupEntity == null ? Guid.Empty : _contentGroupEntity.EntityGuid; }
        }

        #region Retrieve the lists - either as object or by the type-indexer

        public List<IEntity> Content
        {
            get
            {
                if (_contentGroupEntity != null)
                {
                    var list = ((EntityRelationship) _contentGroupEntity.GetBestValue(cContent)).ToList();
                    if (list.Count > 0)
                        return list;
                }

                return new List<IEntity> {null};
            }
        }

        public List<IEntity> Presentation
        {
            get
            {
                if (_contentGroupEntity == null)
                    return new List<IEntity>();
                return ((EntityRelationship) _contentGroupEntity.GetBestValue(cPresentation)).ToList();
            }
        }

        public List<IEntity> ListContent
        {
            get
            {
                if (_contentGroupEntity == null)
                    return new List<IEntity>();
                return ((EntityRelationship) _contentGroupEntity.GetBestValue(cListC)).ToList();
            }
        }

        public List<IEntity> ListPresentation
        {
            get
            {
                if (_contentGroupEntity == null)
                    return new List<IEntity>();
                return ((EntityRelationship) _contentGroupEntity.GetBestValue(cListP)).ToList();
            }
        }

        public List<IEntity> this[string type]
        {
            get
            {
                switch (type.ToLower())
                {
                    case "content":
                        return Content;
                    case "presentation":
                        return Presentation;
                    case "listcontent":
                        return ListContent;
                    case "listpresentation":
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
                var type2 = ReCapitalizePartName(type).Replace(cContent, cPresentation);
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

        private void SaveChangedLists(Dictionary<string, int?[]> values)
        {
            // ensure that there are never more presentations than values
            if (values.ContainsKey(cPresentation))
            {
                var contentCount = Content.Count;
                if (values.ContainsKey(cContent))
                    contentCount = values[cContent].Length;
                if (values[cPresentation].Length > contentCount)
                    throw new Exception("Presentation may not contain more items than Content.");
            }

            var context = EavDataController.Instance(_zoneId, _appId).Entities; // EavContext.Instance(_zoneId, _appId);
            context.UpdateEntity(_contentGroupEntity.EntityGuid, values);

            // Refresh content group entity (ensures contentgroup is up to date)
            _contentGroupEntity =
                new ContentGroupManager(_zoneId, _appId).GetContentGroup(_contentGroupEntity.EntityGuid)._contentGroupEntity;
        }

        private Dictionary<string, int?[]> PrepareSavePackage(string type, IEnumerable<int?> entityIds,
            Dictionary<string, int?[]> alreadyInitializedDictionary = null)
        {
            type = ReCapitalizePartName(type);
            if (alreadyInitializedDictionary == null) alreadyInitializedDictionary = new Dictionary<string, int?[]>();
            alreadyInitializedDictionary.Add(type, entityIds.ToArray());
            return alreadyInitializedDictionary;
        }


        private string ReCapitalizePartName(string partName)
        {
            partName = partName.ToLower();
            if (partName == cContent.ToLower()) partName = cContent;
            else if (partName == cPresentation.ToLower()) partName = cPresentation;
            else if (partName == cListC.ToLower()) partName = cListC;
            else if (partName == cListP.ToLower()) partName = cListP;
            else throw new Exception("Wanted to capitalize part name - but part name unknown: " + partName);
            return partName;
        }

        /// <summary>
        /// Removes entities from a group. This will also remove the corresponding presentation entities.
        /// </summary>
        /// <param name="contentGroupGuid"></param>
        /// <param name="type">Should be "Content" or "ListContent"</param>
        /// <param name="sortOrder"></param>
        public void RemoveContentAndPresentationEntities(string type, int sortOrder)
        {

            var list1 = ListWithNulls(type);
            list1.RemoveAt(sortOrder);
            var type2 = ReCapitalizePartName(type).Replace(cContent, cPresentation);
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
            if (type.ToLower() != cContent.ToLower())
                throw new Exception("This is only meant to work for content, not for list-content");

            if (!sortOrder.HasValue)
                sortOrder = Content.Count;

            var list1 = ListWithNulls(type);
            list1.Insert(sortOrder.Value, contentId);

            var list2 = GetPresentationIdWithSameLengthAsContent();
            list2.Insert(sortOrder.Value, presentationId);

            SaveChangedLists(PrepareSavePackage(cContent, list1, PrepareSavePackage(cPresentation, list2)));

        }


        private List<int?> GetPresentationIdWithSameLengthAsContent()
        {
            var difference = Content.Count - Presentation.Count;

            if (difference < 0)
                throw new Exception("There are more Presentation elements than Content elements.");

            var entityIds = ListWithNulls(cPresentation); // Presentation.Select(p => p?.EntityId).ToList();

            // extend as necessary
            if (difference != 0)
                entityIds.AddRange(Enumerable.Repeat(new int?(), difference));

            return entityIds; // SaveChangedLists(PrepareSavePackage(cPresentation, entityIds));
        }

        public void ReorderEntities(int sortOrder, int destinationSortOrder)
        {
            var contentIds = ListWithNulls(cContent); 
            var presentationIds = GetPresentationIdWithSameLengthAsContent();

            var contentId = contentIds[sortOrder];
            var presentationId = presentationIds[sortOrder];

            contentIds.RemoveAt(sortOrder);
            presentationIds.RemoveAt(sortOrder);

            contentIds.Insert(destinationSortOrder, contentId);
            presentationIds.Insert(destinationSortOrder, presentationId);

            var list = PrepareSavePackage(cContent, contentIds);
            list = PrepareSavePackage(cPresentation, presentationIds, list);
            SaveChangedLists(list);
        }

        public bool ReorderAll(int[] newSequence)
        {
            var oldCIds = ListWithNulls(cContent);
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

            var list = PrepareSavePackage(cContent, newContentIds);
            list = PrepareSavePackage(cPresentation, newPresIds, list);
            SaveChangedLists(list);

            return true;
        }
    }
}