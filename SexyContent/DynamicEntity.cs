using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using DotNetNuke.Entities.Portals;
using ToSic.Eav;
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
                result = null; // String.Format(Configuration.ErrorKeyMissing, memberName);

            return true;
        }


        public object GetEntityValue(string attributeName, out bool propertyNotFound)
        {
            propertyNotFound = false;   // assume found, as that's usually the case

            #region check the two special cases which the EAV doesn't know
            switch (attributeName)
            {
                case "Toolbar":
                    return Toolbar.ToString();
                case Constants.PresentationKey:
                    // todo: move out, as this code returns a _different_ presentation object each time!
                    var inContentGroup = Entity as EntityInContentGroup;
                    return (inContentGroup != null)
                        ? new DynamicEntity(inContentGroup.Presentation, _dimensions, SxcInstance)
                        : null;
            }
            #endregion


            // 2016-02-27 2dm - fixed to use the full standard ValueConverter - seems to fix some issues
            var result = Entity.GetBestValue(attributeName, _dimensions, true);

            if (result is Eav.Data.EntityRelationship)
            {
                var relList = ((Eav.Data.EntityRelationship) result).Select(
                    p => new DynamicEntity(p, _dimensions, SxcInstance)
                    ).ToList();
                result = relList;// new DynamicEntityList(Entity, attributeName, relList, _sxcInstance.Environment.Permissions.UserMayEditContent);
            }

            //set out-information
            propertyNotFound = result == null;
            return result;




        }

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
            // temp solution till we remove all dupl. code
            var ser = new Serializer(SxcInstance, _dimensions);

            var dynamicEntity = this;
            var entity = Entity;
            bool propertyNotFound;

            // Convert DynamicEntity to dictionary
            var dictionary = ((from d in entity.Attributes select d.Value).ToDictionary(k => k.Name, v =>
            {
                var value = dynamicEntity.GetEntityValue(v.Name, out propertyNotFound);
                return (v.Type == "Entity" && value is IList<DynamicEntity>)
                    ? ((IList<DynamicEntity>) value).Select(p => new {p.EntityId, p.EntityTitle}).ToList()  // convert to a light list for optimal serialization
                    : value;
            }));

            var dic2 = ser.GetDictionaryFromEntity(entity);

            dictionary.Add(Constants.JsonEntityIdNodeName, entity.EntityId);

            ser.AddPresentation(entity, dictionary);
            ser.AddEditInfo(entity, dictionary);

            return dictionary;
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