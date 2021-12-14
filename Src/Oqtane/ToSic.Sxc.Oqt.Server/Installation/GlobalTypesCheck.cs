using System.IO;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Server.Installation
{
    // On first installation of 2sxc module in oqtane, SystemLoader can not load all 2sxc global types
    // because it has dependency on ToSic_Eav_* sql tables, before this tables are actually created by oqtane 2.3.x,
    // but after next restart of oqtane application all is ok, and all 2sxc global types are loaded as expected
    // this code will check if there is less than 50 global types and warn user to restart application to fix that
    public class GlobalTypesCheck
    {

        public GlobalTypesCheck(IAppStates appStates)
        {
            _appStates = appStates;
        }

        private readonly IAppStates _appStates;

        private static bool? _globalTypesCheckedAndError;

        internal static OqtViewResultsDto InstallationErrorResult;

        public bool WarnIfGlobalTypesAreNotLoaded(out OqtViewResultsDto oqtViewResultsDto)
        {
            // Don't repeat if already checked
            if(_globalTypesCheckedAndError.HasValue)
            {
                oqtViewResultsDto = InstallationErrorResult;
                return _globalTypesCheckedAndError.Value;
            }
            
            // First time, run all checks
            var errorMessage = string.Empty;

            // Check if there is less than 50 global types and warn user to restart application.
            _globalTypesCheckedAndError = _appStates.Get(Eav.Constants.PresetIdentity).ContentTypes.Count() < 50;
            if (_globalTypesCheckedAndError.Value)
            {
                errorMessage = "<strong>Warning:</strong> The \"global types\" are not loaded. Please <a href=\"/admin/system\">Restart Application</a>.";
                InstallationErrorResult = new OqtViewResultsDto
                {
                    ErrorMessage = errorMessage,
                };
            }

            oqtViewResultsDto = InstallationErrorResult;
            
            return !string.IsNullOrEmpty(errorMessage);
        }


    }
}
