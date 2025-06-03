using ToSic.Eav.Data.Debug;
using ToSic.Eav.Data.PropertyDump.Sys;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Sxc.Data.Wrappers.Internal;

namespace ToSic.Sxc.Data.Internal.Wrapper;

partial class PreWrapObject: IPropertyDumpCustom
{
    [PrivateApi]
    public List<PropertyDumpItem> _DumpProperties(PropReqSpecs specs, string path, IPropertyDumpService dumpService)
        => _innerObject == null
            ? []
            : new PreWrapObjectDumpHelper()._Dump(this, PropDic, WrapperSvc, specs, path, dumpService);
}