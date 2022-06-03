using System.Collections.Generic;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.WebApi.Admin
{
    public class CodeControllerReal : HasLog<CodeControllerReal>
    {
        public const string LogSuffix = "Code";

        public CodeControllerReal() : base("Api.CodeRl")
        {

        }

        public class HelpItem
        {
            // the name of the class
            public string Term { get; set; }
            // message from the attribute
            public string[] Help { get; set; }
        }

        public IEnumerable<HelpItem> InlineHelp(string language)
        {
            return new[]
            {
                new HelpItem
                {
                    Term = "Language",
                    Help = new[]
                    {
                        "The language to use for the help. Currently only 'csharp' is supported.",
                        "csharp"
                    }
                },
                new HelpItem
                {
                    Term = "Code",
                    Help = new[]
                    {
                        "The code to generate help for.",
                        "var x = 0;"
                    }
                },
                new HelpItem{
                    Term = "line",
                    Help = new[]
                    {
                        "The line number to generate help for. If not specified, the whole code is returned.",
                        "1"
                    }
                },
                new HelpItem
                {
                    Term = "column",
                    Help = new[]
                    {
                        "The column number to generate help for. If not specified, the whole code is returned.",
                        "1"
                    }
                },
            };
        }
    }
}
