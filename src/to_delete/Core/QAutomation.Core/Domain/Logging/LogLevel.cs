using System.Collections.Generic;

namespace QAutomation.Core.Domain.Logging
{
    public sealed class LogLevel
    {
        public static readonly LogLevel Debug = new LogLevel(10, "debug");
        public static readonly LogLevel Trace = new LogLevel(20, "trace");
        public static readonly LogLevel Information = new LogLevel(30, "information");
        public static readonly LogLevel Warning = new LogLevel(40, "warning");
        public static readonly LogLevel Error = new LogLevel(50, "error");
        public static readonly LogLevel Fatal = new LogLevel(60, "fatal");

        public static readonly IEnumerable<LogLevel> AllSystemLogLevels =
            new[]
            {Debug, Information, Warning, Error, Fatal, Trace};

        private readonly int _code;
        private readonly string _name;

        private LogLevel(int code, string name)
        {
            _code = code;
            _name = name;
        }

        public int Code
        {
            get { return _code; }
        }

        public string Name
        {
            get { return _name; }
        }
    }
}