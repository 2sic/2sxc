using System;
using System.Linq;

namespace ToSic.Sxc.Code.Documentation
{
    public class DocsAttribute: Attribute
    {
        public string[] Messages
        {
            set => _messages = value;
        }

        private string[] GetMessages(string fullName)
        {
            if (!AutoLink) return _messages;

            var newMessages = _messages.ToList();
            var helpLink = $"[documentation](https://docs.2sxc.org/api/dot-net/{fullName}.html";
            newMessages.Add(helpLink);
            return newMessages.ToArray();
        }


        public bool AutoLink = true;

        public bool AllProperties = true;
        private string[] _messages;

        public string HelpLink { get; set; }
    }
}
