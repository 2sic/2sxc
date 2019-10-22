using ToSic.SexyContent.Serializers;

namespace ToSic.SexyContent
{
	public class SxcHelper
	{
		public readonly SxcInstance SxcInstance;
		public SxcHelper(SxcInstance sxcInstance)
		{
			SxcInstance = sxcInstance;
		}

		private Serializer _serializer;
		public Serializer Serializer => _serializer ?? (_serializer = new Serializer(SxcInstance));
	}
}