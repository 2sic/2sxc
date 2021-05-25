using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.Sxc.Code;
using ToSic.Sxc.Context;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Blocks
{
    [PrivateApi("Hide implementation")]
    public class CmsView: ICmsView
    {
        private readonly IDynamicCode _dynCode;
        private readonly IView _view;
        public CmsView(IDynamicCode dynCode, IBlock block)
        {
            _dynCode = dynCode;
            _view = block?.View;
        }
        
        public int Id => _view?.Id ?? 0;

        public string Name => _view?.Name ?? "";

        public string Edition => _view?.Edition;

        public dynamic Configuration => _configuration ?? (_configuration = _dynCode.AsDynamic(_view.Entity ?? FakeEntity));
        private dynamic _configuration;

        private IDataBuilder Builder => _builder ?? (_builder = _dynCode.GetService<IDataBuilder>());
        private IDataBuilder _builder;

        private IEntity FakeEntity => _fakeEntity ?? (_fakeEntity = Builder.FakeEntity(_dynCode.App?.AppId ?? 0));
        private IEntity _fakeEntity;

        public dynamic Resources => _resources ?? (_resources = _dynCode.AsDynamic(_view?.Resources ?? FakeEntity));
        private dynamic _resources;

        public dynamic Settings => _settings ?? (_settings = _dynCode.AsDynamic(_view?.Settings ?? FakeEntity));
        private dynamic _settings;
    }
}
