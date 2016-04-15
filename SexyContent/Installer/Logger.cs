using System;
using System.IO;
using System.Text;
using System.Web.Hosting;

namespace ToSic.SexyContent.Installer
{
    public class Logger
    {
        private string _detailedLog;
        private readonly bool _saveUnimportantDetails;

        private StreamWriter _fileStreamWriter;
        private StreamWriter FileStreamWriter => _fileStreamWriter ?? OpenLogFiles();

        private  FileStream upgradeFileHandle = null;

        public Logger(bool saveDetails)
        {
            _saveUnimportantDetails = saveDetails;
        }

        internal void Add(string newLine)
        {
            _detailedLog += "\n" + newLine;
        }


        internal void CloseLogFiles()
        {
            FileStreamWriter.BaseStream.Close();
            // upgradeFileHandle.Close();
            //var renamedLockFilePath = NewLogFileName();
            //File.Move(LockFileName, renamedLockFilePath);
        }

        private string _lfn;
        private string LogFileName => _lfn ?? (_lfn = GenerateNewLogFileName());

        private static string GenerateNewLogFileName()
        {
            var renamedLockFilePath =
                HostingEnvironment.MapPath(Settings.Installation.LogDirectory +
                                           DateTime.UtcNow.ToString(@"yyyy-MM-dd HH-mm-ss-fffffff") + ".log.resources");
            return renamedLockFilePath;
        }


        internal StreamWriter OpenLogFiles()
        {
            if (_fileStreamWriter == null)
            {
                upgradeFileHandle = new FileStream(LogFileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
                _fileStreamWriter = new StreamWriter(upgradeFileHandle);
            }
            return FileStreamWriter;
        }



        internal string FormatLogMessage(string version, string message)
            => DateTime.UtcNow.ToString(@"yyyy-MM-ddTHH\:mm\:ss") + " " + version + " - " + message;
        

        internal void LogStep(string version, string message, bool isImportant = true)
        {
            var niceLine = FormatLogMessage(version, message);
            Add(niceLine);
            //DetailedLog += "\n" + niceLine;

            if (!isImportant && (isImportant || !_saveUnimportantDetails)) return;

            // sometimes a detail would be logged, when the file isn't open yet; then don't save
            if (!(FileStreamWriter?.BaseStream?.CanWrite ?? false)) return;

            FileStreamWriter.WriteLine(niceLine);
            FileStreamWriter.Flush();
        }


        internal void SaveDetailedLog()
        {
            if (!Directory.Exists(HostingEnvironment.MapPath(Settings.Installation.LogDirectory)))
                Directory.CreateDirectory(HostingEnvironment.MapPath(Settings.Installation.LogDirectory));

            var logFilePath = HostingEnvironment.MapPath(Settings.Installation.LogDirectory + DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss") + " detailed.resources");
            // if (appendToFile || !File.Exists(logFilePath))
            File.AppendAllText(logFilePath, _detailedLog, Encoding.UTF8);

        }

        internal void LogSuccessfulUpgrade(string version, bool appendToFile = true)
        {
            if (!Directory.Exists(HostingEnvironment.MapPath(Settings.Installation.LogDirectory)))
                Directory.CreateDirectory(HostingEnvironment.MapPath(Settings.Installation.LogDirectory));

            var logFilePath = HostingEnvironment.MapPath(Settings.Installation.LogDirectory + version + ".resources");
            if (appendToFile || !File.Exists(logFilePath))
                File.AppendAllText(logFilePath, DateTime.UtcNow.ToString(@"yyyy-MM-ddTHH\:mm\:ss.fffffffzzz"), Encoding.UTF8);
        }


    }
}