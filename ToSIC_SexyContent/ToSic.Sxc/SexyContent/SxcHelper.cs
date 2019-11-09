using ToSic.SexyContent.Serializers;
using ToSic.Sxc.Blocks;

namespace ToSic.SexyContent
{
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