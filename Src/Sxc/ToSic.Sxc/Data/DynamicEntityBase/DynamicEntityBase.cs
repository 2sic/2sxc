using System.Collections.Generic;
using System.Dynamic;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Debug;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Lib.Logging;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using static System.StringComparer;

namespace ToSic.Sxc.Data
{
    [PrivateApi]
    public abstract partial class DynamicEntityBase : DynamicObject, IDynamicEntityBase, IPropertyLookup, ISxcDynamicObject, ICanDebug
    {
        protected DynamicEntityBase(DynamicEntity.MyServices services, bool strict)
        {
            _Services = services;
            StrictGet = strict;
        }

        // ReSharper disable once InconsistentNaming
        [PrivateApi("Private, but public for debugging in emergencies")]
        public DynamicEntity.MyServices _Services { get; }

        [PrivateApi]
        protected bool StrictGet { get; }

        // ReSharper disable once InconsistentNaming
        protected readonly Dictionary<string, object> _ValueCache = new Dictionary<string, object>(InvariantCultureIgnoreCase);


        public abstract PropReqResult FindPropertyInternal(PropReqSpecs specs, PropertyLookupPath path);

        /// <summary>
        /// Generate a dynamic entity based on an IEntity.
        /// Used in various cases where a property would return an IEntity, and the Razor should be able to continue in dynamic syntax
        /// </summary>
        /// <param name="contents"></param>
        /// <returns></returns>
        protected IDynamicEntity SubDynEntityOrNull(IEntity contents) => SubDynEntityOrNull(contents, _Services, Debug, strictGet: StrictGet);

        internal static IDynamicEntity SubDynEntityOrNull(IEntity contents, DynamicEntity.MyServices services, bool? debug, bool strictGet)
        {
            if (contents == null) return null;
            var result = new DynamicEntity(contents, services, strict: strictGet);
            if (debug == true) result.Debug = true;
            return result;
        }

        #region Debug system

        /// <inheritdoc />
        public bool Debug { get; set; }


        [PrivateApi("Internal")]
        public abstract List<PropertyDumpItem> _Dump(PropReqSpecs specs, string path);

        #endregion

        protected ILog LogOrNull => _logOrNull.Get(() => _Services.AsC?.Log?.SubLogOrNull("DynEnt", Debug));
        private readonly GetOnce<ILog> _logOrNull = new GetOnce<ILog>();

    }
}
