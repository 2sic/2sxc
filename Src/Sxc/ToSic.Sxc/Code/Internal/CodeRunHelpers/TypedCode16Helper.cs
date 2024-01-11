using System.Collections.Generic;
using ToSic.Lib.Helpers;
using ToSic.Sxc.Data;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Internal;

namespace ToSic.Sxc.Code.Internal.CodeRunHelpers;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class TypedCode16Helper: CodeHelperXxBase
{
    public bool DefaultStrict = true;

    //protected readonly IDynamicCodeRoot CodeRoot;
    private readonly IDictionary<string, object> _myModelData;
    //protected readonly bool IsRazor;
    //protected readonly string CodeFileName;
    internal ContextData Data { get; }
    public TypedCode16Helper(IDynamicCodeRoot codeRoot, IBlockInstance data, IDictionary<string, object> myModelData, bool isRazor, string codeFileName)
        : base(codeRoot, isRazor, codeFileName, SxcLogging.SxcLogName + ".TCd16H")
    {
        //CodeRoot = codeRoot;
        _myModelData = myModelData;
        //IsRazor = isRazor;
        //CodeFileName = codeFileName;
        Data = data as ContextData;
        //this.LinkLog(codeRoot.Log);
    }

    public ITypedItem MyItem => _myItem.Get(() => CodeRoot.Cdf.AsItem(Data.MyItem, propsRequired: DefaultStrict));
    private readonly GetOnce<ITypedItem> _myItem = new();

    public IEnumerable<ITypedItem> MyItems => _myItems.Get(() => CodeRoot.Cdf.AsItems(Data.MyItem, propsRequired: DefaultStrict));
    private readonly GetOnce<IEnumerable<ITypedItem>> _myItems = new();

    public ITypedItem MyHeader => _myHeader.Get(() => CodeRoot.Cdf.AsItem(Data.MyHeader, propsRequired: DefaultStrict));
    private readonly GetOnce<ITypedItem> _myHeader = new();

    public ITypedModel MyModel => _myModel.Get(() => new TypedModel(_myModelData, CodeRoot, IsRazor, CodeFileName));
    private readonly GetOnce<ITypedModel> _myModel = new();


    public ITypedStack AllResources => (CodeRoot as DynamicCodeRoot)?.AllResources;

    public ITypedStack AllSettings => (CodeRoot as DynamicCodeRoot)?.AllSettings;

    //public IDevTools DevTools => _devTools.Get(() => new DevTools(IsRazor, CodeFileName, Log));
    //private readonly GetOnce<IDevTools> _devTools = new GetOnce<IDevTools>();
}