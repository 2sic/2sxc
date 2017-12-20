using ToSic.SexyContent.Serializers;

namespace ToSic.SexyContent
{
	public class SxcHelper
	{
		private readonly SxcInstance _sexy;
		public SxcHelper(SxcInstance sexy)
		{
			_sexy = sexy;
		}

		private Serializer _serializer;
		public Serializer Serializer => _serializer ?? (_serializer = new Serializer(_sexy));
	}
}