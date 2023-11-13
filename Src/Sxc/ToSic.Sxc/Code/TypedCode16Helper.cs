using System.Collections.Generic;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Data;
using ToSic.Sxc.DataSources;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Code
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public class TypedCode16Helper: CodeHelperXxBase
    {
        public bool DefaultStrict = true;

        //protected readonly IDynamicCodeRoot CodeRoot;
        private readonly IDictionary<string, object> _myModelData;
        //protected readonly bool IsRazor;
        //protected readonly string CodeFileName;
        internal ContextData Data { get; }
        public TypedCode16Helper(IDynamicCodeRoot codeRoot, IContextData data, IDictionary<string, object> myModelData, bool isRazor, string codeFileName)
            : base(codeRoot, isRazor, codeFileName, Constants.SxcLogName + ".TCd16H")
        {
            //CodeRoot = codeRoot;
            _myModelData = myModelData;
            //IsRazor = isRazor;
            //CodeFileName = codeFileName;
            Data = data as ContextData;
            //this.LinkLog(codeRoot.Log);
        }

        public ITypedItem MyItem => _myItem.Get(() => CodeRoot.Cdf.AsItem(Data.MyItem, Protector, propsRequired: DefaultStrict));
        private readonly GetOnce<ITypedItem> _myItem = new GetOnce<ITypedItem>();

        public IEnumerable<ITypedItem> MyItems => _myItems.Get(() => CodeRoot.Cdf.AsItems(Data.MyItem, Protector, propsRequired: DefaultStrict));
        private readonly GetOnce<IEnumerable<ITypedItem>> _myItems = new GetOnce<IEnumerable<ITypedItem>>();

        public ITypedItem MyHeader => _myHeader.Get(() => CodeRoot.Cdf.AsItem(Data.MyHeader, Protector, propsRequired: DefaultStrict));
        private readonly GetOnce<ITypedItem> _myHeader = new GetOnce<ITypedItem>();

        public ITypedModel MyModel => _myModel.Get(() => new TypedModel(_myModelData, CodeRoot, IsRazor, CodeFileName));
        private readonly GetOnce<ITypedModel> _myModel = new GetOnce<ITypedModel>();


        public ITypedStack AllResources => (CodeRoot as DynamicCodeRoot)?.AllResources;

        public ITypedStack AllSettings => (CodeRoot as DynamicCodeRoot)?.AllSettings;

        //public IDevTools DevTools => _devTools.Get(() => new DevTools(IsRazor, CodeFileName, Log));
        //private readonly GetOnce<IDevTools> _devTools = new GetOnce<IDevTools>();
    }
}
