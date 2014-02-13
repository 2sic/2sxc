using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav;
using ToSic.SexyContent;
using ToSic.Eav.DataSources;

namespace ToSic.SexyContent.DataSources
{
    /// <summary>
    /// 2SexyContent List DataSource
    /// </summary>
    public class EntitiesFromList : EntityIdFilter
    {
        public override string Name { get { return "EntitiesFromList"; } }

        public EntitiesFromList()
        {
            BaseDataStream = Out["Default"];
            Out["Default"] = new DataStream(this, "Default", GetEntities);
        }

        private readonly IDataStream BaseDataStream;

        private IDictionary<int, IEntity> GetEntities()
        {
            if (!ListID.HasValue)
                return BaseDataStream.List;

            var sexy = new SexyContent();
            var entityIDs = (from c in sexy.TemplateContext.GetContentGroupItems(ListID.Value)
                                where c.Type == ContentGroupItemType.Content.ToString("F") && c.EntityID.HasValue
                                select c.EntityID.Value).ToArray();

            base.Configuration["EntityIds"] = String.Join(",",entityIDs);
            return BaseDataStream.List;
        }

        public int? ListID { get; set; }
    }

}
