using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using DotNetNuke.Entities.Portals;
using Newtonsoft.Json;
using ToSic.Eav;
using ToSic.SexyContent.EAVExtensions;
using EntityRelationship = ToSic.Eav.Data.EntityRelationship;

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
                if (SexyContext == null || PortalSettings.Current == null)
                    return new HtmlString("");

                // If we're not in a running context, of which we know the permissions, no toolbar
                if (!SexyContext.Environment.Permissions.UserMayEditContent)
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
        private SexyContent SexyContext { get; set; }

        /// <summary>
        /// Constructor with EntityModel and DimensionIds
        /// </summary>
        public DynamicEntity(IEntity entityModel, string[] dimensions, SexyContent sexy)
        {
            Entity = entityModel;
            _dimensions = dimensions;
            SexyContext = sexy;
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
                        ? new DynamicEntity(inContentGroup.Presentation, _dimensions, SexyContext)
                        : null;
            }
            #endregion


            // 2016-02-27 2dm - fixed to use the full standard ValueConverter - seems to fix some issues
            var result = Entity.GetBestValue(attributeName, _dimensions, true); // let internal resolver do it all now
                                                                                // var result = Entity.GetBestValue(attributeName, _dimensions);

            // set out-information
            propertyNotFound = result == null;
            return result;


            // #region handle 2sxc special conversions for file names and entity-lists

            //if (!propertyNotFound)
            //{
            // if (Entity.Attributes.ContainsKey(attributeName))
            // {
            //  var attribute = Entity.Attributes[attributeName];
            //  if (attribute.Type == "Hyperlink" && result is string)
            //   result = SexyContent.ResolveHyperlinkValues((string) result,
            //    SexyContext == null ? PortalSettings.Current : SexyContext.OwnerPS);

            //  if (attribute.Type == "Entity" && result is EntityRelationship)
            //   // Convert related entities to Dynamics
            //   result = ((EntityRelationship) result).Select(
            //    p => new DynamicEntity(p, _dimensions, SexyContext)
            //    ).ToList();
            // }

            // return result;
            //}
            //#endregion



            //#region all failed, return null
            //propertyNotFound = true;
            //return null;

            //#endregion


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

        public int EntityId
        {
            get { return Entity.EntityId; }
        }

        public Guid EntityGuid
        {
            get { return Entity.EntityGuid; }
        }

        public object EntityTitle
        {
            get { return Entity.Title[_dimensions]; }
        }

        public dynamic GetDraft()
        {
            return new DynamicEntity(Entity.GetDraft(), _dimensions, SexyContext);
        }

        public dynamic GetPublished()
        {
            return new DynamicEntity(Entity.GetPublished(), _dimensions, SexyContext);
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
                if (v.Type == "Entity" && value is List<DynamicEntity>)
                    return ((List<DynamicEntity>)value).Select(p => new { p.EntityId, p.EntityTitle });
                return value;
            }));

            dictionary.Add("EntityId", entity.EntityId);
            dictionary.Add("Modified", entity.Modified);

            if (entity is EntityInContentGroup && !dictionary.ContainsKey("Presentation"))
            {
                var entityInGroup = (EntityInContentGroup)entity;
                if (entityInGroup.Presentation != null)
                {
                    var subItm = new DynamicEntity(entityInGroup, _dimensions, SexyContext);
                    dictionary.Add("Presentation", subItm.ToDictionary());
                }
            }

            if (entity is IHasEditingData)
                dictionary.Add("_2sxcEditInformation", new { sortOrder = ((IHasEditingData)entity).SortOrder });
            else
                dictionary.Add("_2sxcEditInformation", new { entityId = entity.EntityId, title = entity.Title != null ? entity.Title[_dimensions[0]] : "(no title)" });

            return dictionary;
        }

    }
}