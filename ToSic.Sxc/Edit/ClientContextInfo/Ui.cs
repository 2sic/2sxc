using Newtonsoft.Json;
using ToSic.Eav.Configuration;

namespace ToSic.Sxc.Edit.ClientContextInfo
{
    public class Ui
    {
        public bool AutoToolbar { get; }

        [JsonProperty("Form", NullValueHandling = NullValueHandling.Ignore)]
        public string Form { get; }

        public Ui(bool autoToolbar)
        {
            AutoToolbar = autoToolbar;

            Form = Features.Enabled(FeatureIds.PreferOldFormsToNewAngularUi) ? null : "ng8";
        }
    }
}
