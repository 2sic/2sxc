using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Context;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Blocks
{
    [PrivateApi("Hide implementation")]
    public class CmsView: ICmsView, IWrapper<IView>
    {
        public CmsView(/*IDynamicCode dynCode,*/ IBlock block)
        {
            //_dynCode = dynCode;
            UnwrappedContents = block?.View;
        }
        //private readonly IDynamicCode _dynCode;
        [PrivateApi]
        public IView UnwrappedContents { get; }

        /// <inheritdoc />
        public int Id => UnwrappedContents?.Id ?? 0;

        /// <inheritdoc />
        public string Name => UnwrappedContents?.Name ?? "";

        /// <inheritdoc />
        public string Identifier => UnwrappedContents?.Identifier ?? "";

        /// <inheritdoc />
        public string Edition => UnwrappedContents?.Edition;

        ///// <inheritdoc />
        //public dynamic Configuration => _configuration ?? (_configuration = MakeDynamic(_view.Entity));
        //private dynamic _configuration;

        ///// <inheritdoc />
        //public dynamic Resources => _resources ?? (_resources = MakeDynamic(_view?.Resources));
        //private dynamic _resources;

        ///// <inheritdoc />
        //public dynamic Settings => _settings ?? (_settings = MakeDynamic(_view?.Settings));
        //private dynamic _settings;

        #region Internal code to construct dynamic entities on the View

        //private dynamic MakeDynamic(IEntity entity) => _dynCode.AsDynamic(entity ?? FakeEntity);

        //private IDataBuilder Builder => _builder ?? (_builder = _dynCode.GetService<IDataBuilder>());
        //private IDataBuilder _builder;

        //private IEntity FakeEntity => _fakeEntity ?? (_fakeEntity = Builder.FakeEntity(_dynCode.App?.AppId ?? 0));
        //private IEntity _fakeEntity;
        
        #endregion

    }
}
