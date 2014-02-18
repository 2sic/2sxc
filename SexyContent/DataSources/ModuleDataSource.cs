using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ToSic.Eav;
using ToSic.Eav.DataSources;

namespace ToSic.SexyContent.DataSources
{
    public class ModuleDataSource : BaseDataSource
    {
        public override string Name { get { return "ModuleDataSource"; } }
        private SexyContent Sexy = new SexyContent(new int(), new int(), true);

        public ModuleDataSource()
        {
            Out.Add("Default", new DataStream(this, "Default", GetContent));
            Out.Add("Presentation", new DataStream(this, "Default", GetPresentation));
            Out.Add("ListContent", new DataStream(this, "Default", GetListContent));
            Out.Add("ListPresentation", new DataStream(this, "Default", GetListPresentation));

            // ToDo: Review with 2dm
            Configuration.Add("ListId", "[Module:ModuleID]");
            
        }

        private IDictionary<int, IEntity> GetContent()
        {
            if (!ListId.HasValue)
                return new Dictionary<int, IEntity>();

            var originals = In["Default"].List;
            var entityIds = Sexy.TemplateContext.GetContentGroupItems(ListId.Value, ContentGroupItemType.Content).Where(p => p.EntityID.HasValue);
            return entityIds.Distinct().ToDictionary(id => id.EntityID.Value, id => originals[id.EntityID.Value]);
        }
        private IDictionary<int, IEntity> GetPresentation()
        {
            return null;
        }
        private IDictionary<int, IEntity> GetListContent()
        {
            return null;
        }
        private IDictionary<int, IEntity> GetListPresentation()
        {
            return null;
        }

        private int? ListId
        {
            get
            {
                EnsureConfigurationIsLoaded();
                var listIdString = Configuration["ListId"];
                int listId;
                return int.TryParse(listIdString, out listId) ? listId : new int?();
            }
        }
    }
}