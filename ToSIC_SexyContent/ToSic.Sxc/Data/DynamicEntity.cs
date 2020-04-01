using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using ToSic.Eav.Documentation;
using ToSic.Eav.Run;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Edit.Toolbar;
using IEntity = ToSic.Eav.Data.IEntity;
// ReSharper disable InheritdocInvalidUsage

namespace ToSic.Sxc.Data
{
    /// <summary>
    /// A dynamic entity object - the main object you use when templating things in RazorComponent objects <br/>
    /// Note that it will provide many things not listed here, usually things like `.Image`, `.FirstName` etc. based on your ContentType.
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public class DynamicEntity : DynamicObject, IDynamicEntity, IEquatable<IDynamicEntity>, ICompatibilityLevel
    {
        [PrivateApi]
        public IEntity Entity { get; protected set; }

        [PrivateApi]
        public int CompatibilityLevel { get; }

        /// <inheritdoc />
        [Obsolete("use Edit.Toolbar instead")]
        [PrivateApi]
        public HtmlString Toolbar {
            get
            {
                // if it's neither in a running context nor in a running portal, no toolbar
                if (BlockBuilder == null)
                    return new HtmlString("");

                // If we're not in a running context, of which we know the permissions, no toolbar
                var userMayEdit = BlockBuilder?.UserMayEdit ?? false;

                if (!userMayEdit)
                    return new HtmlString("");

                if(CompatibilityLevel == 10)
                    throw new Exception("content.Toolbar is deprecated in the new RazorComponent. Use @Edit.TagToolbar(content) or @Edit.Toolbar(content) instead. See https://r.2sxc.org/EditToolbar");

                var toolbar = new ItemToolbar(Entity).Toolbar;
                return new HtmlString(toolbar);
            }
        }

        [PrivateApi]
        protected readonly string[] Dimensions;
        [PrivateApi]
        internal IBlockBuilder BlockBuilder { get; }   // must be internal for further use cases

        /// <summary>
        /// Constructor with EntityModel and DimensionIds
        /// </summary>
        [PrivateApi]
        public DynamicEntity(IEntity entity, string[] dimensions, int compatibility, IBlockBuilder blockBuilder)
        {
            Entity = entity;
            Dimensions = dimensions;
            CompatibilityLevel = compatibility;
            BlockBuilder = blockBuilder;
        }

        /// <inheritdoc />
        public override bool TryGetMember(GetMemberBinder binder, out object result)
            => TryGetMember(binder.Name, out result);

        /// <inheritdoc />
        public bool TryGetMember(string memberName, out object result)
        {
            result = GetEntityValue(memberName);//, out var propertyNotFound);
            //if (propertyNotFound) result = null;
            return true;
        }

        [PrivateApi]
        public object GetEntityValue(string attributeName)
        {
            #region check the two special cases Toolbar / Presentation which the EAV doesn't know

            if (attributeName == "Toolbar")
#pragma warning disable 612
#pragma warning disable 618
                return Toolbar.ToString();
#pragma warning restore 618
#pragma warning restore 612

            if (attributeName == ViewParts.Presentation)
                return GetPresentation;

            #endregion

            // check Entity is null (in cases where null-objects are asked for properties)
            if (Entity == null) return null;

            // check if we already have it in the cache
            if (_valCache.ContainsKey(attributeName)) return _valCache[attributeName];

            var result = Entity.GetBestValue(attributeName, Dimensions, true);
            if (result is IEnumerable<IEntity> rel)
                result = new DynamicEntityWithList(rel, Dimensions, CompatibilityLevel, BlockBuilder);

            _valCache.Add(attributeName, result);
            return result;
        }

        private readonly Dictionary<string, object> _valCache = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// Get a property using the string name. Only needed in special situations, as most cases can use the object.name directly
        /// </summary>
        /// <param name="name">the property name. </param>
        /// <returns>a dynamically typed result, can be string, bool, etc.</returns>
        public dynamic Get(string name) => GetEntityValue(name);

        /// <inheritdoc />
        [PrivateApi("should use Content.Presentation")]
        [Obsolete("should use Content.Presentation")]
        public dynamic Presentation => GetPresentation; 

        private IDynamicEntity GetPresentation
            => _presentation ?? (_presentation = Entity is EntityInBlock entityInGroup
                   ? new DynamicEntity(entityInGroup.Presentation, Dimensions, CompatibilityLevel, BlockBuilder)
                   : null);
        private IDynamicEntity _presentation;


        /// <inheritdoc />
        /// <remarks>If the entity doesn't exist, it will return 0</remarks>
        public int EntityId => Entity?.EntityId ?? 0;

        /// <inheritdoc />
        /// <remarks>If the entity doesn't exist, it will return an empty guid</remarks>
        public Guid EntityGuid => Entity?.EntityGuid ?? Guid.Empty;

        /// <inheritdoc />
        /// <remarks>If the entity doesn't exist, it will return null</remarks>
        public object EntityTitle => Entity?.Title[Dimensions];

        /// <inheritdoc />
        public dynamic GetDraft() => new DynamicEntity(Entity?.GetDraft(), Dimensions, CompatibilityLevel, BlockBuilder);
        
        /// <inheritdoc />
        public dynamic GetPublished() => new DynamicEntity(Entity?.GetPublished(), Dimensions, CompatibilityLevel, BlockBuilder);

        /// <summary>
        /// Tell the system that it's a demo item, not one added by the user
        /// 2019-09-18 trying to mark demo-items for better detection in output #1792
        /// </summary>
        /// <returns></returns>
        public bool IsDemoItem => Entity is EntityInBlock entInCg && entInCg.IsDemoItem;

        [Obsolete]
        [PrivateApi("probably we won't continue recommending to use this, but first we must provide an alternative")]
        public IHtmlString Render()
        {
            if(CompatibilityLevel == 10)
                throw new Exception("content.Render() is deprecated in the new RazorComponent. Use ToSic.Sxc.Blocks.Render.One(content) instead. See https://r.2sxc.org/EditToolbar");

            return Blocks.Render.One(this);
        }


        #region Changing comparison operation to internally compare the entities, not this wrapper
        /// <inheritdoc />
        public static bool operator ==(DynamicEntity d1, IDynamicEntity d2) => OverrideIsEqual(d1, d2);
        /// <inheritdoc />
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

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj))
                return true;
            if (obj is IDynamicEntity deObj)
                return Entity == deObj.Entity;
            if (obj is IEntity entObj)
                return Entity == entObj;

            return false;
        }

        /// <summary>
        /// This is used by various equality comparison. 
        /// Since we define two DynamicEntities to be equal when they host the same entity, this uses the Entity.HashCode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => Entity != null ? Entity.GetHashCode() : 0;

        /// <inheritdoc />
        public bool Equals(IDynamicEntity dynObj) => Entity == dynObj?.Entity;

        #endregion

        /// <inheritdoc />
        public List<IDynamicEntity> Parents(string type = null, string field = null)
            => Entity.Parents(type, field)
                .Select(e => new DynamicEntity(e, Dimensions, CompatibilityLevel, BlockBuilder))
                .Cast<IDynamicEntity>()
                .ToList();

        /// <inheritdoc />
        public List<IDynamicEntity> Children(string field = null, string type = null)
            => Entity.Children(field, type)
                .Select(e => new DynamicEntity(e, Dimensions, CompatibilityLevel, BlockBuilder))
                .Cast<IDynamicEntity>()
                .ToList();
    }
}