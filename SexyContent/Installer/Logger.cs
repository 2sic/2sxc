using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web.Hosting;

namespace ToSic.SexyContent.Installer
{
    public class Logger
    {
        // private string _detailedLog;
        private readonly bool _saveUnimportantDetails;

        private StreamWriter _fileStreamWriter;
        private StreamWriter FileStreamWriter => _fileStreamWriter ?? OpenLogFiles();

        //private StreamWriter _detailedStreameWriter;
        //private StreamWriter DetailedStreamWriter => _detailedStreameWriter ?? OpenLogFiles(true);
        // private  FileStream _upgradeFileHandle;

        public Logger(bool saveDetails)
        {
            _saveUnimportantDetails = saveDetails;
        }

        //internal void Add(string newLine)
        //{
        //    _detailedLog += "\n" + newLine;
        //}


        internal void CloseLogFiles()
        {
            FileStreamWriter.BaseStream.Close();
        }

        //private string _lfn;
        //private string LogFileName => _lfn ?? (_lfn = GenerateNewLogFileName());

        private static string GenerateNewLogFileName()
        {
            var renamedLockFilePath =
                HostingEnvironment.MapPath(Settings.Installation.LogDirectory +
                                           DateTime.UtcNow.ToString(@"yyyy-MM-dd HH-mm-ss-fffffff") + ".log.resources");
            return renamedLockFilePath;
        }


        internal StreamWriter OpenLogFiles()//bool returnDetailed = false)
        {
            if (_fileStreamWriter == null)
            {
                var fileHandle = new FileStream(GenerateNewLogFileName(), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
                _fileStreamWriter = new StreamWriter(fileHandle);

                //fileHandle = new FileStream(GenerateNewLogFileName().Replace(".log", ".log.detailed"), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
                //_detailedStreameWriter = new StreamWriter(fileHandle);
            }
            return _fileStreamWriter;// returnDetailed ? _detailedStreameWriter : _fileStreamWriter;
        }



        internal string FormatLogMessage(string version, string message)
            => DateTime.UtcNow.ToString(@"yyyy-MM-ddTHH\:mm\:ss") + " " + version + " - " + message;
        

        internal void LogStep(string version, string message, bool isImportant = true)
        {
            var niceLine = FormatLogMessage(version, message);

            //DetailedStreamWriter.WriteLine(niceLine);
            //DetailedStreamWriter.Flush();
            //Add(niceLine);
            //DetailedLog += "\n" + niceLine;

            if (!isImportant && !_saveUnimportantDetails) return;

            // sometimes a detail would be logged, when the file isn't open yet; then don't save
            // if (!(FileStreamWriter?.BaseStream?.CanWrite ?? false)) return;

            FileStreamWriter.WriteLine(niceLine);
            FileStreamWriter.Flush();
        }


        //internal void SaveDetailedLog()
        //{
        //    EnsureLogDirectoryExists();

        //    var logFilePath = HostingEnvironment.MapPath(Settings.Installation.LogDirectory + DateTime.Now.ToString("yyyy-MM-dd hh-mm-ss") + " detailed.resources");
        //    // if (appendToFile || !File.Exists(logFilePath))
        //    File.AppendAllText(logFilePath, _detailedLog, Encoding.UTF8);

        //}

        internal void LogVersionCompletedToPreventRerunningTheUpgrade(string version, bool appendToFile = true)
        {
            EnsureLogDirectoryExists();

            var logFilePath = HostingEnvironment.MapPath(Settings.Installation.LogDirectory + version + ".resources");
            if (appendToFile || !File.Exists(logFilePath))
                File.AppendAllText(logFilePath, DateTime.UtcNow.ToString(@"yyyy-MM-ddTHH\:mm\:ss.fffffffzzz"), Encoding.UTF8);
        }

        private static void EnsureLogDirectoryExists()
        {
            // if (!Directory.Exists(HostingEnvironment.MapPath(Settings.Installation.LogDirectory)))
            Directory.CreateDirectory(HostingEnvironment.MapPath(Settings.Installation.LogDirectory));
        }

        internal void DeleteAllLogFiles()
        {
            if (Directory.Exists(HostingEnvironment.MapPath(Settings.Installation.LogDirectory)))
            {
                var files = new List<string>(Directory.GetFiles(HostingEnvironment.MapPath(Settings.Installation.LogDirectory)));
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