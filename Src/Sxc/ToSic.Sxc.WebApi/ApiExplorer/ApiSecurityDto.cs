// ReSharper disable InconsistentNaming

using System.Collections.Generic;

namespace ToSic.Sxc.WebApi.ApiExplorer
{
    public class ApiSecurityDto
    {
        public bool ignoreSecurity { get; set; }
        
        public bool allowAnonymous { get; set; }

        public bool requireVerificationToken { get; set; }
        public bool _validateAntiForgeryToken { get; set; }
        public bool _autoValidateAntiforgeryToken { get; set; }
        public bool _ignoreAntiforgeryToken { get; set; }

        public bool view { get; set; }
        public bool edit { get; set; }
        public bool admin { get; set; }
        public bool superUser { get; set; }


        public bool requireContext { get; set; }
    }
}
