using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Compatibility.Sxc
{
    /// <summary>
    /// This is for compatibility - old code had a Sxc.Serializer.Prepare code which should still work
    /// </summary>
	public class SxcHelper
	{
		public readonly IBlockBuilder Cms;
		public SxcHelper(IBlockBuilder cms)
		{
			Cms = cms;
		}

		private OldDataToDictionaryWrapper _entityToDictionary;
		public OldDataToDictionaryWrapper Serializer 
            => _entityToDictionary ?? (_entityToDictionary = new OldDataToDictionaryWrapper(Cms?.UserMayEdit ?? false));
	}
}