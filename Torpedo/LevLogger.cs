namespace Torpedo
{
    enum LogLevel { LogOff, LogError, LogInfo, LogDebug };

    internal class LevLog
    {
        private LogLevel logLevel;
        private string logDescription;

        public LogLevel LogLevel { get { return logLevel; } }
        public string LogDescription { get { return logDescription; } }

        public LevLog(LogLevel logLevel, string logDescription)
        {
            this.logLevel = logLevel;
            this.logDescription = logDescription;
        }

        public override string ToString()
        {
            return $"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second} [{logLevel.ToString().Remove(0, 3)}] : {logDescription}";
        }
    }

    internal class LevLogger
    {
        // 0 - log off
        // 1 - Errors
        // 2 - Info
        // 3 - Debug
        private LogLevel _loggingLevel;
        private string logLocation;
        private List<LevLog> _logs;

        public LevLogger(LogLevel loggingLevel, string logLocation)
        {
            _loggingLevel = loggingLevel;
            this.logLocation = logLocation;
            _logs = new List<LevLog>();
        }

        public void AddLog(LevLog log)
        {
            if (log.LogLevel != LogLevel.LogOff)
            {
                _logs.Add(log);
                Console.WriteLine(log.ToString());
                //File.AppendAllText(logLocation, log.ToString()+"\n");
            }
        }
    }
}
