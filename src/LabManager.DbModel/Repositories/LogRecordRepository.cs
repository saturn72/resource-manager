using System.Collections.Generic;
using Saturn72.Core.Domain.Logging;
using Saturn72.Core.Services.Logging;

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
            _dbAdapter.DeleteById<LogRecordModel>(logRecordId);
        }

        public IEnumerable<LogRecordModel> GetAll()
        {
            return _dbAdapter.GetAll<LogRecordModel>();
        }

        public LogRecordModel GetById(long logRecordId)
        {
            return _dbAdapter.GetById<LogRecordModel>(logRecordId);
        }

        public long Create(LogRecordModel logRecordModel)
        {
            _dbAdapter.Create(logRecordModel);
            return logRecordModel.Id;
        }
    }
}