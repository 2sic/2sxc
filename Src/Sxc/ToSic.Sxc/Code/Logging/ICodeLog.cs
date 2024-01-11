using System.Runtime.CompilerServices;
using ToSic.Lib.Data;

namespace ToSic.Sxc.Code;

/// <summary>
/// A special logger for dynamic code (Razor, WebApi).
/// It is always available to add messages to insights. 
/// </summary>
/// <remarks>
/// Added in v15, replaces the then removed `ILog` interface.
/// </remarks>
[PublicApi]
public interface ICodeLog: IWrapper<ILog>
{

    /// <summary>
    /// Add a message log entry
    /// </summary>
    /// <param name="message">Message to log</param>
    /// <param name="cPath">auto pre filled by the compiler - the path to the code file</param>
    /// <param name="cName">auto pre filled by the compiler - the method name</param>
    /// <param name="cLine">auto pre filled by the compiler - the code line</param>
    string Add(string message,
        [CallerFilePath] string cPath = null,
        [CallerMemberName] string cName = null,
        [CallerLineNumber] int cLine = 0
    );

    /// <summary>
    /// Add a warning log entry
    /// </summary>
    /// <param name="message"></param>
    /// <param name="cPath">auto pre filled by the compiler - the path to the code file</param>
    /// <param name="cName">auto pre filled by the compiler - the method name</param>
    /// <param name="cLine">auto pre filled by the compiler - the code line</param>
    void Warn(string message,
        [CallerFilePath] string cPath = null,
        [CallerMemberName] string cName = null,
        [CallerLineNumber] int cLine = 0
    );


    /// <summary>
    /// Add an exception as special log entry
    /// </summary>
    /// <param name="ex">The Exception object</param>
    /// <param name="cPath">auto pre filled by the compiler - the path to the code file</param>
    /// <param name="cName">auto pre filled by the compiler - the method name</param>
    /// <param name="cLine">auto pre filled by the compiler - the code line</param>
    void Exception(Exception ex,
        [CallerFilePath] string cPath = null,
        [CallerMemberName] string cName = null,
        [CallerLineNumber] int cLine = 0
    );


    /// <summary>
    /// Add a log entry for method call, returning a method to call when done
    /// </summary>
    /// <param name="parameters">what was passed to the call in the brackets</param>
    /// <param name="message">the message to log</param>
    /// <param name="useTimer">enable a timer from call/close</param>
    /// <param name="cPath">auto pre filled by the compiler - the path to the code file</param>
    /// <param name="cName">auto pre filled by the compiler - the method name</param>
    /// <param name="cLine">auto pre filled by the compiler - the code line</param>
    Action<string> Call(
        string parameters = null,
        string message = null,
        bool useTimer = false,
        [CallerFilePath] string cPath = null,
        [CallerMemberName] string cName = null,
        [CallerLineNumber] int cLine = 0
    );

    /// <summary>
    /// Add a log entry for method call, returning a method to call when done
    /// </summary>
    /// <param name="parameters">what was passed to the call in the brackets</param>
    /// <param name="message">the message to log</param>
    /// <param name="useTimer">enable a timer from call/close</param>
    /// <param name="cPath">auto pre filled by the compiler - the path to the code file</param>
    /// <param name="cName">auto pre filled by the compiler - the method name</param>
    /// <param name="cLine">auto pre filled by the compiler - the code line</param>
    /// <remarks>
    /// Not used much, but major change in V15 - the first value in the result is the data, the second is the string to log.
    /// Before in the `ILog` it was (message, data), new is (data, message)
    /// </remarks>
    Func<T, string, T> Call<T>(
        string parameters = null,
        string message = null,
        bool useTimer = false,
        [CallerFilePath] string cPath = null,
        [CallerMemberName] string cName = null,
        [CallerLineNumber] int cLine = 0
    );

    /// <summary>
    /// Determines if this log should be preserved in the short term.
    /// Like for live-analytics / live-insights.
    /// Default is true.
    ///
    /// In scenarios like search-indexing it will default to false.
    /// You can then do `Log.Preserve = true;` to temporarily activate it while debugging.
    /// </summary>
    bool Preserve { get; set; }

}