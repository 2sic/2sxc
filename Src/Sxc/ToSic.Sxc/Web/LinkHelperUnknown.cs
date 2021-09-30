using System;
using ToSic.Eav.Documentation;
using ToSic.Eav.Run;
using ToSic.Eav.Run.Unknown;

namespace ToSic.Sxc.Web
{
    [PrivateApi("for testing / un-implemented use")]
    public class LinkHelperUnknown: LinkHelper, IIsUnknown
    {
        public LinkHelperUnknown(ImgResizeLinker imgLinker, WarnUseOfUnknown<LinkHelperUnknown> warn) : base(imgLinker)
        {
        }

        protected override string ToImplementation(int? pageId = null, string parameters = null, string api = null)
        {
            // TODO: stv
            throw new NotImplementedException();
        }

        public override string GetDomainName()
        {
            // TODO: stv - use a pre-standardised dummy-domain like unknown.2sxc.org
            throw new NotImplementedException();
        }
    }
}
