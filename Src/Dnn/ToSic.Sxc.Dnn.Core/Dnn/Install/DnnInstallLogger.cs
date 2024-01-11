using System.IO;
using System.Text;
using System.Web.Hosting;
using ToSic.Lib.Services;
using static System.IO.Directory;

namespace ToSic.Sxc.Dnn.Install;

internal class DnnInstallLogger: ServiceBase
{
    private readonly bool _saveUnimportantDetails;

    public DnnInstallLogger(): base("Dnn.InstLg")
    {
        var l = Log.Fn();
        _saveUnimportantDetails = DnnEnvironmentInstaller.SaveUnimportantDetails;
        l.A($"{nameof(_saveUnimportantDetails)}: {_saveUnimportantDetails}");
        l.Done();
    }

    internal void CloseLogFiles()
    {
        var l = Log.Fn($"Closing: {DateTime.Now.Ticks}");
        if (_fileStreamWriterCached == null) return;

        _fileStreamWriterCached.Close();
        _fileStreamWriterCached.Dispose();
        _fileStreamWriterCached = null;
        l.Done();
    }

    private StreamWriter FileStreamWriter => _fileStreamWriterCached ??= OpenLogFiles(0);
    private StreamWriter _fileStreamWriterCached;


    internal StreamWriter OpenLogFiles(int attempts)
    {
        var l = Log.Fn<StreamWriter>($"Opening: {DateTime.Now.Ticks}; {nameof(attempts)}: {attempts}");
        EnsureLogDirectoryExists();

        var logFileNameBase = DnnConstants.LogDirectory +
                              DateTime.UtcNow.ToString(@"yyyy-MM-dd HH-mm-ss-fffffff")
                              + "-" + System.Diagnostics.Process.GetCurrentProcess().Id
                              + "-" + AppDomain.CurrentDomain.Id
                              + (attempts == 0 ? "" : $"-{attempts}");

        var logFileName = HostingEnvironment.MapPath($"{logFileNameBase}.log.resources");
        l.A($"{nameof(logFileName)}: {logFileName}");

        StreamWriter streamWriter = null;
        try
        {
            var fileHandle = new FileStream(logFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite,
                FileShare.Read);
            streamWriter = new(fileHandle);
        }
        catch (Exception ex)
        {
            l.Ex(ex);
            l.A($"{nameof(streamWriter)} is null : {streamWriter is null}");
            if (streamWriter != null)
            {
                streamWriter.Close();
                streamWriter.Dispose();
                streamWriter = null;
            }
            l.A($"Will try again, current attempt count is {attempts}");
            if (attempts < 3) return OpenLogFiles(++attempts);
        }

        return l.Return(streamWriter);
    }



    internal string FormatLogMessage(string version, string message)
        => DateTime.UtcNow.ToString(@"yyyy-MM-ddTHH\:mm\:ss") + " " + version + " - " + message;
        

    internal void LogStep(string version, string message, bool isImportant = true)
    {
        var l = Log.Fn($"{nameof(version)} '{version}': {message}");
        var niceLine = FormatLogMessage(version, message);

        if (!isImportant && !_saveUnimportantDetails) return;

        FileStreamWriter.WriteLine(niceLine);
        FileStreamWriter.Flush();
        l.Done();
    }


    internal void LogVersionCompletedToPreventRerunningTheUpgrade(string version)
    {
        var l = Log.Fn();
        EnsureLogDirectoryExists();

        var logFilePath = HostingEnvironment.MapPath(DnnConstants.LogDirectory + version + ".resources");
        if (!File.Exists(logFilePath))
            File.AppendAllText(logFilePath, DateTime.UtcNow.ToString(@"yyyy-MM-ddTHH\:mm\:ss.fffffffzzz"), Encoding.UTF8);
        l.Done();
    }

    private static void EnsureLogDirectoryExists() => CreateDirectory(HostingEnvironment.MapPath(DnnConstants.LogDirectory));

    internal void DeleteAllLogFiles()
    {
        if (Exists(HostingEnvironment.MapPath(DnnConstants.LogDirectory)))
        {
            var files = new List<string>(GetFiles(HostingEnvironment.MapPath(DnnConstants.LogDirectory)));
            files.ForEach(x =>
            {
                try
                {
                    File.Delete(x);
                }
                catch
                {
                }
            });
        }
    }

}