//using ToSic.Eav.WebApi.Sys.Admin;
//using ToSic.Eav.WebApi.Sys.Dto;
//using ToSic.Sxc.Dnn.WebApi.Sys;
//using RealController = ToSic.Eav.WebApi.Sys.Admin.AppInternalsControllerReal;

//namespace ToSic.Sxc.Dnn.Backend.Admin;

///// <summary>
///// Proxy Class to the AppInternalsController (Web API Controller)
///// </summary>
//[DnnLogExceptions]
//[ShowApiWhenReleased(ShowApiMode.Never)]
//public class AppInternalsController() : DnnSxcControllerBase(RealController.LogSuffix)
//{
//    private RealController Real => SysHlp.GetService<RealController>();

//    // Replaced by DataSource System.AppEnhancements through query System.SysData.
//    // Use app/auto/query/System.SysData/ with SysDataSource=System.AppEnhancements.
//    ///// <inheritdoc/>
//    //[HttpGet]
//    //[ValidateAntiForgeryToken]
//    //[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Edit)]
//    //public AppInternalsDto Get(int appId)
//    //    => Real.Get(appId);
//}