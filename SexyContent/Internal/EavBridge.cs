using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav;
using ToSic.Eav.BLL;
using ToSic.SexyContent.Interfaces;

namespace ToSic.SexyContent.Internal
{
    /// <summary>
    /// Helper construct which ensures that the current user will be in the 
    /// revision log on changes done with the EavContext
    /// </summary>
    internal class EavBridge
    {
        private readonly EavDataController _eavContext;
        private readonly Environment.Environment _env = new Environment.Environment();

        #region constructors
        public EavBridge(IApp app) : this(app.ZoneId, app.AppId)
        {
        }

        public EavBridge(int zoneId, int appId)
        {
            _eavContext = EavDataController.Instance(zoneId, appId);
            _eavContext.UserName = _env.User.CurrentUserIdentityToken;// Environment.Dnn7.UserIdentity.CurrentUserIdentityToken;
        }
        #endregion

        public bool Publish(int repositoryId, bool state)
        {
            _eavContext.Publishing.PublishDraftInDbEntity(repositoryId, state);
            return state;
        }

        public void AddOrUpdateLanguage(string cultureCode, string cultureText, bool active)
            => _eavContext.Dimensions.AddOrUpdateLanguage(cultureCode, cultureText, active);

        public EavDataController FullController => _eavContext;

        #region Special upgrade one-time helper
        /// <summary>
        /// Special method used in the V6 upgrade
        /// shouldn't be used after that, as a shared-attribute was added later on
        /// note: could be removed, if the v6 upgrade would receive the now supported attribute
        /// </summary>
        /// <param name="name"></param>
        internal static void ForceShareOnGlobalContentType(string name)
        {
            var metaDataCtx = new EavBridge(Eav.Constants.DefaultZoneId, Eav.Constants.MetaDataAppId).FullController;
            metaDataCtx.AttribSet.GetAttributeSet(name).AlwaysShareConfiguration = true;
            metaDataCtx.SqlDb.SaveChanges();
        }
        #endregion

        #region Data-level entity actions

        public void EntityUpdate(int id, Dictionary<string, object> values)
            => _eavContext.Entities.UpdateEntity(id, values);

        public Tuple<int, Guid> EntityCreate(string typeName, Dictionary<string, object> values, Guid? entityGuid = null)
        {
            var contentType = DataSource.GetCache(_eavContext.ZoneId, _eavContext.AppId).GetContentType(typeName);
            var ent = _eavContext.Entities.AddEntity(contentType.AttributeSetId, values, null, null, entityGuid: entityGuid);
            return new Tuple<int, Guid>(ent.EntityID, ent.EntityGUID);
        }

        public bool EntityDelete(int id)
        {
            var canDelete = _eavContext.Entities.CanDeleteEntity(id);
            if (!canDelete.Item1)
                throw new Exception(canDelete.Item2);
            return _eavContext.Entities.DeleteEntity(id);
        }

        public bool EntityExists(Guid guid) 
            => _eavContext.Entities.EntityExists(guid);

        public int EntityGetOrResurect(Guid guid)
        {
            var existingEnt = _eavContext.Entities.GetEntitiesByGuid(guid).First();
            if (existingEnt.ChangeLogDeleted != null)
                existingEnt.ChangeLogDeleted = null;
            return existingEnt.EntityID;
        }

        #endregion

        public static int ZoneCreate(string name)
        {
            return EavDataController.Instance(null, null)
                .Zone.AddZone(name).Item1.ZoneID;
        }

        public List<LanguageInfo> ZoneLanguages()
        {
            var dims = _eavContext.Dimensions.GetLanguages();
            var mapped = dims.Select(d => new LanguageInfo
            {
                Active = d.Active,
                TennantKey = d.ExternalKey
            }).ToList();
            return mapped;
        }


    }
    public class LanguageInfo
    {
        public bool Active;
        public string TennantKey;
    }
}