using System;
using ToSic.Eav.Run.Unknown;
using ToSic.Sxc.Blocks.Output;

namespace ToSic.Sxc.Web
{
    // ReSharper disable once UnusedMember.Global
    public class BlockResourceExtractorUnknown: BlockResourceExtractor
    {
        public BlockResourceExtractorUnknown(WarnUseOfUnknown<BlockResourceExtractorUnknown> warn) { }

        public override (string Template, bool Include2sxcJs) Process(string renderedTemplate) => (renderedTemplate, false);
    }
}
