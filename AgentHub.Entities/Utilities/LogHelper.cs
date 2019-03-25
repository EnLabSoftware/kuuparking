using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace AgentHub.Entities.Utilities
{
    public static class LogHelper
    {
        public static Logger Instance { get; private set; }
        static LogHelper()
        {
            try
            {
                var config = new LoggingConfiguration();

                var fileTarget = new FileTarget();
                config.AddTarget("file", fileTarget);

                var logFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
                Directory.CreateDirectory(logFolder);
                fileTarget.FileName = "${basedir}/Logs/LogFile.txt";
                fileTarget.Layout = "${message}";
                fileTarget.ArchiveFileName = "${basedir}/Logs/Archives/{#}.txt";

                fileTarget.ArchiveEvery = FileArchivePeriod.Day;
                fileTarget.ArchiveNumbering = ArchiveNumberingMode.Date;
                fileTarget.MaxArchiveFiles = AppSettings.MaxLogArchiveFilesInDays;
                fileTarget.ArchiveDateFormat = "yyyy-MM-dd";
                fileTarget.ConcurrentWrites = true;
                fileTarget.KeepFileOpen = false;

                var rule2 = new LoggingRule("*", LogLevel.Debug, fileTarget);
                config.LoggingRules.Add(rule2);

                LogManager.Configuration = config;
                Instance = LogManager.GetLogger("ELLN");
            }
            catch
            {
            }
        }

        public static void LogException(Exception exception)
        {
            var message = exception.Message;
            if (exception.InnerException != null)
                message = exception.InnerException.Message;

            Instance.Error(exception, Environment.NewLine + DateTime.Now + ": " + message + Environment.NewLine + exception.StackTrace);
        }

        public static string GetFullErrorMessage(Exception exception)
        {
            var innerException = exception.InnerException;
            var message = exception.Message;
            var indentMargin = "   ";
            while (innerException != null)
            {
                message = Environment.NewLine + indentMargin + innerException.Message;
                innerException = innerException.InnerException;
                indentMargin = indentMargin + "   ";
            }                

            return message;
        }
    }
}
