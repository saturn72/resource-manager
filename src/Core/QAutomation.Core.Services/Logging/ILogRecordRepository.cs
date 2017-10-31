using System.Collections.Generic;
using QAutomation.Core.Domain.Logging;

namespace QAutomation.Core.Services.Logging
{
    public interface ILogRecordRepository
    {
        void Delete(long logRecordId);
        IEnumerable<LogRecordDomainModel> GetAll();
        LogRecordDomainModel GetById(long logRecordId);
        long Create(LogRecordDomainModel logRecordModel);
    }
}