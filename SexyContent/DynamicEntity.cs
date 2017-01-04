using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using DotNetNuke.Entities.Portals;
using ToSic.Eav;
using ToSic.Eav.Serializers;
using ToSic.SexyContent.ContentBlock;
using ToSic.SexyContent.EAVExtensions;
using ToSic.SexyContent.Edit.Toolbar;
using ToSic.SexyContent.Interfaces;
using Serializer = ToSic.SexyContent.Serializers.Serializer;

namespace ToSic.SexyContent
{


    public class DynamicEntity : DynamicObject, IDynamicEntity
    {
        public ContentConfiguration Configuration = new ContentConfiguration();
        public IEntity Entity { get; set; }
        public HtmlString Toolbar {
            get
            {
                // if it's neither in a running context nor in a running portal, no toolbar
                if (SxcInstance == null || PortalSettings.Current == null)
                    return new HtmlString("");

                // If we're not in a running context, of which we know the permissions, no toolbar
                if (!SxcInstance.Environment.Permissions.UserMayEditContent)
                    return new HtmlString("");

                var toolbar = new ItemToolbar(this).Toolbar;
                return new HtmlString(toolbar);

                //if (Entity is IHasEditingData)
                //    return new HtmlString("<ul class=\"sc-menu\" data-toolbar='"
                //                          + JsonConvert.SerializeObject(new
                //                          {
                //                              sortOrder = ((IHasEditingData) Entity).SortOrder,
                //                              useModuleList = true,
                //                              isPublished = Entity.IsPublished
                //                          }) 
                //                          + "'></ul>");

                //return new HtmlString("<ul class=\"sc-menu\" data-toolbar='"
                //                      + JsonConvert.SerializeObject(new
                //                      {
                //                          entityId = Entity.EntityId,
                //                          isPublished = Entity.IsPublished,
                //                          contentType = Entity.Type.Name
                //                      })
                //                      + "'></ul>");
            }
        }
        private readonly string[] _dimensions;
        private SxcInstance SxcInstance { get; }

        /// <summary>
        /// Constructor with EntityModel and DimensionIds
        /// </summary>
        public DynamicEntity(IEntity entityModel, string[] dimensions, SxcInstance sexy)
        {
            Entity = entityModel;
            _dimensions = dimensions;
            SxcInstance = sexy;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
            => TryGetMember(binder.Name, out result);

        public bool TryGetMember(string memberName, out object result)
        {
            bool propertyNotFound;
            result = GetEntityValue(memberName, out propertyNotFound);

            if (propertyNotFound)
                result = null;

            return true;
        }


        public object GetEntityValue(string attributeName, out bool propertyNotFound)
        {
            propertyNotFound = false;   // assume found, as that's usually the case

            #region check the two special cases which the EAV doesn't know

            if (attributeName == "Toolbar")
                return Toolbar.ToString();

            if (attributeName == Constants.PresentationKey)
                return Presentation;

            #endregion


            var result = Entity.GetBestValue(attributeName, _dimensions, true);

            if (result is Eav.Data.EntityRelationship)
            {
                var relList = ((Eav.Data.EntityRelationship) result).Select(
                    p => new DynamicEntity(p, _dimensions, SxcInstance)
                    ).ToList();
                result = relList;
            }

            //set out-information
            propertyNotFound = result == null;
            return result;
        }

        private DynamicEntity _presentation;
        private DynamicEntity Presentation => _presentation ??
                                              (_presentation = (Entity is EntityInContentGroup)
                                                  ? new DynamicEntity(((EntityInContentGroup) Entity).Presentation, _dimensions, SxcInstance)
                                                  : null);

        /// <summary>
        /// Configuration class for this expando
        /// </summary>
        public class ContentConfiguration
        {
            public string ErrorKeyMissing {
                get { return null; }
                set
                {
                    throw new Exception("Obsolete: Do not use ErrorKeyMissing anymore. Check if the value is null instead.");
                }
            }
        }

        public int EntityId => Entity.EntityId;

        public Guid EntityGuid => Entity.EntityGuid;

        public object EntityTitle => Entity.Title[_dimensions];

        public dynamic GetDraft() => new DynamicEntity(Entity.GetDraft(), _dimensions, SxcInstance);
        
        public dynamic GetPublished() => new DynamicEntity(Entity.GetPublished(), _dimensions, SxcInstance);

        internal Dictionary<string, object> ToDictionary()
        {
            // 2017-01-04 new serialization, prevent dupl. code
            var ser = new Serializer(SxcInstance, _dimensions);
            var dicNew = ser.GetDictionaryFromEntity(Entity);
            var dicToSerialize = ser.ConvertNewSerRelToOldSerRel(dicNew);

            #region Old Serialization and test-code to see if new version is the same
            // 2017-01-04 old code, mostly duplicate of serialization 
            // Convert DynamicEntity to dictionary
            //var dynamicEntity = this;
            //var entity = Entity;
            //bool propertyNotFound;
            //var dicOldMethod = ((from d in entity.Attributes select d.Value).ToDictionary(k => k.Name, v =>
            //{
            //    var value = dynamicEntity.GetEntityValue(v.Name, out propertyNotFound);
            //    return (v.Type == "Entity" && value is IList<DynamicEntity>)
            //        ? ((IList<DynamicEntity>)value).Select(p => new SerializableRelationshipOld() { EntityId = p.EntityId, EntityTitle = p.EntityTitle }).ToList()  // convert to a light list for optimal serialization
            //        : value;
            //}));

            // 2017-01-04 test to compare old serialization with newer - see if identical
            //foreach (var key in dicToSerialize.Keys)
            //{
            //    if (!dicNew.ContainsKey(key))
            //        throw new Exception("key not found in target");
            //    var e1 = dicToSerialize[key];
            //    var e2 = dicNew[key];
            //    var origlist = e1 as List<SerializableRelationshipOld>;
            //    var origrel = origlist?.FirstOrDefault();
            //    var secondlist = e2 as List<SerializableRelationshipOld>;
            //    var secondrel = secondlist?.FirstOrDefault();
            //    if (origrel != null && secondrel != null)
            //    {
            //        if (!(
            //            origrel.EntityId == secondrel.EntityId && origrel.EntityTitle == secondrel.EntityTitle))
            //            throw new Exception("relationship with different stuff");
            //    }
            //    else if (e1 != null && e2 != null && !e1.Equals(e2))
            //        if (key != "Languages") // special case, skip testing this
            //            throw new Exception("not full match on " + key + " orig is " + e1);
            //}
            //if (dicNew.Keys.Any(key => key != "Id" && key != "Title" && !dicToSerialize.ContainsKey(key)))
            //    throw new Exception("key in 2 not found in original");
            #endregion

            dicToSerialize.Add(Constants.JsonEntityIdNodeName, Entity.EntityId);

            //ser.AddPresentation(Entity, dicToSerialize);
            //ser.AddEditInfo(Entity, dicToSerialize);

            return dicToSerialize;
        }


        public HtmlString Render()
        {
            if (Entity.Type.Name == Settings.AttributeSetStaticNameContentBlockTypeName)
            {
                var cb = new EntityContentBlock(SxcInstance.ContentBlock, Entity);
                return cb.SxcInstance.Render();
            }

            return new HtmlString("<!-- auto-render of item " + EntityId + " -->");
        }
    }

}