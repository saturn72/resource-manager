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
                var entity = BsonMapper.Global.Entity<ResourceModel>();
                entity.Id(x => x.Id);
                entity.Ignore(x => x.Status);

                EnsureIndexes(db);
            }
        }

        private static void EnsureIndexes(LiteDatabase db)
        {
            var col = GetCollection<ResourceModel>(db);
            col.EnsureIndex(s => s.IpAddress);
            col.EnsureIndex(s => s.FriendlyName);
        }

        private static LiteCollection<T> GetCollection<T>(LiteDatabase db)
        {
            return db.GetCollection<T>(LiteDbEntitiesNames.EntityToDocumentName[typeof(T)]);
        }
    }
}