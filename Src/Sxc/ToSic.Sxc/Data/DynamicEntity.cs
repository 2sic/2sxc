using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Run;
using ToSic.Eav.Run.Basic;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Edit.Toolbar;
using IEntity = ToSic.Eav.Data.IEntity;
#if NET451
using HtmlString = System.Web.HtmlString;
using IHtmlString = System.Web.IHtmlString;
#else
using HtmlString = Microsoft.AspNetCore.Html.HtmlString;
using IHtmlString = Microsoft.AspNetCore.Html.IHtmlContent;
#endif



namespace ToSic.Sxc.Data
{
    /// <summary>
    /// A dynamic entity object - the main object you use when templating things in RazorComponent objects <br/>
    /// Note that it will provide many things not listed here, usually things like `.Image`, `.FirstName` etc. based on your ContentType.
    /// </summary>
    [PublicApi_Stable_ForUseInYourCode]
    public partial class DynamicEntity : DynamicObject, IDynamicEntity, ICompatibilityLevel
    {
        [PrivateApi]
        public IEntity Entity { get; private set; }

        [PrivateApi]
        public int CompatibilityLevel { get; }

        /// <inheritdoc />
        [Obsolete("use Edit.Toolbar instead")]
        [PrivateApi]
        public HtmlString Toolbar {
            get
            {
                // if it's neither in a running context nor in a running portal, no toolbar
                if (Block == null)
                    return new HtmlString("");

                // If we're not in a running context, of which we know the permissions, no toolbar
                var userMayEdit = Block?.Context.EditAllowed ?? false;

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

        [PrivateApi("Keep internal only - should never surface")]
        internal IBlock Block { get; }

        /// <summary>
        /// Constructor with EntityModel and DimensionIds
        /// </summary>
        [PrivateApi]
        public DynamicEntity(IEntity entity, string[] dimensions, int compatibility, IBlock block)
        {
            SetEntity(entity);
            Dimensions = dimensions;
            CompatibilityLevel = compatibility;
            Block = block;
            ServiceProviderOrNull = Block?.Context?.ServiceProvider;
        }

        [PrivateApi]
        protected void SetEntity(IEntity entity)
        {
            Entity = entity;
            _EntityForEqualityCheck = (Entity as IEntityWrapper)?._EntityForEqualityCheck ?? Entity;
        }

        /// <summary>
        /// Very internal implementation - we need this to allow the IValueProvider to be created, and normally it's provided by the Block context.
        /// But in rare cases (like when the App.Resources is a DynamicEntity) it must be injected separately.
        /// </summary>
        [PrivateApi]
        internal IServiceProvider ServiceProviderOrNull;

        /// <inheritdoc />
        [PrivateApi]
        public override bool TryGetMember(GetMemberBinder binder, out object result)
            => TryGetMember(binder.Name, out result);

        [PrivateApi]
        public bool TryGetMember(string memberName, out object result)
        {
            result = GetEntityValue(memberName);
            return true;
        }

        [PrivateApi]
        public object GetEntityValue(string field)
        {
            #region check the two special cases Toolbar / Presentation which the EAV doesn't know

            if (field == "Toolbar")
#pragma warning disable 612
#pragma warning disable 618
                return Toolbar.ToString();
#pragma warning restore 618
#pragma warning restore 612

            if (field == ViewParts.Presentation)
                return GetPresentation;

            #endregion

            // check Entity is null (in cases where null-objects are asked for properties)
            if (Entity == null) return null;

            // check if we already have it in the cache
            if (_valCache.ContainsKey(field)) return _valCache[field];

            var result = Entity.GetBestValue(field, Dimensions/*, true*/);

            // New mechanism to not use resolve-hyperlink
            if (result is string strResult 
                && BasicValueConverter.CouldBeReference(strResult)
                && Entity.Attributes.ContainsKey(field) &&
                Entity.Attributes[field].Type == Eav.Constants.DataTypeHyperlink)
            {
                result = ServiceProviderOrNull?.Build<IValueConverter>()?.ToValue(strResult, EntityGuid) ??
                      result;
            }

            if (result is IEnumerable<IEntity> rel)
            {
                var list = new DynamicEntityWithList(Entity, field, rel, Dimensions, CompatibilityLevel, Block);
                // special case: if it's a Dynamic Entity without block (like App.Settings)
                // it needs the Service Provider from this object to work
                if (Block == null) list.ServiceProviderOrNull = ServiceProviderOrNull;
                result = list;
            }

            _valCache.Add(field, result);
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
        public dynamic Presentation => GetPresentation; 

        private IDynamicEntity GetPresentation
            => _presentation ?? (_presentation = Entity is EntityInBlock entityInGroup
                   ? SubDynEntity(entityInGroup.Presentation) // new DynamicEntity(entityInGroup.Presentation, Dimensions, CompatibilityLevel, Block)
                   : null);
        private IDynamicEntity _presentation;

        [PrivateApi]
        protected IDynamicEntity SubDynEntity(IEntity contents)
        {
            if (contents == null) return null;
            var child = new DynamicEntity(contents, Dimensions, CompatibilityLevel, Block);
            // special case: if it's a Dynamic Entity without block (like App.Settings)
            // it needs the Service Provider from this object to work
            if (Block == null && ServiceProviderOrNull != null) child.ServiceProviderOrNull = ServiceProviderOrNull;
            return child;
        }


        /// <inheritdoc />
        public int EntityId => Entity?.EntityId ?? 0;

        /// <inheritdoc />
        public Guid EntityGuid => Entity?.EntityGuid ?? Guid.Empty;

        /// <inheritdoc />
        public object EntityTitle => Entity?.Title[Dimensions];

        /// <inheritdoc />
        public dynamic GetDraft() => new DynamicEntity(Entity?.GetDraft(), Dimensions, CompatibilityLevel, Block);
        
        /// <inheritdoc />
        public dynamic GetPublished() => new DynamicEntity(Entity?.GetPublished(), Dimensions, CompatibilityLevel, Block);

        /// <inheritdoc />
        public bool IsDemoItem => Entity is EntityInBlock entInCg && entInCg.IsDemoItem;

        [Obsolete]
        [PrivateApi("probably we won't continue recommending to use this, but first we must provide an alternative")]
        public IHtmlString Render()
        {
            if(CompatibilityLevel == 10)
                throw new Exception("content.Render() is deprecated in the new RazorComponent. Use ToSic.Sxc.Blocks.Render.One(content) instead. See https://r.2sxc.org/EditToolbar");

            return Blocks.Render.One(this);
        }


        /// <inheritdoc />
        public List<IDynamicEntity> Parents(string type = null, string field = null)
            => Entity.Parents(type, field)
                .Select(e => new DynamicEntity(e, Dimensions, CompatibilityLevel, Block))
                .Cast<IDynamicEntity>()
                .ToList();

        /// <inheritdoc />
        public List<IDynamicEntity> Children(string field = null, string type = null)
            => Entity.Children(field, type)
                .Select(e => new DynamicEntity(e, Dimensions, CompatibilityLevel, Block))
                .Cast<IDynamicEntity>()
                .ToList();
    }
}