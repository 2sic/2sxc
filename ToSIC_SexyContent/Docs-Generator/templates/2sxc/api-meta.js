exports.test = {};
exports.getData = getData;

const prioWeb = { 
  priority: "web"
};

const prioInternal = {
  priority: "internal"
};

const prioData = {
  priority: "data"
};

const prioMeta = {
  priority: "metadata"
};
const prioAdam = {
  priority: "adam"
};

exports.data = {
  "ToSic.Eav": prioInternal,
  "ToSic.Eav.Apps": prioInternal,
  "ToSic.Eav.Apps.Assets": prioInternal,
  "ToSic.Eav.Caching": prioInternal,
  "ToSic.Eav.Configuration": prioInternal,
  "ToSic.Eav.Data": prioInternal,
  "ToSic.Eav.DataSources": prioData,
  "ToSic.Eav.DataSources.Caching": prioInternal,
  "ToSic.Eav.DataSources.Queries": prioData,
  "ToSic.Eav.DataSources.System": prioInternal,
  "ToSic.Eav.Environment": prioInternal,
  "ToSic.Eav.Logging": prioInternal,
  "ToSic.Eav.LookUp": prioInternal,
  "ToSic.Eav.Metadata": prioMeta,
  "ToSic.Eav.Run": prioInternal,
  "ToSic.Eav.Security": prioInternal,
  "ToSic.Sxc.Adam": prioAdam,
  "ToSic.Sxc.Apps": prioInternal,
  "ToSic.Sxc.Blocks": prioInternal,
  "ToSic.Sxc.Code": prioInternal,
  "ToSic.Sxc.Data": prioWeb,
  "ToSic.Sxc.DataSources": prioData,
  "ToSic.Sxc.Dnn": prioWeb,
  "ToSic.Sxc.Dnn.Code": prioInternal,
  "ToSic.Sxc.Dnn.DataSources": prioData,
  "ToSic.Sxc.Dnn.LookUp": prioInternal,
  "ToSic.Sxc.Dnn.Run": prioInternal,
  "ToSic.Sxc.Dnn.Web": prioInternal,
  "ToSic.Sxc.Engines": prioInternal,
  "ToSic.Sxc.LookUp": prioInternal,
  "ToSic.Sxc.Search": prioWeb,
  "ToSic.Sxc.Web": prioWeb,
}

exports.priorityNormal = 'normal';

function getData() {
    return "hello;"
}