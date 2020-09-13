using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.ImportExport.Json;
using ToSic.Eav.Security.Permissions;
using ToSic.Eav.WebApi.Formats;
using ToSic.Sxc.WebApi.Save;

namespace ToSic.Sxc.WebApi.Cms
{
    internal class EditSaveBackend : WebApiBackendBase<EditSaveBackend>
    {
        public EditSaveBackend() : base("Cms.SaveBk")
        {
        }



        public Dictionary<Guid, int> DoSave(AppManager appMan, List<BundleWithHeader<IEntity>> items, bool forceSaveAsDraft)
        {
            // only save entities that are
            // a) not in a group
            // b) in a group where the slot isn't marked as empty
            var entitiesToSave = items
                .Where(e => e.Header.Group == null || !e.Header.Group.SlotIsEmpty)
                .ToList();

            var save = new Eav.WebApi.SaveHelpers.SaveEntities(Log);
            save.UpdateGuidAndPublishedAndSaveMany(appMan, entitiesToSave, forceSaveAsDraft);

            return save.GenerateIdList(appMan.Read.Entities, items);

        }
    }
}
