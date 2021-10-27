using System.Collections.Generic;

namespace ToSic.Sxc.WebApi.InPage
{
    public class AjaxRenderDto
    {
        public string Html { get; set; }

        public IEnumerable<AjaxResourceDtoWIP> Resources { get; set; }
    }

    public class AjaxResourceDtoWIP
    {
        public string Id { get; set; }

        public string Url { get; set; }

        /// <summary>
        /// "js" or "css"
        /// </summary>
        public string Type { get; set; } = "js";

        public string Contents { get; set; }
    }
}
