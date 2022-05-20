using System;
using System.Linq;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Apps
{
    public static class CmsQueryExtensions
    {
        public static bool DeleteQueryIfNotUsedByView(this CmsManager cms, int id, ILog log)
        {
            var wrapLog = log.Fn<bool>($"delete pipe:{id} on app:{cms.AppId}");

            // Stop if views still use this Query
            var viewUsingQuery = cms.Read.Views.GetAll()
                .Where(t => t.Query?.Id == id)
                .Select(t => t.Id)
                .ToArray();
            if (viewUsingQuery.Any())
                throw new Exception(
                    $"Query is used by Views and cant be deleted. Query ID: {id}. TemplateIds: {string.Join(", ", viewUsingQuery)}");

            return wrapLog.Return(cms.Queries.Delete(id), "ok");
        }

    }
}
