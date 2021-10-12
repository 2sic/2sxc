using System;
using System.Collections.Generic;
using System.Dynamic;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Debug;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Data
{
    public abstract partial class DynamicEntityBase : DynamicObject, IDynamicEntityBase, IPropertyLookup, ISxcDynamicObject
    {
        protected DynamicEntityBase(DynamicEntityDependencies dependencies) => _Dependencies = dependencies;

        [PrivateApi]
        // ReSharper disable once InconsistentNaming
        public DynamicEntityDependencies _Dependencies { get; }

        // ReSharper disable once InconsistentNaming
        protected readonly Dictionary<string, object> _ValueCache = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);

        /// <inheritdoc />
        public void SetDebug(bool debug) => _debug = debug;
        protected bool _debug;
        


        [PrivateApi("Internal")]
        public abstract PropertyRequest FindPropertyInternal(string field, string[] dimensions, ILog parentLogOrNull);
        

        
        /// <summary>
        /// Generate a dynamic entity based on an IEntity.
        /// Used in various cases where a property would return an IEntity, and the Razor should be able to continue in dynamic syntax
        /// </summary>
        /// <param name="contents"></param>
        /// <returns></returns>
        [PrivateApi]
        protected IDynamicEntity SubDynEntityOrNull(IEntity contents) => SubDynEntityOrNull(contents, _Dependencies, _debug);

        internal static IDynamicEntity SubDynEntityOrNull(IEntity contents, DynamicEntityDependencies dependencies, bool? debug)
        {
            if (contents == null) return null;
            var result = new DynamicEntity(contents, dependencies);
            if(debug == true) result.SetDebug(true);
            return result;
        }

        #region WIP Debug system

        [PrivateApi("Internal")]
        public abstract List<PropertyDumpItem> _Dump(string[] languages, string path, ILog parentLogOrNull);

        #endregion
    }
}
