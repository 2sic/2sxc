using System;
using System.Dynamic;
using System.Linq;
using System.Web;
using DotNetNuke.Entities.Portals;
using ToSic.Eav.Apps;
using ToSic.SexyContent.EAVExtensions;
using ToSic.SexyContent.Edit.Toolbar;
using ToSic.SexyContent.Interfaces;

namespace ToSic.SexyContent
{


    public class DynamicEntity : DynamicObject, IDynamicEntity
    {
        public ContentConfiguration Configuration = new ContentConfiguration();
        public ToSic.Eav.Interfaces.IEntity Entity { get; set; }
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
        internal SxcInstance SxcInstance { get; }   // must be internal for further use cases

        /// <summary>
        /// Constructor with EntityModel and DimensionIds
        /// </summary>
        public DynamicEntity(ToSic.Eav.Interfaces.IEntity entityModel, string[] dimensions, SxcInstance sexy)
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

            if (attributeName == AppConstants.Presentation)
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

        public IHtmlString Render() => ContentBlocks.Render.One(this);
        
    }

}