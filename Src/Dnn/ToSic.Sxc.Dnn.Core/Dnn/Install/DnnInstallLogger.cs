using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Hosting;
using static System.IO.Directory;

namespace ToSic.Sxc.Dnn.Install
{
    public class DnnInstallLogger
    {
        private readonly bool _saveUnimportantDetails;

        public DnnInstallLogger()
        {
            _saveUnimportantDetails = DnnEnvironmentInstaller.SaveUnimportantDetails;
        }

        internal void CloseLogFiles()
        {
            if (_fileStreamWriterCached == null) return;

            _fileStreamWriterCached.Close();
            _fileStreamWriterCached.Dispose();
            _fileStreamWriterCached = null;
        }

        private StreamWriter FileStreamWriter => _fileStreamWriterCached ?? (_fileStreamWriterCached = OpenLogFiles());
        private StreamWriter _fileStreamWriterCached;


        internal StreamWriter OpenLogFiles()
        {
            EnsureLogDirectoryExists();

            var logFileName = HostingEnvironment.MapPath(DnnConstants.LogDirectory +
                                       DateTime.UtcNow.ToString(@"yyyy-MM-dd HH-mm-ss-fffffff")
                                       + "-" + System.Diagnostics.Process.GetCurrentProcess().Id
                                       + "-" + AppDomain.CurrentDomain.Id + ".log.resources");

            StreamWriter streamWriter = null;
            try
            {
                var fileHandle = new FileStream(logFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite,
                    FileShare.Read);
                streamWriter = new StreamWriter(fileHandle);
            }
            catch (Exception)
            {
                if (streamWriter != null)
                {
                    streamWriter.Close();
                    streamWriter.Dispose();
                }
            }
            return streamWriter;
        }



        internal string FormatLogMessage(string version, string message)
            => DateTime.UtcNow.ToString(@"yyyy-MM-ddTHH\:mm\:ss") + " " + version + " - " + message;
        

        internal void LogStep(string version, string message, bool isImportant = true)
        {
            var niceLine = FormatLogMessage(version, message);

            if (!isImportant && !_saveUnimportantDetails) return;

            FileStreamWriter.WriteLine(niceLine);
            FileStreamWriter.Flush();
        }


        internal void LogVersionCompletedToPreventRerunningTheUpgrade(string version)
        {
            EnsureLogDirectoryExists();

            var logFilePath = HostingEnvironment.MapPath(DnnConstants.LogDirectory + version + ".resources");
            if (!File.Exists(logFilePath))
                File.AppendAllText(logFilePath, DateTime.UtcNow.ToString(@"yyyy-MM-ddTHH\:mm\:ss.fffffffzzz"), Encoding.UTF8);
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
}