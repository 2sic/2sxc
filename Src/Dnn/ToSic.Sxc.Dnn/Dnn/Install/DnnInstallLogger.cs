using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Hosting;

namespace ToSic.Sxc.Dnn.Install
{
    public class DnnInstallLogger
    {
        // private string _detailedLog;
        private readonly bool _saveUnimportantDetails;

        private StreamWriter _fileStreamWriter;
        private StreamWriter FileStreamWriter => _fileStreamWriter ?? OpenLogFiles();

        public DnnInstallLogger(bool saveDetails)
        {
            _saveUnimportantDetails = saveDetails;
        }

        internal void CloseLogFiles()
        {
            FileStreamWriter.BaseStream.Close();
        }

        private static string GenerateNewLogFileName()
        {
            var renamedLockFilePath =
                HostingEnvironment.MapPath(DnnConstants.LogDirectory +
                                           DateTime.UtcNow.ToString(@"yyyy-MM-dd HH-mm-ss-fffffff") + "-" + System.Diagnostics.Process.GetCurrentProcess().Id + "-" + AppDomain.CurrentDomain.Id + ".log.resources");
            return renamedLockFilePath;
        }


        internal StreamWriter OpenLogFiles()//bool returnDetailed = false)
        {
            EnsureLogDirectoryExists();

            if (_fileStreamWriter == null)
            {
                var fileHandle = new FileStream(GenerateNewLogFileName(), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
                _fileStreamWriter = new StreamWriter(fileHandle);
            }
            return _fileStreamWriter;
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


        internal void LogVersionCompletedToPreventRerunningTheUpgrade(string version, bool appendToFile = true)
        {
            EnsureLogDirectoryExists();

            var logFilePath = HostingEnvironment.MapPath(DnnConstants.LogDirectory + version + ".resources");
            if (appendToFile || !File.Exists(logFilePath))
                File.AppendAllText(logFilePath, DateTime.UtcNow.ToString(@"yyyy-MM-ddTHH\:mm\:ss.fffffffzzz"), Encoding.UTF8);
        }

        private static void EnsureLogDirectoryExists()
        {
            Directory.CreateDirectory(HostingEnvironment.MapPath(DnnConstants.LogDirectory));
        }

        internal void DeleteAllLogFiles()
        {
            if (Directory.Exists(HostingEnvironment.MapPath(DnnConstants.LogDirectory)))
            {
                var files = new List<string>(Directory.GetFiles(HostingEnvironment.MapPath(DnnConstants.LogDirectory)));
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