using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Import;
using ToSic.SexyContent.DataImportExport.Extensions;

namespace ToSic.SexyContent.Data.Api01
{
    /// <summary>
    /// This is a simple controller with some Create/Update/Delete commands. 
    /// It needs to be initialized with AppId, DefaultLanguage and maybe ZoneId.
    /// </summary>
    public class SimpleDataController
    {
        private SexyContent _contentManager;

        private string _defaultLanguageCode;

        private int _zoneId;

        private int _appId;

        private string _userName;



        public SimpleDataController(int zoneId, int appId, string userName, string defaultLanguageCode)
        {
            this._zoneId = zoneId;
            this._appId = appId;
            this._userName = userName;
            this._defaultLanguageCode = defaultLanguageCode;
            this._contentManager = new SexyContent(zoneId, appId);
        }



        public bool Create(string contentTypeName, Dictionary<string, object> values)
        {
            try
            {
                var attributeSet = _contentManager.GetAvailableAttributeSets().FirstOrDefault(item => item.Name == contentTypeName);
                var importEntity = CreateImportEntity(attributeSet.StaticName);
                importEntity.AppendAttributeValues(attributeSet, values, _defaultLanguageCode, false, true);
                importEntity.Import(_zoneId, _appId, _userName);
                return true;
            }
            catch
            {   // The content-type does not exist, or an attribute in the content-type
                return false;
            }
        }



        public bool Update(int entityId, Dictionary<string, object> values)
        {
            try
            {
                var entity = _contentManager.ContentContext.GetEntity(entityId);
                return Update(entity, values);
            }
            catch
            {   // Entity does not exists
                return false;
            }
        }

        public bool Update(Guid entityGuid, Dictionary<string, object> values)
        {
            try
            {
                var entity = _contentManager.ContentContext.GetEntity(entityGuid);
                return Update(entity, values);
            }
            catch
            {   // Entity does not exists
                return false;
            }
        }

        private bool Update(Eav.Entity entity, Dictionary<string, object> values)
        {
            try
            {
                var attributeSet = _contentManager.ContentContext.GetAttributeSet(entity.AttributeSetID);
                var importEntity = CreateImportEntity(entity.EntityGUID, attributeSet.StaticName);
                importEntity.AppendAttributeValues(attributeSet, values, _defaultLanguageCode, false, true);
                importEntity.Import(_zoneId, _appId, _userName);
                return true;
            }
            catch
            {
                return false;  
            }
        }



        public bool Delete(int entityId)
        {
            var deleted = false;            
            try
            {
                if (_contentManager.ContentContext.CanDeleteEntity(entityId).Item1)
                {
                    deleted = _contentManager.ContentContext.DeleteEntity(entityId);
                }
            }
            catch
            {
                deleted = false;
            }
            return deleted;
        }

        public bool Delete(Guid entityGuid)
        {
            try
            {
                var entity = _contentManager.ContentContext.GetEntity(entityGuid);
                return Delete(entity.EntityID);
            }
            catch
            {   // Entity specified does not exist!
                return false;
            }
        }



        private static Entity CreateImportEntity(string attributeSetStaticName)
        {
            return CreateImportEntity(Guid.NewGuid(), attributeSetStaticName);
        }

        private static Entity CreateImportEntity(Guid entityGuid, string attributeSetStaticName)
        {
            return new Entity()
            {
                EntityGuid = entityGuid,
                AttributeSetStaticName = attributeSetStaticName,
                AssignmentObjectTypeId = SexyContent.AssignmentObjectTypeIDDefault,
                KeyNumber = null,
                Values = new Dictionary<string, List<IValueImportModel>>()
            };
        }
    }
}