//using System;
//using Oqtane.Modules;
//using ToSic.Lib.Logging;
//using ToSic.Lib.Services;
//using ToSic.Sxc.Services;

//namespace ToSic.Sxc.Oqt.Client.Services
//{
//    public class FeaturesService: ServiceBase, IFeaturesService, ICanDebug, IService
//    {
//        public FeaturesService() : base($"Blazor.FeatSv")
//        { }


//        public bool IsEnabled(params string[] nameIds)
//        {
//            var result = true;
//            if (!Debug) return result;
//            var wrapLog = Log.Fn<bool>(string.Join(",", nameIds ?? Array.Empty<string>()));
//            return wrapLog.Return(result, $"{result}");
//        }

//        public bool Debug { get; set; }
//    }
//}
