namespace ToSic.Sxc.Dnn;

public class DnnConstants
{
    public const string LogName = "Dnn";

    public const string ModuleNameContent = "2Sexy Content";
    public const string ModuleNameApp = "2Sexy Content App";

    public const string SysFolderRootVirtual = "~/desktopmodules/tosic_sexycontent/";

    internal const string LogDirectory = SysFolderRootVirtual + "Upgrade/Log/";

    public const string DnnContextKey = "DnnContext";

    /// <summary>
    /// The ID in the current HTTP request for storing the EAV log object
    /// </summary>
    public const string EavLogKey = "EavLog";

    /// <summary>
    /// Application Setting key to enable extended logging
    /// </summary>
    public const string AdvancedLoggingEnabledKey = "2sxc-enable-extended-logging";
        
    /// <summary>
    /// Application Setting key to ensure extended logging will expire
    /// </summary>
    public const string AdvancedLoggingTillKey = "2sxc-extended-logging-expires";

    /// <summary>
    /// AntiForgery token header name
    /// </summary>
    public const string AntiForgeryTokenHeaderName = "RequestVerificationToken";

    /// <summary>
    /// Prefix for user identity token
    /// </summary>
    public const string UserTokenPrefix = "dnn:userid=";
}