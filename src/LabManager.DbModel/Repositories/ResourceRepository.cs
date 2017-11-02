using System;
using System.Collections.Generic;
using System.Linq;
using LabManager.Common.Domain.Resource;
using LabManager.Services.Resources;

namespace LabManager.DbModel.Repositories
{
    public class ResourceRepository : IResourceRepository
    {
        #region Fields
        private readonly IDbAdapter _dbAdapter;
        #endregion

        #region ctor
        public ResourceRepository(IDbAdapter dbAdapter)
        {
            _dbAdapter = dbAdapter;
        }
        #endregion

        public long Create(ResourceModel resourceModel)
        {
            _dbAdapter.Command(db =>
            {
                var col = db.GetCollection<ResourceModel>();
                col.Insert(resourceModel);
            });
            return resourceModel.Id;
        }

        public IEnumerable<ResourceModel> GetAll()
        {
            var res = _dbAdapter.GetAll<ResourceModel>();
            foreach (var item in res)
                item.Status = ResourceStatus.Unknown;
            return res;
        }

        public ResourceModel GetById(long id)
        {
            return _dbAdapter.GetById<ResourceModel>(id);
        }


        public IEnumerable<ResourceModel> GetBy(Func<ResourceModel, bool> query)
        {
            return _dbAdapter.GetBy(query);
        }
    }
}