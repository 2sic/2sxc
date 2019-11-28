using ToSic.Eav.Documentation;
using ToSic.SexyContent.Serializers;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Code
{
    [PrivateApi("this is an internal API, not sure if location is final.")]
	public class SxcHelper
	{
		public readonly ICmsBlock Cms;
		public SxcHelper(ICmsBlock cms)
		{
			Cms = cms;
		}

		private Serializer _serializer;
		public Serializer Serializer => _serializer ?? (_serializer = new Serializer(Cms));
	}
}