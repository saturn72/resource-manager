using System;
using System.Collections.Generic;
using System.Linq;
using LiteDB;
using Saturn72.Core.Domain;

namespace LabManager.DbModel
{
    public static class DbAdapterExtensions
    {
        public static void DeleteById<TDomainModel>(this IDbAdapter dbAdapter, long id)
            where TDomainModel : DomainModelBase
        {
            dbAdapter.Command(db =>
            {
                GetCollection<TDomainModel>(db).Delete(id);
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

        public static IEnumerable<TDomainModel> GetBy<TDomainModel>(this IDbAdapter dbAdapter,
            Func<TDomainModel, bool> query)
            where TDomainModel : DomainModelBase
        {
            return dbAdapter.GetAll<TDomainModel>().Where(query).ToArray();
        }

        public static void Create<TDomainModel>(this IDbAdapter dbAdapter, TDomainModel model)
            where TDomainModel : DomainModelBase
        {
            dbAdapter.Command(db =>
            {
                GetCollection<TDomainModel>(db).Insert(model);
            });
        }

        public static void Update<TDomainModel>(this IDbAdapter dbAdapter, TDomainModel model)
            where TDomainModel : DomainModelBase
        {
            dbAdapter.Command(db =>
            {
                GetCollection<TDomainModel>(db).Update(model);
            });
        }
    }
}