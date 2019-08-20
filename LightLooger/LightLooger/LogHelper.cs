using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LightLog
{
    public class LogHelper
    {
        
        private static string _infoDirPath;
        private static string _debugDirPath;
        private static string _errorDirPath;
        private static string _baseDirPath;

        private static LogHelper Instance { set; get; }
        private LogLevel LogLv { get; set; } = LogLevel.Info;
        static LogHelper()
        { }
        public LogHelper()
        { }

        //暂时不启用，先采用简易模式
        //private ConcurrentQueue<LogTask> _logQueue = new ConcurrentQueue<LogTask>();
        
        public static LogHelper GetInstance()
        {
            if (Instance == null)
            {
                Instance = new LogHelper();
            }
            return Instance;
        }

        public LogHelper BuildLogLevel(LogLevel lv = LogLevel.Info)
        {
            GetInstance().LogLv = lv;
            return GetInstance();
        }

        public LogHelper BuildLogLevel()
        {
            string lvString = AppConfigHelper.Instance.GetSettingsKeyValue("LogLevel");
            switch (lvString)
            {
                case "Info":
                    BuildLogLevel(LogLevel.Info);
                    break;
                case "Debug":
                    BuildLogLevel(LogLevel.Debug);
                    break;
                case "Error":
                    BuildLogLevel(LogLevel.Error);
                    break;
                default:
                    BuildLogLevel(LogLevel.Info);
                    break;
            }
            return GetInstance();
        }

        public LogHelper BuildLogDirectory(string basedir = "")
        {
            _baseDirPath = Utils.PathCombine(basedir);
            if (string.IsNullOrEmpty(_baseDirPath))
            {
                _baseDirPath = Utils.PathCombine(Directory.GetCurrentDirectory());
            }

            var logsdir = Utils.PathCombine(_baseDirPath, "Logs");
            if (!Directory.Exists(logsdir))
            {
                Directory.CreateDirectory(logsdir);
            }

            var _infoDirPath = Utils.PathCombine(logsdir, "Info");
            if (!Directory.Exists(_infoDirPath))
            {
                Directory.CreateDirectory(_infoDirPath);
            }

            var _debugDirPath = Utils.PathCombine(logsdir, "Debug");
            if (!Directory.Exists(_debugDirPath))
            {
                Directory.CreateDirectory(_debugDirPath);
            }

            var _errorDirPath = Utils.PathCombine(logsdir, "Error");
            if (!Directory.Exists(_errorDirPath))
            {
                Directory.CreateDirectory(_errorDirPath);
            }

            return GetInstance();
        }


        public string GetErrorDirPath()
        {
            return _errorDirPath;
        }
        public static void Info(object sender, object msg)
        {
            if (string.IsNullOrEmpty(_infoDirPath))
            {
                GetInstance().BuildLogDirectory();
            }
            if ((int)LogLevel.Info >= (int)GetInstance().LogLv)
            {
                LogCore.WriteTask(new LogTask(LogLevel.Info, sender, msg), _infoDirPath);
            }
        }

        public static void Debug(object sender, object msg)
        {
            if (string.IsNullOrEmpty(_debugDirPath))
            {
                GetInstance().BuildLogDirectory();
            }
            if ((int)LogLevel.Debug >= (int)GetInstance().LogLv)
            {
                LogCore.WriteTask(new LogTask(LogLevel.Debug, sender, msg), _debugDirPath);
            }
        }
        /// <summary>
        /// Error日志主要为了捕捉异常，采用的同步模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="msg"></param>
        public static void Error(object sender, object msg)
        {
            if (string.IsNullOrEmpty(_errorDirPath))
            {
                GetInstance().BuildLogDirectory();
            }
            if ((int)LogLevel.Error >= (int)GetInstance().LogLv)
            {
                LogCore.WriteTask(new LogTask(LogLevel.Error, sender, msg), _errorDirPath);
            }
        }

    }
    /// <summary>
    /// Info : 0; Debug : 1; Error : 2
    /// </summary>
    public enum LogLevel
    {
        Info,
        Debug,
        Error
    }
}
