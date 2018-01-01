using System;
using LiteDB;

namespace LabManager.DbModel
{
    public class LiteDbAdapter : IDbAdapter
    {
        private LiteDatabase Instance { get; }

        public LiteDbAdapter(string dbName)
        {
            Instance = new LiteDatabase(dbName);
        }

        public virtual void Command(Action<LiteDatabase> command)
        {
            command(Instance);
        }

        public virtual TQueryResult Query<TQueryResult>(Func<LiteDatabase, TQueryResult> query)
        {
            return query(Instance);
        }
    }
}