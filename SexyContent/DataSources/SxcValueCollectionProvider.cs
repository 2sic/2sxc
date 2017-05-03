using ToSic.Eav.ValueProvider;

namespace ToSic.SexyContent.DataSources
{
    public class SxcValueCollectionProvider: ValueCollectionProvider
    {
        public SxcInstance SxcInstance;

        public SxcValueCollectionProvider(SxcInstance sxc)
        {
            SxcInstance = sxc;
        }
    }
}