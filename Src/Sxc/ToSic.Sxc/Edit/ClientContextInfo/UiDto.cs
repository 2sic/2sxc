using Newtonsoft.Json;

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

            // for 2sxc 10
            Form = null; // Features.Enabled(FeatureIds.EditFormPreferAngularJs) ? null : "ng8";
        }
    }
}
