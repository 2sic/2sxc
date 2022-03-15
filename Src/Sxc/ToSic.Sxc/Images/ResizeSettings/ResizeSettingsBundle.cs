using System;
using Newtonsoft.Json;

namespace ToSic.Sxc.Images
{
    public class ResizeSettingsBundle: ResizeSettingsSrcSet
    {
        [JsonProperty("img")]
        public ResizeSettingsSrcSet Img { get; set; }

        [JsonProperty("sources")]
        public ResizeSettingsSrcSet[] Sources { get; set; }

        internal new ResizeSettingsBundle InitAfterLoad(double factor, int widthIfEmpty, ResizeSettingsSrcSet defaultsIfEmpty)
        {
            base.InitAfterLoad(factor, widthIfEmpty, defaultsIfEmpty);
            Img?.InitAfterLoad(factor, widthIfEmpty, defaultsIfEmpty);
            Sources = Sources ?? Array.Empty<ResizeSettingsSrcSet>();
            foreach (var s in Sources) s.InitAfterLoad(factor, widthIfEmpty, defaultsIfEmpty);
            return this;
        }

    }
}