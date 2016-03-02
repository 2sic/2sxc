using ToSic.SexyContent.Serializers;

namespace ToSic.SexyContent
{
	public class SxcHelper
	{
		private readonly SxcInstance Sexy;
		public SxcHelper(SxcInstance sexy)
		{
			Sexy = sexy;
		}

		private Serializer _serializer;
		public Serializer Serializer => _serializer ?? (_serializer = new Serializer(Sexy));
	}
}