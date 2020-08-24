using System;

namespace ToSic.Sxc.Web.Basic
{
    // ReSharper disable once UnusedMember.Global
    public class BasicClientDependencyOptimizer: ClientDependencyOptimizer
    {
        public override Tuple<string, bool> Process(string renderedTemplate) => new Tuple<string, bool>(renderedTemplate, false);
    }
}
