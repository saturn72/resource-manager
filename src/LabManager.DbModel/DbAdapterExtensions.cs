using System.Collections.Generic;
using LiteDB;
using QAutomation.Core.Domain;
using System.Linq;

namespace LabManager.DbModel
{
    public static class DbAdapterExtensions
    {
        public static void DeleteById<TDomainModel>(this IDbAdapter dbAdapter, long id)
            where TDomainModel : DomainModelBase
        {
            dbAdapter.Command(db =>
            {
                var col = GetCollection<TDomainModel>(db);
                col.Delete(id);
            });
        }

        public static IEnumerable<TDomainModel> GetAll<TDomainModel>(this IDbAdapter dbAdapter)
            where TDomainModel : DomainModelBase
        {
            return dbAdapter.Query(db => GetCollection<TDomainModel>(db).FindAll().ToArray());
        }

        private static LiteCollection<TDomainModel> GetCollection<TDomainModel>(LiteDatabase db)
            where TDomainModel : DomainModelBase
        {
            return db.GetCollection<TDomainModel>(typeof(TDomainModel).Name);
        }

        public static TDomainModel GetById<TDomainModel>(this IDbAdapter dbAdapter, long id)
            where TDomainModel : DomainModelBase
        {
            return dbAdapter.Query(db => GetCollection<TDomainModel>(db).FindById(id));
        }

        public static void Create<TDomainModel>(this IDbAdapter dbAdapter, TDomainModel model)
            where TDomainModel : DomainModelBase
        {

            dbAdapter.Command(db =>
            {
                var col = GetCollection<TDomainModel>(db);
                col.Insert(model);
            });
        }
    }
}