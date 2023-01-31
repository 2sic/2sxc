//using System.Collections.Generic;
//using System.Linq;
//using ToSic.Eav.Apps;
//using ToSic.Eav.ImportExport.Json.V1;
//using ToSic.Eav.ImportExport.Serialization;
//using ToSic.Eav.Plumbing;
//using ToSic.Eav.WebApi.Dto;
//using ToSic.Lib.Logging;

//namespace ToSic.Sxc.WebApi.Cms
//{
//    public partial class EditLoadPrefetchHelper
//    {
//        public List<JsonEntity> PrefetchSettingsEntities(int appId, EditDto editData, AppRuntime appRuntime) => Log.Func(l =>
//        {
//            var hasWysiwyg = editData.ContentTypes.SelectMany(
//                ct => ct.Attributes.Where(at => at.InputType.ContainsInsensitive("wysiwyg"))
//            ).ToList();

//            if (!hasWysiwyg.Any()) return (new List<JsonEntity>(), "no wysiwyg");

//            var entities = appRuntime.Entities
//                .GetWithParentAppsExperimental("StringWysiwygConfiguration")
//                .ToList();

//            var jsonSerializer = _jsonSerializerGenerator.Value.SetApp(appRuntime.AppState);
//            var result = entities.Select(e => jsonSerializer.ToJson(e)).ToList();

//            return (result, $"{result.Count}");

//        });

//    }
//}
