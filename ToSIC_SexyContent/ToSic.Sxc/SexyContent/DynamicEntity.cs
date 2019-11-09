using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.SexyContent.EAVExtensions;
using ToSic.Sxc;
using ToSic.Sxc.Edit.Toolbar;
using IEntity = ToSic.Eav.Data.IEntity;

// ReSharper disable once CheckNamespace
namespace ToSic.SexyContent
{
    [PrivateApi]
    public class DynamicEntity : DynamicObject, IDynamicEntity, IEquatable<IDynamicEntity>
    {

        public IEntity Entity { get; }
        /// <inheritdoc />
        [Obsolete]
        public HtmlString Toolbar {
            get
            {
                // if it's neither in a running context nor in a running portal, no toolbar
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
#pragma warning disable 612
                return Toolbar.ToString();
#pragma warning restore 612

            if (attributeName == Sxc.Views.Parts.Presentation)
                return GetPresentation;

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

        /// <summary>
        /// Get a property using the string name. Only needed in special situations, as most cases can use the object.name directly
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public dynamic Get(string name) => GetEntityValue(name, out _);

        /// <inheritdoc />
        public dynamic Presentation => GetPresentation; 

        private IDynamicEntity GetPresentation
            => _presentation ?? (_presentation = Entity is EntityInContentGroup
                   ? new DynamicEntity(((EntityInContentGroup) Entity).Presentation, _dimensions, SxcInstance)
                   : null);
        private IDynamicEntity _presentation;


        public int EntityId => Entity.EntityId;

        public Guid EntityGuid => Entity.EntityGuid;

        public object EntityTitle => Entity.Title[_dimensions];

        public dynamic GetDraft() => new DynamicEntity(Entity.GetDraft(), _dimensions, SxcInstance);
        
        public dynamic GetPublished() => new DynamicEntity(Entity.GetPublished(), _dimensions, SxcInstance);

        /// <summary>
        /// Tell the system that it's a demo item, not one added by the user
        /// 2019-09-18 trying to mark demo-items for better detection in output #1792
        /// </summary>
        /// <returns></returns>
        public bool IsDemoItem => Entity is EntityInContentGroup entInCg && entInCg.IsDemoItem;

        public IHtmlString Render() => ContentBlocks.Render.One(this);


        #region Changing comparison operation to internally compare the entities, not this wrapper

        public static bool operator ==(DynamicEntity d1, IDynamicEntity d2) => OverrideIsEqual(d1, d2);
        public static bool operator !=(DynamicEntity d1, IDynamicEntity d2) => !OverrideIsEqual(d1, d2);

        /// <summary>
        /// Check if they are equal, based on the underlying entity. 
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <remarks>
        /// It's important to do null-checks first, because if anything in here is null, it will otherwise throw an error. 
        /// But we can't use != null, because that would call the != operator and be recursive.
        /// </remarks>
        /// <returns></returns>
        private static bool OverrideIsEqual(DynamicEntity d1, IDynamicEntity d2)
        {
            // check most basic case - they are really the same object or both null
            if (ReferenceEquals(d1, d2))
                return true;

            return d1?.Entity == d2?.Entity;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;
            if (obj is IDynamicEntity deobj)
                return Entity == deobj.Entity;
            if (obj is IEntity entobj)
                return Entity == entobj;

            return false;
        }

        /// <summary>
        /// This is used by various equality comparison. 
        /// Since we define two DynamicEntities to be equal when they host the same entity, this uses the Entity.HashCode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => Entity != null ? Entity.GetHashCode() : 0;

        public bool Equals(IDynamicEntity dynObj) => Entity == dynObj?.Entity;

        #endregion

        public List<IDynamicEntity> Parents(string type = null, string field = null)
            => Entity.Parents(type, field)
                .Select(e => new DynamicEntity(e, _dimensions, SxcInstance))
                .Cast<IDynamicEntity>()
                .ToList();
    }
}