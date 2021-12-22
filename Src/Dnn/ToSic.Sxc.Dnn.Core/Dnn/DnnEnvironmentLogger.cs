using System;
using DotNetNuke.Services.Exceptions;
using ToSic.Eav.Apps.Environment;

namespace ToSic.Sxc.Dnn
{
    public class DnnEnvironmentLogger: IEnvironmentLogger
    {
        public void LogException(Exception ex) => Exceptions.LogException(ex);
    }
}
