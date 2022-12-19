using System;
using System.Runtime.CompilerServices;
using ToSic.Eav.Data;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;

namespace ToSic.Sxc.Code
{
    [PrivateApi("Hide implementation")]
    public class CodeLog : Wrapper<ILog>, ICodeLog
    {
        public CodeLog(ILog log) : base(log ?? new Log(LogConstants.NameUnknown))
        { }

        /// <inheritdoc />
        public string Add(string message, [CallerFilePath] string cPath = null, [CallerMemberName] string cName = null, [CallerLineNumber] int cLine = 0)
            => _contents.AddAndReuse(message, cPath, cName, cLine);

        /// <inheritdoc />
        public void Warn(string message, [CallerFilePath] string cPath = null, [CallerMemberName] string cName = null, [CallerLineNumber] int cLine = 0) 
            => _contents.W(message, cPath, cName, cLine);

        public void Exception(Exception ex, [CallerFilePath] string cPath = null, [CallerMemberName] string cName = null, [CallerLineNumber] int cLine = 0)
            => _contents.Ex(ex, cPath, cName, cLine);


        /// <inheritdoc />
        public Action<string> Call(string parameters = null, string message = null, bool useTimer = false,
            [CallerFilePath] string cPath = null, [CallerMemberName] string cName = null, [CallerLineNumber] int cLine = 0)
        {
            // must call the opener first, then return the closing function
            var call = _contents.Fn(parameters, message, useTimer, new CodeRef(cPath, cName, cLine));
            return finalMsg => call.Done(finalMsg);
        }

        /// <inheritdoc />
        public Func<T, string, T> Call<T>(string parameters = null, string message = null, bool useTimer = false,
            [CallerFilePath] string cPath = null, [CallerMemberName] string cName = null, [CallerLineNumber] int cLine = 0)
        {
            // must call the opener first, then return the closing function
            var call = _contents.Fn<T>(parameters, message, useTimer, new CodeRef( cPath, cName, cLine));
            return (data, finalMsg) => call.Return(data, finalMsg);
        }
    }
}


