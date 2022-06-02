using System;

namespace ToSic.Sxc.Code.Documentation
{
    public class DocsAttribute: Attribute
    {

        public string[] Messages { get; set; }

        public bool AutoLink = true;

        public bool AllProperties = true;

        public string HelpLink { get; set; }
    }
}
