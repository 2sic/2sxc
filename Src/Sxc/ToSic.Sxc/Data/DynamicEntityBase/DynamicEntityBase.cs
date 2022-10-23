using System;
using System.Collections.Generic;
using System.Dynamic;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Debug;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Data
{
    public abstract partial class DynamicEntityBase : DynamicObject, IDynamicEntityBase, IPropertyLookup, ISxcDynamicObject, ICanDebug
    {
        protected DynamicEntityBase(DynamicEntityDependencies dependencies) => _Dependencies = dependencies;

        [PrivateApi]
        // ReSharper disable once InconsistentNaming
        public DynamicEntityDependencies _Dependencies { get; }

        // ReSharper disable once InconsistentNaming
        protected readonly Dictionary<string, object> _ValueCache = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);

        /// <inheritdoc />
        public bool Debug { get; set; }


        [PrivateApi("Internal")]
        public abstract PropertyRequest FindPropertyInternal(string field, string[] dimensions, ILog parentLogOrNull, PropertyLookupPath path);
        

        
        /// <summary>
        /// Generate a dynamic entity based on an IEntity.
        /// Used in various cases where a property would return an IEntity, and the Razor should be able to continue in dynamic syntax
        /// </summary>
        /// <param name="contents"></param>
        /// <returns></returns>
        [PrivateApi]
        protected IDynamicEntity SubDynEntityOrNull(IEntity contents) => SubDynEntityOrNull(contents, _Dependencies, Debug);

        internal static IDynamicEntity SubDynEntityOrNull(IEntity contents, DynamicEntityDependencies dependencies, bool? debug)
        {
            if (contents == null) return null;
            var result = new DynamicEntity(contents, dependencies);
            if (debug == true) result.Debug = true; // result.SetDebug(true);
            return result;
        }

        #region WIP Debug system

        [PrivateApi("Internal")]
        public abstract List<PropertyDumpItem> _Dump(string[] languages, string path, ILog parentLogOrNull);

        #endregion

        protected ILog LogOrNull => _logOrNull.Get(() => _Dependencies.LogOrNull?.SubLogOrNull("DynEnt", Debug));
        private readonly GetOnce<ILog> _logOrNull = new GetOnce<ILog>();

    }
}
