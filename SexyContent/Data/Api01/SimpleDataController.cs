using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.Pkcs;
using System.Web;

namespace ToSic.SexyContent.Data.Api01
{
    /// <summary>
    /// This is a simple controller with some Create/Update/Delete commands. 
    /// It needs to be initialized with AppId, DefaultLanguage and maybe ZoneId.
    /// </summary>
    public class SimpleDataController
    {
        public SimpleDataController(int appId, string defaultLanguageCode)
        {
            // todo:
        }

        // todo: develop methods "Create", "Update", "Delete"
        public bool Create(string contentTypeName, Dictionary<string, object> values)
        {
            throw new NotImplementedException();
        }

        public bool Update(int entityId, Dictionary<string, object> values)
        {
            throw new NotImplementedException();
        }

        public bool Update(Guid entityGuid, Dictionary<string, object> values)
        {
            throw new NotImplementedException();
        }

        public bool Delete(int entityId)
        {
            throw new NotImplementedException();
        }

        public bool Delete(Guid entityGuid)
        {
            throw new NotImplementedException();
        }
    }
}