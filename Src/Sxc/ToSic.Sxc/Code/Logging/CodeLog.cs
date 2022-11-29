using System;
using System.Runtime.CompilerServices;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Code
{
    [PrivateApi]
    // inherit from wrapper class
    public class CodeLog : Wrapper<ILog>, ICodeLog
    {
        public CodeLog(ILog log) : base(log ?? new Log(LogConstants.NameUnknown))
        { }

        /// <inheritdoc />
        public string Add(string message, [CallerFilePath] string cPath = null, [CallerMemberName] string cName = null, [CallerLineNumber] int cLine = 0)
            => _contents.AddAndReuse(message, cPath, cName, cLine);

        /// <inheritdoc />
        public void Warn(string message) => _contents.W(message);

        public void Exception(Exception ex) => _contents.Ex(ex);


        /// <inheritdoc />
        public Action<string> Call(string parameters = null, string message = null, bool useTimer = false,
            [CallerFilePath] string cPath = null, [CallerMemberName] string cName = null, [CallerLineNumber] int cLine = 0)
        {
            // must call the opener first, then return the closing function
            var call = _contents.Fn(parameters, message, useTimer, cPath, cName, cLine);
            return finalMsg => call.Done(finalMsg);
        }

        /// <inheritdoc />
        public Func<T, string, T> Call<T>(string parameters = null, string message = null, bool useTimer = false,
            [CallerFilePath] string cPath = null, [CallerMemberName] string cName = null, [CallerLineNumber] int cLine = 0)
        {
            // must call the opener first, then return the closing function
            var call = _contents.Fn<T>(parameters, message, useTimer, null, cPath, cName, cLine);
            return (data, finalMsg) => call.Return(data, finalMsg);
        }
    }
}


