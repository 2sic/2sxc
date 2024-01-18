using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;
using DotNetNuke.Services.Tokens;
using DotNetNuke.Common.Utilities;

namespace ToSic.Sxc.Dnn.LookUp;

/// <summary>
/// This class is mainly here to deliver all standard DNN-token lists to 2sxc. 
/// So it mainly initializes the normal DNN Token provider and offers a property called Property-Access which then contains all value-resolvers
/// </summary>
[PrivateApi("not for public use, it's an internal class just needed to retrieve DNN stuff")]
internal sealed class DnnTokenReplace : TokenReplace
{
    /// <param name="instanceId"></param>
    /// <param name="ps"></param>
    /// <param name="userInfo"></param>
    public DnnTokenReplace(int instanceId, PortalSettings ps, UserInfo userInfo)
        : base(Scope.DefaultSettings, "", ps, userInfo, instanceId == 0 ? Null.NullInteger : instanceId)
    {
        ModuleId = instanceId;
        PortalSettings = ps;
        try
        {
            // The first replace also initializes it...
            // So this must be executed, otherwise the list doesn't get built
            // But we've seen cases where something fails in here, hence the try/catch
            ReplaceTokens("InitializePropertySources");
        }
        catch
        {
            // In DNN 9.11 we have some issues which seem to error in the Token Lookup
            // But the reason is not clear.
            // So ATM we just ignore.
            /* ignore */
            //throw;
        }

        //try
        //{
        //    var minfo = TestModuleInfo;
        //    TestReplaceTokens("test");
        //    ReplaceTokens("test");
        //}
        //catch
        //{
        //    throw;
        //}
    }

    /// <summary>
    /// Make the protected PropertySource available to outside...
    /// </summary>
    public Dictionary<string, IPropertyAccess> PropertySources => PropertySource;





    //public ModuleInfo TestModuleInfo
    //{
    //    get
    //    {
    //        if (ModuleId > int.MinValue && (_moduleInfo == null || _moduleInfo.ModuleID != ModuleId))
    //            _moduleInfo = PortalSettings == null || PortalSettings.ActiveTab == null ? ServiceLocator<IModuleController, ModuleController>.Instance.GetModule(ModuleId, Null.NullInteger, true) : ServiceLocator<IModuleController, ModuleController>.Instance.GetModule(ModuleId, PortalSettings.ActiveTab.TabID, false);
    //        return _moduleInfo;
    //    }
    //    set => _moduleInfo = value;
    //}

    //private ModuleInfo _moduleInfo;

    //private string TestReplaceTokens(string sourceText)
    //{
    //    if (sourceText == null)
    //        return string.Empty;
    //    StringBuilder stringBuilder = new StringBuilder();
    //    foreach (Match match in TokenizerRegex.Matches(sourceText))
    //    {
    //        string objectName = match.Result("${object}");
    //        if (!string.IsNullOrEmpty(objectName))
    //        {
    //            if (objectName == "[")
    //                objectName = "no_object";
    //            string propertyName = match.Result("${property}");
    //            string format = match.Result("${format}");
    //            string str1 = match.Result("${ifEmpty}");
    //            string str2 = replacedTokenValue(objectName, propertyName, format);
    //            if (!string.IsNullOrEmpty(str1) && string.IsNullOrEmpty(str2))
    //                str2 = str1;
    //            stringBuilder.Append(str2);
    //        }
    //        else
    //            stringBuilder.Append(match.Result("${text}"));
    //    }
    //    return stringBuilder.ToString();
    //}
}