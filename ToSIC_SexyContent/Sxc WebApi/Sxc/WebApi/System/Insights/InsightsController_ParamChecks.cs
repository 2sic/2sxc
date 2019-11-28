

namespace ToSic.Sxc.WebApi.System
{
    public partial class InsightsController 
    {
        private void ThrowIfNotSuperuser()
        {
            if (!PortalSettings.UserInfo.IsSuperUser)
                throw Http.PermissionDenied("requires Superuser permissions");
        }


        private bool UrlParamsIncomplete(int? appId, out string message)
        {
            ThrowIfNotSuperuser();
            return UrlParamIncomplete("appid", appId, out message);
        }



        private bool UrlParamsIncomplete(int? appId, string type, out string message)
            => UrlParamsIncomplete(appId, out message)
               || UrlParamIncomplete("type", type, out message);


        private bool UrlParamsIncomplete(int? appId, string type, string attribute, out string message)
            => UrlParamsIncomplete(appId, type, out message)
               || UrlParamIncomplete("attribute", attribute, out message);

        private bool UrlParamsIncomplete(int? appId, int? entity, out string message)
            => UrlParamsIncomplete(appId, out message)
               || UrlParamIncomplete("entity", entity, out message);

        /// <summary>
        /// verify hat a value is not null, otherwise give a reasonable message back
        /// </summary>
        /// <param name="name">parameter name for the message</param>
        /// <param name="value">value object which is expected to be not null</param>
        /// <param name="message">returned message</param>
        /// <returns>true if incomplete, false if ok</returns>
        private static bool UrlParamIncomplete(string name, object value, out string message)
        {
            message = null;
            if (value != null) return false;
            message = $"please add '{name}' to the url parameters";
            return true;
        }

    }
}