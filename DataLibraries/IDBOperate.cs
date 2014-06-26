using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace DataLibraries
{
    public interface IDBOperate
    {
        bool Insere(object model);

        bool Update(object model);

        bool Delete(object model);

        T Get<T>(object id) where T : new();

        DataTable QueryDt(string sql, params IDataParameter[] args);

        IList<T> QueryList<T>(string sql, params IDataParameter[] args) where T : new();

        IDbCommand Command { get; }

        IDbConnection Connection { get;}
        /// <summary>
        /// 获取并开启事务
        /// </summary>
        IDbTransaction Transaction { get;}

        int ExecuteNonQuery(string sql,params IDataParameter[] parms);

        object ExecuteScalar(string sql, params IDataParameter[] parms);

        IDataReader ExecuteReader(string sql, params IDataParameter[] parms);

        IDataReader ExecuteReader(CommandBehavior behavior,string sql,params IDataParameter[] parms);

        IDataParameter CreateParameter();
    }
}
