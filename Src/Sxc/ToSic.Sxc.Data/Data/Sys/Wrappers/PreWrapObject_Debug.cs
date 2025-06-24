using ToSic.Eav.Data.Sys;
using ToSic.Eav.Data.Sys.PropertyDump;

namespace ToSic.Sxc.Data.Sys.Wrappers;

partial class PreWrapObject: IPropertyDumpCustom
{
    [PrivateApi]
    public List<PropertyDumpItem> _DumpProperties(PropReqSpecs specs, string path, IPropertyDumpService dumpService)
        => _innerObject == null
            ? []
            : new PreWrapObjectDumpHelper()._Dump(this, PropDic, WrapperSvc, specs, path, dumpService);
}