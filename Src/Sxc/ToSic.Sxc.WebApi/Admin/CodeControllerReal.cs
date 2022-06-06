using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ToSic.Eav.Logging;
using ToSic.Sxc.Code.Documentation;

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
            // TODO: stv# how to use languages?
            // - loging
            // - caching
            // - optimize for speed by limiting assemblies to ToSic only
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes().Where(t => t.IsDefined(typeof(DocsAttribute))))
                .Select(t => new HelpItem
                {
                    Term = t.Name,
                    Help = t.GetCustomAttribute<DocsAttribute>().GetMessages(t.FullName)
                });
        }
    }
}
