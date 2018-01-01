using System;
using LiteDB;

namespace LabManager.DbModel
{
    public interface IDbAdapter
    {
        void Command(Action<LiteDatabase> command);
        TQueryResult Query<TQueryResult>(Func<LiteDatabase, TQueryResult> query);
    }
}