using System;
using NLog;

namespace DHTSpider.Core.Logging
{
    public sealed class Logger
    {
        private static NLog.Logger logger = null;

        private static Logger instance = null;
        public static Logger Instance
        {
            get
            {
                if(instance == null)
                {
                    throw new InvalidOperationException("Logger is not initialed");
                }

                return instance;
            }
        }

        public static bool InitLogger(string name = "")
        {
            instance = new Logger(name);
            return true;
        }
        
        public Logger(string name = "")
        {
            if (!string.IsNullOrEmpty(name))
            {
                logger = LogManager.GetLogger(name);
            }
            else
            {
                logger = LogManager.GetCurrentClassLogger();
            }
        }

        private void WriteMessage(LogLevel level, string message)
        {
            logger.Log(level, message);
        }

        public void Fatal(string logContent)
        {
            WriteMessage(LogLevel.Fatal, logContent);
        }

        public void Fatal(string format, params object[] objValues)
        {
            WriteMessage(LogLevel.Fatal, string.Format(format, objValues));
        }

        public void Error(string logContent)
        {
            WriteMessage(LogLevel.Error, logContent);
        }

        public void Error(string format, params object[] objValues)
        {
            WriteMessage(LogLevel.Error, string.Format(format, objValues));
        }

        public void Error(Exception exception)
        {
            WriteMessage(LogLevel.Fatal, exception.ToString());
        }

        public void Info(string logContent)
        {
            WriteMessage(LogLevel.Info, logContent);
        }

        public void Info(string format, params object[] objValues)
        {
            WriteMessage(LogLevel.Info, string.Format(format, objValues));
        }

        public void Warn(string logContent)
        {
            WriteMessage(LogLevel.Warn, logContent);
        }

        public void Warn(string format, params object[] objValues)
        {
            WriteMessage(LogLevel.Warn, string.Format(format, objValues));
        }

        public void Debug(string logContent)
        {
            WriteMessage(LogLevel.Debug, logContent);
        }

        public void Debug(string format, params object[] objValues)
        {
            WriteMessage(LogLevel.Debug, string.Format(format, objValues));
        }
    }
}
