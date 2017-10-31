using LabManager.Common.Domain.Resource;
using LiteDB;

namespace LabManager.DbModel.Infrastructure
{
    public class LiteDbMapper
    {
        public static void Map(string dbName)
        {
            using (var db = new LiteDatabase(dbName))
            {
                //var col = db.GetCollection<ResourceModel>(LiteDbEntitiesNames.EntityToDocumentName[typeof(ResourceModel)]);
                //col.
                var entity = BsonMapper.Global.Entity<ResourceModel>();
                entity.Id(x => x.Id);
                entity.Ignore(x => x.Status);
            }
           
        }
    }
}
