using System;

namespace ToSic.Sxc.Compatibility.Sxc
{
    /// <summary>
    /// This is for compatibility - old code had a Sxc.Serializer.Prepare code which should still work
    /// </summary>
    [Obsolete]
	public class SxcHelper
	{
        private readonly bool _editAllowed;
		public SxcHelper(bool editAllowed) => _editAllowed = editAllowed;

        private OldDataToDictionaryWrapper _entityToDictionary;
		public OldDataToDictionaryWrapper Serializer 
            => _entityToDictionary ?? (_entityToDictionary = new OldDataToDictionaryWrapper(_editAllowed));
	}
}