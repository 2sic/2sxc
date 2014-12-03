using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav;
using ToSic.Eav.Import;
using ToSic.SexyContent.DataImportExport.Extensions;
using ImportEntity = ToSic.Eav.Import.Entity;

namespace ToSic.SexyContent.Data.Api01
{
    /// <summary>
    /// This is a simple controller with some Create, Update and Delete commands. 
    /// </summary>
    public class SimpleDataController
    {
        private EavContext _contentContext;

        private string _defaultLanguageCode;

        private int _zoneId;

        private int _appId;

        private string _userName;



        /// <summary>
        /// Create a simple data controller to create, update and delete entities.
        /// </summary>
        /// <param name="zoneId">Zone ID</param>
        /// <param name="appId">App ID</param>
        /// <param name="userName">Name of user loged in</param>
        /// <param name="defaultLanguageCode">Default language of system</param>
        public SimpleDataController(int zoneId, int appId, string userName, string defaultLanguageCode)
        {
            this._zoneId = zoneId;
            this._appId = appId;
            this._userName = userName;
            this._defaultLanguageCode = defaultLanguageCode;
            this._contentContext = EavContext.Instance(zoneId, appId);
        }



        /// <summary>
        /// Create a new entity of the content-type specified.
        /// </summary>
        /// <param name="contentTypeName">Content-type</param>
        /// <param name="values">
        ///     Values to be set collected in a dictionary. Each dictionary item is a pair of attribute 
        ///     name and value. To set references to other entities, set the attribute value to a list of 
        ///     entity ids. 
        /// </param>
        /// <exception cref="ArgumentException">Content-type does not exist, or an attribute in values</exception>
        public void Create(string contentTypeName, Dictionary<string, object> values)
        {
            var attributeSet = _contentContext.GetAllAttributeSets().FirstOrDefault(item => item.Name == contentTypeName);
            if (attributeSet == null)
            {
                throw new ArgumentException("Content type '" + contentTypeName + "' does not exist.");
            }
            
            var importEntity = CreateImportEntity(attributeSet.StaticName);
            importEntity.AppendAttributeValues(attributeSet, ConvertEntityRelations(values), _defaultLanguageCode, false, true);
            importEntity.Import(_zoneId, _appId, _userName);
        }



        /// <summary>
        /// Update an entity specified by ID.
        /// </summary>
        /// <param name="entityId">Entity ID</param>
        /// <param name="values">
        ///     Values to be set collected in a dictionary. Each dictionary item is a pair of attribute 
        ///     name and value. To set references to other entities, set the attribute value to a list of 
        ///     entity ids. 
        /// </param>
        /// <exception cref="ArgumentException">Attribute in values does not exit</exception>
        /// <exception cref="ArgumentNullException">Entity does not exist</exception>
        public void Update(int entityId, Dictionary<string, object> values)
        {
            var entity = _contentContext.GetEntity(entityId);
            Update(entity, values);
        }

        /// <summary>
        /// Update an entity specified by GUID.
        /// </summary>
        /// <param name="entityGuid">Entity GUID</param>param>
        /// <param name="values">
        ///     Values to be set collected in a dictionary. Each dictionary item is a pair of attribute 
        ///     name and value. To set references to other entities, set the attribute value to a list of 
        ///     entity ids. 
        /// </param>
        /// <exception cref="ArgumentException">Attribute in values does not exit</exception>
        /// <exception cref="ArgumentNullException">Entity does not exist</exception>
        public void Update(Guid entityGuid, Dictionary<string, object> values)
        {
            var entity = _contentContext.GetEntity(entityGuid);
            Update(entity, values);
        }

        private void Update(Eav.Entity entity, Dictionary<string, object> values)
        {
            var attributeSet = _contentContext.GetAttributeSet(entity.AttributeSetID);            
            var importEntity = CreateImportEntity(entity.EntityGUID, attributeSet.StaticName);
            importEntity.AppendAttributeValues(attributeSet, ConvertEntityRelations(values), _defaultLanguageCode, false, true);
            importEntity.Import(_zoneId, _appId, _userName);
        }


        /// <summary>
        /// Delete the entity specified by ID.
        /// </summary>
        /// <param name="entityId">Entity ID</param>
        /// <exception cref="InvalidOperationException">Entity cannot be deleted for example when it is referenced by another object</exception>
        public void Delete(int entityId)
        {
            if (!_contentContext.CanDeleteEntity(entityId).Item1)
            {
                throw new InvalidOperationException("The entity " + entityId  + " cannot be deleted because of it is referenced by another object.");
            }
            _contentContext.DeleteEntity(entityId);
        }


        /// <summary>
        /// Delete the entity specified by GUID.
        /// </summary>
        /// <param name="entityGuid">Entity GUID</param>
        /// <exception cref="ArgumentNullException">Entity does not exist</exception>
        /// <exception cref="InvalidOperationException">Entity cannot be deleted for example when it is referenced by another object</exception>
        public void Delete(Guid entityGuid)
        {
            var entity = _contentContext.GetEntity(entityGuid);
            Delete(entity.EntityID);
        }



        private static ImportEntity CreateImportEntity(string attributeSetStaticName)
        {
            return CreateImportEntity(Guid.NewGuid(), attributeSetStaticName);
        }

        private static ImportEntity CreateImportEntity(Guid entityGuid, string attributeSetStaticName)
        {
            return new ImportEntity()
            {
                EntityGuid = entityGuid,
                AttributeSetStaticName = attributeSetStaticName,
                AssignmentObjectTypeId = SexyContent.AssignmentObjectTypeIDDefault,
                KeyNumber = null,
                Values = new Dictionary<string, List<IValueImportModel>>()
            };
        }


        private Dictionary<string, object> ConvertEntityRelations(Dictionary<string, object> values)
        {
            var result = new Dictionary<string, object>();
            foreach (var value in values)
            {
                var ids = value.Value as IEnumerable<int>;
                if (ids != null)
                {   // The value has entity ids. For import, these must be converted to a string of guids.
                    var guids = new List<Guid>();
                    foreach (var id in ids)
                    {
                        var entity = _contentContext.GetEntity(id);
                        guids.Add(entity.EntityGUID);
                    }
                    result.Add(value.Key, string.Join(",", guids));
                }
                else
                {
                    result.Add(value.Key, value.Value);
                }
            }
            return result;
        }
    }
}