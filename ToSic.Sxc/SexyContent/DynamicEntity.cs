using System;
using System.Dynamic;
using System.Linq;
using System.Web;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.SexyContent.EAVExtensions;
using ToSic.SexyContent.Interfaces;
using ToSic.Sxc.Edit.Toolbar;

namespace ToSic.SexyContent
{
    public class DynamicEntity : DynamicObject, IDynamicEntity
    {
        public ContentConfiguration Configuration = new ContentConfiguration();
        public Eav.Interfaces.IEntity Entity { get; set; }
        public HtmlString Toolbar {
            get
            {
                // if it's neither in a running context nor in a running portal, no toolbar
                // 2018-02-03 2dm: disabled the PortalSettings criteria to decouple from DNN, may have side effects
                if (SxcInstance == null)
                    return new HtmlString("");

                // If we're not in a running context, of which we know the permissions, no toolbar
                var userMayEdit = SxcInstance?.UserMayEdit ?? false;

                if (!userMayEdit)
                    return new HtmlString("");

                var toolbar = new ItemToolbar(Entity).Toolbar;
                return new HtmlString(toolbar);
            }
        }
        private readonly string[] _dimensions;
        internal SxcInstance SxcInstance { get; }   // must be internal for further use cases

        /// <summary>
        /// Constructor with EntityModel and DimensionIds
        /// </summary>
        public DynamicEntity(Eav.Interfaces.IEntity entityModel, string[] dimensions, SxcInstance sexy)
        {
            Entity = entityModel;
            _dimensions = dimensions;
            SxcInstance = sexy;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
            => TryGetMember(binder.Name, out result);

        public bool TryGetMember(string memberName, out object result)
        {
            result = GetEntityValue(memberName, out var propertyNotFound);

            if (propertyNotFound)
                result = null;

            return true;
        }

        public object GetEntityValue(string attributeName, out bool propertyNotFound)
        {
            propertyNotFound = false;   // assume found, as that's usually the case

            #region check the two special cases Toolbar / Presentation which the EAV doesn't know

            if (attributeName == "Toolbar")
                return Toolbar.ToString();

            if (attributeName == AppConstants.Presentation)
                return Presentation;

            #endregion

            object result = null;
            // check Entity is null (in cases where null-objects are asked for properties)
            if (Entity != null)
            {
                result = Entity.GetBestValue(attributeName, _dimensions, true);

                if (result is EntityRelationship rel)
                {
                    var relList = rel.Select(
                        p => new DynamicEntity(p, _dimensions, SxcInstance)
                    ).ToList();
                    result = relList;
                }
            }

            //set out-information
            propertyNotFound = result == null;
            return result;
        }

        private DynamicEntity _presentation;

        private DynamicEntity Presentation
            => _presentation ?? (_presentation = Entity is EntityInContentGroup
                   ? new DynamicEntity(((EntityInContentGroup) Entity).Presentation, _dimensions, SxcInstance)
                   : null);

        /// <summary>
        /// Configuration class for this expando
        /// </summary>
        public class ContentConfiguration
        {
            public string ErrorKeyMissing {
                get => null;
                set => throw new Exception("Obsolete: Do not use ErrorKeyMissing anymore. Check if the value is null instead.");
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