using System;
using Moq;
using QAutomation.Core.Domain.Logging;
using System.Linq;
using QAutomation.Core.Services.Logging;
using Shouldly;
using Xunit;

namespace QAutomation.Core.Services.Tests.Logging
{
    public class DbLoggerTests
    {
        [Fact]
        public void DbLogger_DeleteLogRecord()
        {
            var lrRepo = new Mock<ILogRecordRepository>();
            var dbLogger = new DbLogger(lrRepo.Object);
            dbLogger.DeleteLogRecord(123);
            lrRepo.Verify(l => l.Delete(It.IsAny<long>()), Times.Once);
        }

        [Fact]
        public void DbLogger_GetAll()
        {
            var lrRepo = new Mock<ILogRecordRepository>();
            var expLogRecord = new[] {new LogRecordDomainModel()};
            lrRepo.Setup(l => l.GetAll()).Returns(expLogRecord);

            var dbLogger = new DbLogger(lrRepo.Object);
            dbLogger.GetAllLogRecords().ShouldBe(expLogRecord);
        }

        [Fact]
        public void DbLogger_GetById()
        {
            var lrRepo = new Mock<ILogRecordRepository>();
            var expLogRecord = new LogRecordDomainModel();
            lrRepo.Setup(l => l.GetById(It.IsAny<long>())).Returns(expLogRecord);

            var dbLogger = new DbLogger(lrRepo.Object);
            dbLogger.GetLogById(123).ShouldBe(expLogRecord);
        }

        [Fact]
        public void DbLogger_Insert()
        {
            var lrRepo = new Mock<ILogRecordRepository>();

            var exp = new LogRecordDomainModel
            {
                LogLevel = LogLevel.Error,
                ShortMessage = "shortMessage",
                FullMessage = "fullMessage",
                ContextId = new Guid()
            };
            var dbLogger = new DbLogger(lrRepo.Object);

            dbLogger.InsertLog(exp.LogLevel, exp.ShortMessage, exp.FullMessage, exp.ContextId);

            lrRepo.Verify(l => l.Create(It.Is<LogRecordDomainModel>(lr =>
                lr.LogLevel == exp.LogLevel &&
                lr.ShortMessage == exp.ShortMessage &&
                lr.FullMessage == exp.FullMessage &&
                lr.ContextId == exp.ContextId)), Times.Once);
        }

        [Fact]
        public void DbLogger_SupportedLogLevels()
        {
            var dbLogger = new DbLogger(null);
            dbLogger.SupportedLogLevels.ShouldBe(LogLevel.AllSystemLogLevels.ToArray());
        }
    }
}