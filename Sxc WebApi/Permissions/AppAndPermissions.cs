using System;
using System.Collections.Generic;
using System.Web.Http;
using ToSic.Eav.Apps;
using ToSic.Eav.Logging.Simple;
using ToSic.Eav.Security.Permissions;
using ToSic.SexyContent.Environment.Dnn7;
using ToSic.SexyContent.WebApi.Errors;

namespace ToSic.SexyContent.WebApi.Permissions
{
    internal class AppAndPermissions: PermissionsForApp
    {
        //public IPermissionCheck Permissions => _lastChecker;
        //private IPermissionCheck _lastChecker;

        public AppAndPermissions(SxcInstance sxcInstance, int appId, Log parentLog) :
            base(sxcInstance, SystemManager.ZoneIdOfApp(appId), appId, parentLog) { }



        //internal bool EnsureAll(List<Grants> grants, List<ItemIdentifier> items, out HttpResponseException preparedException)
        //{
        //    Log.Add(() => $"EnsureAll([{string.Join(",", grants)}], ...)");
        //    var typeNames = ExtractTypeNamesFromItems(items);

        //    // go through all the groups, assign relevant info so that we can then do get-many
        //    // this will run at least once with null, and the last one will be returned in the set
        //    foreach (var tn in typeNames)
        //        if (!Ensure(grants, tn, out preparedException))
        //            return false;

        //    preparedException = null;
        //    Log.Add("EnsureAll(): ok");
        //    return true;
        //}



        //[Obsolete("get out of this... WIP")]
        //internal bool Ensure(List<Grants> grants, string typeName, out HttpResponseException preparedException)
        //{
        //    return Ensure(grants, TypePermissionChecker(typeName), typeName, out preparedException);
        //    //Log.Add(() => $"Ensure([{string.Join(",", grants)}], {typeName}) or throw");
        //    //var permChecker = _lastChecker = TypePermissionChecker(typeName);

        //    //if (!permChecker.UserMay(grants))
        //    //{
        //    //    Log.Add("permissions not ok");
        //    //    preparedException = Http.PermissionDenied("required permissions for this type are not given");
        //    //    throw preparedException;
        //    //}
        //    //Log.Add("Ensure(...): ok");
        //    //preparedException = null;
        //    //return true;
        //}

        //protected bool Ensure(List<Grants> grants, IPermissionCheck permChecker, string typeName, out HttpResponseException preparedException)
        //{
        //    var wrapLog = Log.Call("Ensure", () => $"[{string.Join(",", grants)}], {typeName}", () => "or throw");
        //    // temp!!!
        //    //_lastChecker = permChecker;
        //    //var permChecker = _lastChecker = TypePermissionChecker(typeName);
            
        //    if (!permChecker.UserMay(grants))
        //    {
        //        Log.Add("permissions not ok");
        //        preparedException = Http.PermissionDenied("required permissions for this type are not given");
        //        throw preparedException;
        //    }
        //    wrapLog("ok");
        //    preparedException = null;
        //    return true;
        //}

        ///// <summary>
        ///// Creates a permission checker for an app
        ///// Optionally you can provide a type-name, which will be 
        ///// included in the permission check
        ///// </summary>
        ///// <param name="typeName"></param>
        ///// <returns></returns>
        //internal IPermissionCheck TypePermissionChecker(string typeName)
        //{
        //    Log.Call("TypePermissionChecker", $"{typeName}");
        //    // now do relevant security checks
        //    var type = typeName == null
        //        ? null
        //        : new AppRuntime(App, Log).ContentTypes.Get(typeName);

        //    // user has edit permissions on this app, and it's the same app as the user is coming from
        //    return new DnnPermissionCheck(Log,
        //        instance: SxcInstance.EnvInstance,
        //        app: App,
        //        portal: PortalForSecurityCheck,
        //        targetType: type);
        //}


    }
}
