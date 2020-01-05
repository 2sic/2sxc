using ToSic.Eav.Documentation;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Conversion;

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

		private DataToDictionary _entityToDictionary;
		public DataToDictionary Serializer 
            => _entityToDictionary ?? (_entityToDictionary = new DataToDictionary(Cms?.UserMayEdit ?? false));
	}
}