using System;
using MediaBrowser.Model.Logging;

namespace AdultEmby.Plugins.TestLogging
{
    /// <summary>
    /// Class NlogManager
    /// </summary>
    public class TestLogManager : ILogManager
    {
        #region Private Fields

        private LogSeverity _severity = LogSeverity.Debug;

        #endregion

        #region Event Declarations

        /// <summary>
        /// Occurs when [logger loaded].
        /// </summary>
        public event EventHandler LoggerLoaded;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the log file path.
        /// </summary>
        /// <value>The log file path.</value>
        public string LogFilePath { get; private set; }

        /// <summary>
        /// Gets or sets the exception message prefix.
        /// </summary>
        /// <value>The exception message prefix.</value>
        public string ExceptionMessagePrefix { get; set; }

        public LogSeverity LogSeverity
        {

            get
            {
                return _severity;
            }

            set
            {
                _severity = value;
            }
        }

        #endregion

        #region Public Methods

        static readonly TestLogManager _instance = new TestLogManager();
        public static TestLogManager Instance
        {
            get
            {
                return _instance;
            }
        }
        TestLogManager()
        {
            // Initialize.
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>ILogger.</returns>
        public MediaBrowser.Model.Logging.ILogger GetLogger(string name)
        {
            return new TestLogger(name, this);
        }

        public void AddConsoleOutput()
        {
        }

        public void RemoveConsoleOutput()
        {
        }

        /// <summary>
        /// Reloads the logger, maintaining the current log level.
        /// </summary>
        public void ReloadLogger()
        {
        }

        /// <summary>
        /// Reloads the logger, using the specified logging level.
        /// </summary>
        /// <param name="level">The level.</param>
        public void ReloadLogger(LogSeverity level)
        {
        }

        /// <summary>
        /// Flushes this instance.
        /// </summary>
        public void Flush()
        {
        }

        #endregion

    }
}