using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using DotNetNuke.Entities.Portals;
using Newtonsoft.Json;
using ToSic.Eav;
using ToSic.SexyContent.ContentBlock;
using ToSic.SexyContent.EAVExtensions;
using static System.String;

namespace ToSic.SexyContent
{
    public class DynamicEntity : DynamicObject
    {
        public ContentConfiguration Configuration = new ContentConfiguration();
        public IEntity Entity { get; set; }
        public HtmlString Toolbar {
            get
            {
                // if it's neither in a running context nor in a running portal, no toolbar
                if (_sxcInstance == null || PortalSettings.Current == null)
                    return new HtmlString("");

                // If we're not in a running context, of which we know the permissions, no toolbar
                if (!_sxcInstance.Environment.Permissions.UserMayEditContent)
                    return new HtmlString("");

                if (Entity is IHasEditingData)
                    return new HtmlString("<ul class=\"sc-menu\" data-toolbar='"
                                          + JsonConvert.SerializeObject(new
                                          {
                                              sortOrder = ((IHasEditingData) Entity).SortOrder,
                                              useModuleList = true,
                                              isPublished = Entity.IsPublished
                                          }) 
                                          + "'></ul>");

                return new HtmlString("<ul class=\"sc-menu\" data-toolbar='"
                                      + JsonConvert.SerializeObject(new
                                      {
                                          entityId = Entity.EntityId,
                                          isPublished = Entity.IsPublished,
                                          contentType = Entity.Type.Name
                                      })
                                      + "'></ul>");
            }
        }
        private readonly string[] _dimensions;
        private SxcInstance _sxcInstance { get; }

        /// <summary>
        /// Constructor with EntityModel and DimensionIds
        /// </summary>
        public DynamicEntity(IEntity entityModel, string[] dimensions, SxcInstance sexy)
        {
            Entity = entityModel;
            _dimensions = dimensions;
            _sxcInstance = sexy;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return TryGetMember(binder.Name, out result);
        }

        public bool TryGetMember(string memberName, out object result)
        {
            var propertyNotFound = false;
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
                case "Presentation":
                    var inContentGroup = Entity as EntityInContentGroup;
                    return (inContentGroup != null)
                        ? new DynamicEntity(inContentGroup.Presentation, _dimensions, _sxcInstance)
                        : null;
            }
            #endregion


            // 2016-02-27 2dm - fixed to use the full standard ValueConverter - seems to fix some issues
            var result = Entity.GetBestValue(attributeName, _dimensions, true);

            if (result is Eav.Data.EntityRelationship)
            {
                var relList = ((Eav.Data.EntityRelationship) result).Select(
                    p => new DynamicEntity(p, _dimensions, _sxcInstance)
                    ).ToList();
                result = new DynamicEntityList(Entity, attributeName, relList, _sxcInstance.Environment.Permissions.UserMayEditContent);
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

        public dynamic GetDraft()
        {
            return new DynamicEntity(Entity.GetDraft(), _dimensions, _sxcInstance);
        }

        public dynamic GetPublished()
        {
            return new DynamicEntity(Entity.GetPublished(), _dimensions, _sxcInstance);
        }

        internal Dictionary<string, object> ToDictionary()
        {
            var dynamicEntity = this;
            var entity = this.Entity;
            // var dynamicEntity = new DynamicEntity(entity, new[] { language }, this);
            bool propertyNotFound;

            // Convert DynamicEntity to dictionary
            var dictionary = ((from d in entity.Attributes select d.Value).ToDictionary(k => k.Name, v =>
            {
                var value = dynamicEntity.GetEntityValue(v.Name, out propertyNotFound);
                if (v.Type == "Entity" && value is IList<DynamicEntity>)
                    return ((IList<DynamicEntity>)value).Select(p => new { p.EntityId, p.EntityTitle });
                return value;
            }));

            dictionary.Add("EntityId", entity.EntityId);
            dictionary.Add("Modified", entity.Modified);

            if (entity is EntityInContentGroup && !dictionary.ContainsKey("Presentation"))
            {
                var entityInGroup = (EntityInContentGroup)entity;
                if (entityInGroup.Presentation != null)
                {
                    var subItm = new DynamicEntity(entityInGroup, _dimensions, _sxcInstance);
                    dictionary.Add("Presentation", subItm.ToDictionary());
                }
            }

            if (entity is IHasEditingData)
                dictionary.Add("_2sxcEditInformation", new { sortOrder = ((IHasEditingData)entity).SortOrder });
            else
                dictionary.Add("_2sxcEditInformation", new { entityId = entity.EntityId, title = entity.Title != null ? entity.Title[_dimensions[0]] : "(no title)" });

            return dictionary;
        }

        public HtmlString Render()
        {
            if (Entity.Type.Name == Settings.AttributeSetStaticNameContentBlockTypeName)
            {
                var cb = new EntityContentBlock(_sxcInstance.ContentBlock, Entity);
                return cb.SxcInstance.Render();
            }
            else
                return new HtmlString("<!-- auto-render of item " + EntityId + " -->");
        }
    }



}