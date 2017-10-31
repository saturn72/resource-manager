using System;

namespace QAutomation.Core.Domain.Logging
{
    public class LogRecordDomainModel : DomainModelBase
    {
        public LogLevel LogLevel { get; set; }
        public string ShortMessage { get; set; }
        public string FullMessage { get; set; }
        public Guid ContextId { get; set; }
    }
}