using System;
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
                if (SexyContext == null || PortalSettings.Current == null)
                    return new HtmlString("");

                if (Entity is IHasEditingData)
                    return new HtmlString("<ul class='sc-menu' data-toolbar='" + JsonConvert.SerializeObject(new { sortOrder = ((IHasEditingData) Entity).SortOrder, useModuleList = true, isPublished = Entity.IsPublished }) + "'></ul>");

                return new HtmlString("<ul class='sc-menu' data-toolbar='" + JsonConvert.SerializeObject(new { entityId = Entity.EntityId, isPublished = Entity.IsPublished, attributeSetName = Entity.Type.Name }) + "'></ul>");
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


        // todo urgent: try to inherit the GetBestValue from the new entity-object and skip most of the code, as it's almost identical...

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
                        ? inContentGroup.Presentation
                        : null;
            }
            #endregion

            // new implementation based on revised EAV API
            var result = Entity.GetBestValue(attributeName, _dimensions);//, out propertyNotFound);
            propertyNotFound = result == null;

            #region handle 2sxc special conversions for file names and entity-lists

            if (!propertyNotFound)
            {
                var attribute = Entity.Attributes[attributeName];
                if (attribute.Type == "Hyperlink" && result is string)
                    result = SexyContent.ResolveHyperlinkValues((string) result,
                        SexyContext == null ? PortalSettings.Current : SexyContext.OwnerPS);

                if (attribute.Type == "Entity" && result is EntityRelationship)
                    // Convert related entities to Dynamics
                    result = ((EntityRelationship) result).Select(
                        p => new DynamicEntity(p, _dimensions, SexyContext)
                        ).ToList();
                return result;
            }
            #endregion

            #region all failed, return null
            propertyNotFound = true;
            return null;
            #endregion

            //if (Entity.Attributes.ContainsKey(attributeName))
            //{
            //    var attribute = Entity.Attributes[attributeName];
            //    result = attribute[_dimensions];

            //    if (attribute.Type == "Hyperlink" && result is string)
            //    {
            //        result = SexyContent.ResolveHyperlinkValues((string) result, SexyContext == null ? PortalSettings.Current : SexyContext.OwnerPS);
            //    }
            //    else if (attribute.Type == "Entity" && result is EntityRelationshipModel)
            //    {
            //        // Convert related entities to Dynamics
            //        result = ((ToSic.Eav.EntityRelationshipModel) result).Select(
            //            p => new DynamicEntity(p, _dimensions, this.SexyContext)
            //            ).ToList();
            //    }
            //}
            //else
            //{
            //    switch (attributeName)
            //    {
            //        case "EntityTitle":
            //            result = EntityTitle;
            //            break;
            //        case "EntityId":
            //            result = EntityId;
            //            break;
            //        case "Toolbar":
            //            result = Toolbar.ToString();
            //            break;
            //        case "IsPublished":
            //            result = Entity.IsPublished;
            //            break;
            //        case "Modified":
            //            result = Entity.Modified;
            //            break;
            //        case "Presentation":
            //            var inContentGroup = Entity as EntityInContentGroup;
            //            if(inContentGroup != null)
            //                result = inContentGroup.Presentation;
            //            break;
            //        default:
            //            result = null;
            //            propertyNotFound = true;
            //            break;
            //    }
            //}

            //return result;
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

    }
}