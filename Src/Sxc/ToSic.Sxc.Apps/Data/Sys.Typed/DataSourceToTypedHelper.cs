using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToSic.Sxc.Data.Sys.Factory;

namespace ToSic.Sxc.Data.Sys.Typed;
internal class DataSourceToTypedHelper(ICodeDataFactory cdf, IDataSource source, ILog? parentLog) : HelperBase(parentLog, "Sxc.Ds2Typ")
{

}
