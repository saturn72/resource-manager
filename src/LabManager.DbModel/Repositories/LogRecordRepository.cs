using System.Collections.Generic;
using QAutomation.Core.Domain.Logging;
using QAutomation.Core.Services.Logging;

namespace LabManager.DbModel.Repositories
{
    public class LogRecordRepository : ILogRecordRepository
    {
        #region Fields

        private readonly IDbAdapter _dbAdapter;

        #endregion

        #region ctor

        public LogRecordRepository(IDbAdapter dbAdapter)
        {
            _dbAdapter = dbAdapter;
        }

        #endregion

        public void Delete(long logRecordId)
        {
            _dbAdapter.DeleteById<LogRecordDomainModel>(logRecordId);
        }

        public IEnumerable<LogRecordDomainModel> GetAll()
        {
            return _dbAdapter.GetAll<LogRecordDomainModel>();
        }

        public LogRecordDomainModel GetById(long logRecordId)
        {
            return _dbAdapter.GetById<LogRecordDomainModel>(logRecordId);
        }

        public long Create(LogRecordDomainModel logRecordModel)
        {
            _dbAdapter.Create(logRecordModel);
            return logRecordModel.Id;
        }
    }
}