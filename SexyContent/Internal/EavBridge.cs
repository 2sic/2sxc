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

        #region constructors
        public EavBridge(IApp app) : this(app.ZoneId, app.AppId)
        {
        }

        public EavBridge(int zoneId, int appId)
        {
            _eavContext = EavDataController.Instance(zoneId, appId);
        }
        #endregion

        public static bool EntityPublish(int zoneId, int appId, int repositoryId, bool state)
        {
            EavDataController.Instance(zoneId, appId).Publishing.PublishDraftInDbEntity(repositoryId, state);
            return state;
        }


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

        //#region Data-level entity actions

        //public static void EntityUpdate(int zoneId, int appId, int id, Dictionary<string, object> values)
        //    => EavDataController.Instance(zoneId, appId).Entities.UpdateEntity(id, values);

        //public static Tuple<int, Guid> EntityCreate(int zoneId, int appId, string typeName, Dictionary<string, object> values, Guid? entityGuid = null)
        //{
        //    var contentType = DataSource.GetCache(zoneId, appId).GetContentType(typeName);
        //    var ent = EavDataController.Instance(zoneId, appId).Entities.AddEntity(contentType.AttributeSetId, values, null, null, entityGuid: entityGuid);
        //    return new Tuple<int, Guid>(ent.EntityID, ent.EntityGUID);
        //}

        //public static bool EntityDelete(int zoneId, int appId, int id)
        //{
        //    var eavContext = EavDataController.Instance(zoneId, appId);
        //    var canDelete = eavContext.Entities.CanDeleteEntity(id);
        //    if (!canDelete.Item1)
        //        throw new Exception(canDelete.Item2);
        //    return eavContext.Entities.DeleteEntity(id);
        //}

        //public static int EntityGetOrCreate(int zoneId, int appId, Guid? newGuid, string contentTypeName, Dictionary<string, object> values)
        //{
        //    var ctl = EavDataController.Instance(zoneId, appId);
        //    if (newGuid.HasValue && ctl.Entities.EntityExists(newGuid.Value)) // eavDc.Entities.EntityExists(newGuid.Value))
        //    {
        //        // check if it's deleted - if yes, resurrect
        //        var existingEnt = ctl.Entities.GetEntitiesByGuid(newGuid.Value).First();
        //        if (existingEnt.ChangeLogDeleted != null)
        //            existingEnt.ChangeLogDeleted = null;

        //        return existingEnt.EntityID;
        //    }
            
        //    return EntityCreate(zoneId, appId, contentTypeName, values, entityGuid: newGuid).Item1;
        //}

        ////public static bool EntityExists(int zoneId, int appId, Guid guid) 
        ////    => EavDataController.Instance(zoneId, appId).Entities.EntityExists(guid);

        ////public static int EntityGetOrResurrect(int zoneId, int appId, Guid guid)
        ////{
        ////    var existingEnt = EavDataController.Instance(zoneId, appId).Entities.GetEntitiesByGuid(guid).First();
        ////    if (existingEnt.ChangeLogDeleted != null)
        ////        existingEnt.ChangeLogDeleted = null;
        ////    return existingEnt.EntityID;
        ////}

        //#endregion

        //public static int ZoneCreate(string name)
        //{
        //    return EavDataController.Instance(null, null)
        //        .Zone.AddZone(name).Item1.ZoneID;
        //}

        //public List<LanguageInfo> ZoneLanguages()
        //{
        //    var dims = _eavContext.Dimensions.GetLanguages();
        //    var mapped = dims.Select(d => new LanguageInfo
        //    {
        //        Active = d.Active,
        //        TennantKey = d.ExternalKey
        //    }).ToList();
        //    return mapped;
        //}
        //public List<Tuple<bool, string>> ZoneLanguages()
        //{
        //    return _eavContext.Dimensions.GetLanguages()
        //        .Select(d => new Tuple<bool, string>(d.Active, d.ExternalKey))
        //        .ToList();
        //}
        //public void ZoneAddOrUpdateLanguage(string cultureCode, string cultureText, bool active)
        //    => _eavContext.Dimensions.AddOrUpdateLanguage(cultureCode, cultureText, active);

    }
    //public class LanguageInfo
    //{
    //    public bool Active;
    //    public string TennantKey;
    //}
}