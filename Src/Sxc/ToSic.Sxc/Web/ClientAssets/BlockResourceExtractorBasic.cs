using System;
using ToSic.Sxc.Blocks.Output;

namespace ToSic.Sxc.Web.Basic
{
    // ReSharper disable once UnusedMember.Global
    public class BasicBlockResourceExtractor: BlockResourceExtractor
    {
        public override Tuple<string, bool> Process(string renderedTemplate) => new Tuple<string, bool>(renderedTemplate, false);
    }
}
