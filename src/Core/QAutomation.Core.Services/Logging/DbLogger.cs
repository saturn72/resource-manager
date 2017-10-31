using System;
using System.Collections.Generic;
using System.Linq;
using QAutomation.Core.Domain.Logging;

namespace QAutomation.Core.Services.Logging
{
    public class DbLogger : ILogger
    {
        #region Fields

        private readonly ILogRecordRepository _logRecordRepository;

        #endregion

        #region Ctor

        public DbLogger(ILogRecordRepository logRecordRepository)
        {
            _logRecordRepository = logRecordRepository;
        }

        #endregion

        public LogLevel[] SupportedLogLevels => LogLevel.AllSystemLogLevels.ToArray();


        public void DeleteLogRecord(long logRecordId)
        {
            _logRecordRepository.Delete(logRecordId);
        }

        public IEnumerable<LogRecordDomainModel> GetAllLogRecords()
        {
            return _logRecordRepository.GetAll();
        }

        public LogRecordDomainModel GetLogById(long logRecordId)
        {
            return _logRecordRepository.GetById(logRecordId);
        }

        public LogRecordDomainModel InsertLog(LogLevel logLevel, string shortMessage, string fullMessage = "",
            Guid contextId = new Guid())
        {
            var lr = new LogRecordDomainModel
            {
                LogLevel = logLevel,
                ShortMessage = shortMessage,
                FullMessage = fullMessage,
                ContextId = contextId
            };
            _logRecordRepository.Create(lr);
            return lr;
        }
    }
}