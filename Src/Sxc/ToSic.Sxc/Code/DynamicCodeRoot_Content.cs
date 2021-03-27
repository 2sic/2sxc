using System.Linq;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Code
{
    public partial class DynamicCodeRoot
    {


        #region basic properties like Content, Presentation, ListContent, ListPresentation

        /// <inheritdoc />
        public dynamic Content
        {
            get
            {
                if (_content == null) TryToBuildContent();
                return _content;
            }
        }
        private dynamic _content;


        /// <inheritdoc />
		public dynamic Header
        {
            get
            {
                if (_header == null) TryToBuildHeaderObject();
                return _header;
            }
        }
        private dynamic _header;

        /// <remarks>
        /// This must be lazy-loaded, otherwise initializing the AppAndDataHelper will break when the Data-object fails 
        /// - this would break API even though the List etc. are never accessed
        /// </remarks>
        private void TryToBuildHeaderObject()
        {
            Log.Add("try to build ListContent (header) object");
            if (Data == null || Block.View == null) return;
            if (!Data.Out.ContainsKey(ViewParts.ListContent)) return;

            var listEntity = Data[ViewParts.ListContent].List.FirstOrDefault();
            _header = listEntity == null ? null : AsDynamic(listEntity);
        }

#pragma warning disable 618

        /// <remarks>
        /// This must be lazy-loaded, otherwise initializing the AppAndDataHelper will break when the Data-object fails 
        /// - this would break API even though the List etc. are never accessed
        /// </remarks>
        [PrivateApi]
        internal void TryToBuildContent()
        {
            Log.Add("try to build Content objects");

            if (Data == null || Block.View == null) return;
            if (!Data.Out.ContainsKey(Eav.Constants.DefaultStreamName)) return;

            var entities = Data.List; //.ToList();
            if (entities.Any()) _content = AsDynamic(entities.First());

        }
#pragma warning restore 618

        #endregion
    }
}
